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
        user_id = str(uuid.uuid4())
        self.client.cookies.update({"ai_user": user_id, "ai_session": user_id})
        self.client.headers.update({"User-Agent": f"LocustUser/{user_id}"})

    @task(3)
    def index(self):
        # Giả lập việc "Locust Client" cũng là một server đang xử lý task nội bộ
        # Điều này giúp nó hiện lên như một Role riêng trong Live Metrics
        if tracer:
            with tracer.start_as_current_span("ClientProcessing", kind=SpanKind.SERVER):
                time.sleep(0.1) # Fake processing time
                self.client.get("/")

    @task(1)
    def view_items(self):
        if tracer:
            with tracer.start_as_current_span("ClientBrowsing", kind=SpanKind.SERVER):
                self.client.get("/Home/ProductList")

    @task(2)
    def view_item_details(self):
        if tracer:
            with tracer.start_as_current_span("ClientViewingDetail", kind=SpanKind.SERVER):
                self.client.get("/Home/ViewItem/1")

    @task(1)
    def contact_page(self):
        if tracer:
            with tracer.start_as_current_span("ClientContacting", kind=SpanKind.SERVER):
                self.client.get("/Home/Contact")
