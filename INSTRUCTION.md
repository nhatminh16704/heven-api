# Heven.Api - Phát triển và Luồng Code Chuẩn (Clean Architecture)

Dự án Heven.Api áp dụng kiến trúc **Clean Architecture** theo template Jason Taylor với các công nghệ lõi: .NET 8, MediatR (CQRS), Minimal APIs, và EF Core.

Đây là tài liệu hướng dẫn **Quick Start** để khi thêm một tính năng mới (Feature) bạn biết chính xác cần tạo file ở đâu, cấu hình vào file nào và làm sao để không làm gãy hệ thống.

---

## 1. Tầng Domain (Luật Doanh nghiệp & Object Cốt lỗi)

**Vị trí:** `src/Domain/`

Khi có bảng mới trong cơ sở dữ liệu:

- **Entities:** Tạo model trong `src/Domain/Entities/`.
  - Luôn ưu tiên kế thừa `BaseEntity` (Không cần ngày giờ) hoặc `BaseAuditableEntity` (Tự động track Created, Updated, CreatedBy).
  - _Ví dụ:_ `public class Notification : BaseAuditableEntity { ... }`
- **Khóa ngoại (FK) dẫn đến `User`:** Vì User thuộc tầng Infrastructure (Identity), để tránh phá vỡ giới hạn tầng, **Tuyệt đối không dùng class ApplicationUser** trong file Entity. Bạn cần dùng kiểu `string` để lưu ID. `public required string UserId { get; set; }`
- **Enums:** Định nghĩa trạng thái trong `src/Domain/Enums/`. (Ví dụ: Enum trạng thái Booking).

---

## 2. Tầng Application (Luồng CQRS + MediatR)

**Vị trí:** `src/Application/`

Khi làm một API (Ví dụ: Thêm mới Property, Lấy danh sách Bookings), chúng ta **không viết code ở Controller**. Thay vào đó làm theo chuẩn CQRS:

### 2.1. Cấu trúc thư mục theo Tính năng (Feature-based)

Mỗi Model tạo ra 1 thư mục riêng. Bên trong chia thành `Commands` (Thay đổi dữ liệu) và `Queries` (Chỉ đọc dữ liệu).
_Ví dụ:_ `src/Application/Bookings/Commands/CreateBooking/`

### 2.2. Command / Query

Trong thư mục trên, tạo 2 thành phần (thường nằm chung 1 file gốc):

- **Command/Query Record:** Ví dụ: `public record CreateBookingCommand(int ListingId, string GuestId) : IRequest<int>;`
- **Handler Class:** Implement từ `IRequestHandler<TCommand, TResult>`.
  - Tại đây Inject `IApplicationDbContext` để tương tác EF Core xử lý DB.
- **Validator (Tùy chọn):** Kế thừa từ `AbstractValidator<CreateBookingCommand>` chạy thư viện FluentValidation để set luật dữ liệu.

---

## 3. Tầng Infrastructure (Database & Identity)

**Vị trí:** `src/Infrastructure/`

Mọi thứ liên kết với công nghệ bên ngoài (DB, Auth) nằm ở đây.

- **DbContext:** Khai báo Entity mới làm `DbSet` trong `src/Infrastructure/Data/ApplicationDbContext.cs`.
- **Fluent API:** Nếu Entity có Relationship phức tạp (đặc biệt là nhiều bảng cùng đổ về 1 bảng gây báo lỗi Cascade Path của EF) -> Vào hàm `OnModelCreating` trong DbContext mà gỡ bằng cách `OnDelete(DeleteBehavior.Restrict)`.
- **Lệnh Migration:** Khi thêm bảng, luôn luôn chạy đủ 2 câu thần chú:
  ```bash
  dotnet ef migrations add TenMigration --project src/Infrastructure/Heven.Infrastructure.csproj
  dotnet ef database update --project src/Infrastructure/Heven.Infrastructure.csproj
  ```

  - Lệnh đầu tiên tạo file Migration, lệnh thứ hai đẩy thay đổi cấu trúc xuống DB.
  - Luôn kiểm tra kĩ phần `.csproj` để tránh quên không đưa file Migration vào dự án đúng.
