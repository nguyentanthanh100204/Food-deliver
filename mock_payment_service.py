"""
Mock Project 2 - Fake Server để Demo Multi-Project Monitoring
==============================================================

Mục đích: 
- Giả lập một project/service thứ 2 
- Gửi telemetry tới cùng Azure Application Insights với Food Ordering App
- Demo multi-project monitoring trên Application Map

Cách hoạt động:
- Script này giả lập một backend API service
- Tự động generate fake requests/dependencies mỗi vài giây
- Mỗi request tạo telemetry data gửi tới Azure Application Insights
"""

from azure.monitor.opentelemetry import configure_azure_monitor
from opentelemetry import trace
from opentelemetry.trace import SpanKind
import time
import random
import os

# === CONFIGURATION ===
CONNECTION_STRING = "InstrumentationKey=d404027d-0fc4-4c64-a58a-1f4466aecc40;IngestionEndpoint=https://koreacentral-0.in.applicationinsights.azure.com/;LiveEndpoint=https://koreacentral.livediagnostics.monitor.azure.com/;ApplicationId=0d0da481-ff00-4898-824c-774bf187aa87"

# Đặt tên cho project ảo (sẽ hiện trong Application Map)
os.environ["OTEL_SERVICE_NAME"] = "Payment-Service-API"  # Tên project ảo
os.environ["OTEL_RESOURCE_ATTRIBUTES"] = "service.namespace=FoodOrderingSystem,service.version=2.0.0"

# === SETUP AZURE MONITOR ===
try:
    configure_azure_monitor(connection_string=CONNECTION_STRING)
    tracer = trace.get_tracer(__name__)
    print("✅ Azure Monitor configured successfully!")
    print(f"📦 Service Name: Payment-Service-API")
    print("🔄 Starting to generate fake telemetry...")
    print("=" * 60)
except ImportError:
    print("❌ ERROR: Run 'pip install azure-monitor-opentelemetry'")
    exit(1)
except Exception as e:
    print(f"❌ ERROR: {e}")
    exit(1)

# === FAKE OPERATIONS ===
OPERATIONS = [
    ("ProcessPayment", 0.5, 2.0, SpanKind.SERVER),      # 500ms - 2s
    ("ValidateCard", 0.1, 0.5, SpanKind.INTERNAL),      # 100ms - 500ms 
    ("CallPaymentGateway", 1.0, 3.0, SpanKind.CLIENT),  # 1s - 3s (external)
    ("UpdateOrderStatus", 0.2, 0.8, SpanKind.INTERNAL), # 200ms - 800ms
    ("SendConfirmationEmail", 0.3, 1.0, SpanKind.CLIENT) # 300ms - 1s (external)
]

def generate_fake_telemetry():
    """Generate một fake request với nested spans"""
    
    # Random chọn operation
    op_name, min_duration, max_duration, span_kind = random.choice(OPERATIONS)
    
    # Tạo parent span (incoming request)
    with tracer.start_as_current_span(
        f"POST /api/payment/{op_name.lower()}", 
        kind=SpanKind.SERVER
    ) as parent_span:
        
        # Set attributes
        parent_span.set_attribute("http.method", "POST")
        parent_span.set_attribute("http.url", f"https://payment-api.com/api/payment/{op_name.lower()}")
        parent_span.set_attribute("http.status_code", random.choice([200, 200, 200, 500]))  # 25% fail rate
        
        # Simulate processing time
        duration = random.uniform(min_duration, max_duration)
        
        # Tạo child span (internal operation)
        with tracer.start_as_current_span(op_name, kind=span_kind) as child_span:
            child_span.set_attribute("operation.type", op_name)
            
            # Nếu là external call, thêm dependency info
            if span_kind == SpanKind.CLIENT:
                child_span.set_attribute("peer.service", "external-payment-gateway")
                child_span.set_attribute("db.system", "postgresql")  # Fake DB
            
            time.sleep(duration)
        
        # Log operation
        status = "✅" if parent_span.attributes.get("http.status_code") == 200 else "❌"
        print(f"{status} {op_name:25s} | {duration:.2f}s | Status: {parent_span.attributes.get('http.status_code')}")

def main():
    """Main loop - generate telemetry mỗi 2-5 giây"""
    
    request_count = 0
    
    try:
        while True:
            request_count += 1
            print(f"\n[Request #{request_count}]")
            
            # Generate fake telemetry
            generate_fake_telemetry()
            
            # Wait random interval (2-5 seconds)
            wait_time = random.uniform(2, 5)
            print(f"⏱️  Waiting {wait_time:.1f}s before next request...")
            time.sleep(wait_time)
            
    except KeyboardInterrupt:
        print("\n\n🛑 Stopped by user")
        print(f"📊 Total requests generated: {request_count}")
        print("=" * 60)

if __name__ == "__main__":
    print("\n" + "=" * 60)
    print("🚀 Mock Payment Service API - Telemetry Generator")
    print("=" * 60)
    print("\nPress Ctrl+C to stop\n")
    
    main()
