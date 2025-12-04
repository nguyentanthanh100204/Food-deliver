# Hướng dẫn Load Testing với Locust

## 1. Cài đặt

Bạn cần cài đặt Python và các thư viện cần thiết:

```bash
pip install locust azure-monitor-opentelemetry
```

## 2. Cấu hình Connection String

1.  Mở file `locustfile.py`.
2.  Tìm dòng `CONNECTION_STRING = "..."`.
3.  Thay thế bằng **Connection String** thực tế của bạn (Lấy trong Azure Portal -> Application Insights -> Overview).

## 3. Chạy Load Test

Mở terminal tại thư mục chứa file `locustfile.py` và chạy lệnh:

```bash
locust -f locustfile.py
```

**Lưu ý:** Nếu bạn gặp lỗi "locust is not recognized...", hãy thử dùng lệnh sau:

```bash
python -m locust -f locustfile.py
```

## 3. Cấu hình và Bắt đầu

1.  Mở trình duyệt và truy cập: `http://localhost:8089`
2.  Nhập số lượng user (ví dụ: **100**)
3.  Nhập Spawn rate (ví dụ: **10** user/giây)
4.  Nhập Host (ví dụ: `http://localhost:port_cua_ban` - thay `port_cua_ban` bằng port thực tế ứng dụng đang chạy, ví dụ `http://localhost:5000`)
5.  Nhấn **Start swarming**

## 4. Theo dõi

-   Xem biểu đồ realtime trên giao diện web của Locust.
-   Kiểm tra hệ thống monitoring của bạn để xem traffic đổ về.
