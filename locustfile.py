from locust import HttpUser, task, between, events
import uuid
import random
import os
import time

# --- Azure Monitoring Setup ---
CONNECTION_STRING = "InstrumentationKey=d404027d-0fc4-4c64-a58a-1f4466aecc40;IngestionEndpoint=https://koreacentral-0.in.applicationinsights.azure.com/;LiveEndpoint=https://koreacentral.livediagnostics.monitor.azure.com/;ApplicationId=0d0da481-ff00-4898-824c-774bf187aa87" 

os.environ["OTEL_SERVICE_NAME"] = "Locust-LoadTest-Client"

try:
    from azure.monitor.opentelemetry import configure_azure_monitor
    from opentelemetry import trace
    from opentelemetry.trace import SpanKind

    configure_azure_monitor(connection_string=CONNECTION_STRING)
    
    tracer = trace.get_tracer(__name__)
    print("✅ Azure Monitor configured successfully! Service Name: Locust-LoadTest-Client")
except ImportError:
    print("❌ LỖI: Chưa cài đặt thư viện. Run: pip install azure-monitor-opentelemetry")
    tracer = None
except Exception as e:
    print(f"❌ LỖI: {e}")
    tracer = None
# ------------------------------

class WebsiteUser(HttpUser):
    wait_time = between(1, 5)

    def on_start(self):
        # Generate unique user ID for this Locust user
        self.user_id = str(uuid.uuid4())
        self.session_id = str(uuid.uuid4())
        
        # Set cookies (for browser-like tracking)
        self.client.cookies.update({
            "ai_user": self.user_id, 
            "ai_session": self.session_id
        })
        
        # Set custom headers to identify user
        self.client.headers.update({
            "User-Agent": f"LocustUser/{self.user_id}",
            "X-User-Id": self.user_id,  # Custom header để server track
        })
        
        print(f"🆔 User started: {self.user_id[:8]}...")

    @task(3)
    def index(self):
        # Giả lập việc "Locust Client" cũng là một server đang xử lý task nội bộ
        # Điều này giúp nó hiện lên như một Role riêng trong Live Metrics
        if tracer:
            with tracer.start_as_current_span("ClientProcessing", kind=SpanKind.SERVER) as span:
                # Add user tracking attributes
                span.set_attribute("user.id", self.user_id)
                span.set_attribute("user.session", self.session_id)
                span.set_attribute("enduser.id", self.user_id)  # OpenTelemetry standard
                
                time.sleep(0.1) # Fake processing time
                self.client.get("/")

    @task(1)
    def view_items(self):
        if tracer:
            with tracer.start_as_current_span("ClientBrowsing", kind=SpanKind.SERVER) as span:
                span.set_attribute("user.id", self.user_id)
                span.set_attribute("enduser.id", self.user_id)
                self.client.get("/Home/ProductList")

    @task(2)
    def view_item_details(self):
        if tracer:
            with tracer.start_as_current_span("ClientViewingDetail", kind=SpanKind.SERVER) as span:
                span.set_attribute("user.id", self.user_id)
                span.set_attribute("enduser.id", self.user_id)
                self.client.get("/Home/ViewItem/1")

    @task(1)
    def contact_page(self):
        if tracer:
            with tracer.start_as_current_span("ClientContacting", kind=SpanKind.SERVER) as span:
                span.set_attribute("user.id", self.user_id)
                span.set_attribute("enduser.id", self.user_id)
                self.client.get("/Home/Contact")