- _Lưu ý về Seeding Data:_ Có một file `ApplicationDbContextInitialiser.cs` chuyên cắm dữ liệu mẫu. Nếu cấu trúc DB bị gãy thì hay sập lỗi ở đây, bỏ chặn nó ngay nếu lỡ tay làm vỡ DB để update lại.

---

## 4. Tầng Web (Endpoints & API Config)

**Vị trí:** `src/Web/`

Toàn bộ ứng dụng của Heven.Api sử dụng **Minimal APIs** phân chia theo class (Group Endpoint).

### 4.1. Thêm Endpoint mới

Thay vì dùng [ApiController], vào `src/Web/Endpoints/` tạo file Endpoint kế thừa `EndpointGroupBase`.
Ví dụ:

### 4.2. Hệ thống Tự Động Định Dạng Response & Bắt Lỗi (Đã Chuẩn Hóa)

Toàn bộ Minimal APIs đã được **Tự Động Bọc Response** thông qua `ApiResponseEndpointFilter.cs` và `CustomExceptionHandler.cs`.
👉 _Bạn trả về cái gì, thư viện JSON cũng tự định dạng thành format thành `{ "success": true, "data": ..., "error": null }`. Do đó KHÔNG CẦN DÙNG TAY GỌI ApiResponse thủ công ở Endpoints._

### 4.3. Fix lỗi Build Failed từ NSwag

Project dùng `NSwag` để xuất Swagger ra file Document Json ở bước **Build**, nếu Database Schema không khớp sẽ văng lỗi Build Failed ảo.
**Xử lý:** Vào `src/Web/Web.csproj`, tìm tag `<Target Name="NSwag">`, thêm chuỗi `;RunDbInit=False` vào phần `EnvironmentVariables`. Điều này sẽ cấm Nswag khởi động DB mồi và giúp app tự bềnh bồng build thành công!

---

## 5. Hệ Thống Caching (Redis)

**Công nghệ:** `StackExchange.Redis` + `IDistributedCache`

Heven.Api sử dụng Redis làm cache layer với 2 chiến lược rõ ràng: **Tự động cache khi đọc** và **Xóa cache bằng Event** khi dữ liệu thay đổi.

### 5.1. Read — Tự Động Cache Qua `ICacheable` + `CachingBehavior`

Mọi Query muốn được cache, chỉ cần implement thêm interface `ICacheable` và khai báo `CacheKey` (tên key trong Redis) + `Expiration` (TTL).

`CachingBehavior` (một MediatR Pipeline Behavior) sẽ **tự động**:

1. Kiểm tra Redis xem `CacheKey` đã có chưa.
2. Nếu **có** → Deserialize và trả về ngay, **không** xuống DB.
3. Nếu **chưa có** → Cho Request chạy bình thường → Lưu kết quả vào Redis với TTL từ `Expiration`.

> ⚠️ **Chỉ dùng `ICacheable` cho `Queries`**, không dùng cho `Commands`.

---

### 5.2. Write — Xóa Cache Bằng Event-Driven (MediatR Notifications)

Khi một `Command` thay đổi dữ liệu (Create/Update/Delete), **không xóa cache trực tiếp trong Handler**. Thay vào đó, bắn một Domain Notification sau khi lưu DB xong.

**Luồng chuẩn:**

```
Command Handler
  └─► Lưu DB thành công
       └─► Publish Notification (INotification)
            └─► Cache Invalidation Handler nghe & xóa Key tương ứng
```

Khai báo `INotification` tương ứng trong `Application/<Feature>/Events/`, publish nó ở cuối Command Handler, sau đó tạo `INotificationHandler` riêng để inject `IDistributedCache` và gọi `RemoveAsync` đúng key cần xóa.

> 💡 **Lợi ích:** Command Handler không cần biết gì về cache. Mọi logic invalidation nằm gọn trong Event Handler — dễ mở rộng, dễ test độc lập.

---

**Luôn nhớ:** Dependency Injection chỉ chảy 1 chiều từ ngoài vào trong: `Web` -> `Infrastructure` -> `Application` -> `Domain`. Đừng reference chéo nhau làm gãy code!
