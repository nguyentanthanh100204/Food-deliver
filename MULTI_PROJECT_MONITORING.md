# Hướng dẫn Cấu hình Monitoring cho Nhiều Dự án (Chung 1 Application Insights)

Để giám sát nhiều dự án trên cùng một Application Insights mà vẫn phân biệt được chúng, bạn cần đặt **Cloud Role Name** cho từng dự án.

Vì bạn đang sử dụng **Auto-instrumentation** (không cài SDK trong code), cách đơn giản nhất là cấu hình trên **Azure Portal**.

## Các bước thực hiện

### Bước 1: Xác định Application Insights chung
Đảm bảo cả 2 App Service của bạn đều đang trỏ về **cùng một** Application Insights Resource (cùng Instrumentation Key hoặc Connection String).

### Bước 2: Đặt tên cho từng dự án (Cloud Role Name)

Thực hiện lần lượt cho **từng App Service** (Project 1 và Project 2).

**Lưu ý:** Nếu hiện tại bạn mới chỉ có **1 dự án**, bạn vẫn nên thực hiện bước này cho dự án đó. Việc này giúp định danh rõ ràng dự án hiện tại trên bản đồ (Application Map). Sau này khi có dự án thứ 2, bạn chỉ cần làm tương tự với tên khác là xong.

1.  Truy cập **Azure Portal**.
2.  Vào **App Service** của dự án.
3.  Tìm menu **Settings** -> chọn **Environment variables** (hoặc **Configuration** ở giao diện cũ).
4.  Thêm một biến môi trường mới (App Setting):
    *   **Name:** `WEBSITE_CLOUD_ROLENAME`
    *   **Value:** Tên dự án bạn muốn hiển thị (Ví dụ: `FoodOrder-Web`, `FoodOrder-API`, `Project-A`, `Project-B`...)
5.  Nhấn **Apply** / **Save** và khởi động lại App Service nếu được yêu cầu.

### Bước 3: Kiểm tra kết quả

1.  Chạy ứng dụng hoặc tạo traffic (dùng Locust).
2.  Vào **Application Insights** -> **Application Map**.
3.  Bạn sẽ thấy sơ đồ hiển thị các node riêng biệt với tên bạn vừa đặt (thay vì tên chung chung), giúp bạn dễ dàng phân biệt traffic và lỗi của từng dự án.
