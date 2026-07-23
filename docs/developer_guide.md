# Developer Guide & Playbook - Cultivation RPG Bot

Tài liệu này đóng vai trò như một cẩm nang (Guide Book) dành cho các Developer khi tham gia phát triển dự án. Nó tổng hợp cấu trúc thư mục, các lệnh thường dùng (Commands), và quy trình triển khai (Workflows) các tính năng mới.

---

## 1. Cấu Trúc Dự Án (Project Structure)

Dự án được chia làm 2 thành phần chính để tách biệt Backend Logic và Presentation Layer:

- `src/CoreAPI/`: Backend viết bằng .NET 10 (C#), kiến trúc Clean Architecture.
  - `CultivationApi.Domain/`: Chứa các Entities (DB Models), hằng số (Constants), và Exceptions.
  - `CultivationApi.Application/`: Chứa Interfaces, DTOs, và Services (Logic game chính nằm ở đây).
  - `CultivationApi.Infrastructure/`: Kết nối cơ sở dữ liệu (PostgreSQL), Entity Framework Core (DbContext, Migrations).
  - `CultivationApi.WebApi/`: Lớp ngoài cùng, chứa các Controllers (Endpoints) và file cấu hình (appsettings.json, game_data.json).
- `src/DiscordBot/`: Frontend viết bằng Node.js 24 + TypeScript (Discord.js).
  - `src/api/`: Các class API Client để gọi HTTP Requests sang Core API (`CultivationApiClient.ts`).
  - `src/controllers/`: Xử lý logic tương tác của người dùng (Buttons, Modals, Slash Commands).
  - `src/events/`: Lắng nghe các sự kiện Discord (`ready`, `interactionCreate`, `messageCreate`).
  - `src/utils/`: Các hàm tiện ích, đặc biệt là `embedBuilder.ts` để vẽ giao diện UI.
- `docs/`: Các tài liệu kiến trúc, database schema, user flow và game design.

---

## 2. Các Lệnh Thường Dùng (Common Commands)

### 2.1. Dành cho Core API (.NET Core 10)

Mở terminal và trỏ vào thư mục `src/CoreAPI/` (hoặc các thư mục project cụ thể) để chạy:

- **Build Project (Kiểm tra lỗi cú pháp):**
  ```bash
  dotnet build
  ```
- **Chạy Project (Local Development):**
  ```bash
  dotnet run --project CultivationApi.WebApi
  ```
- **Cài đặt thư viện mới (ví dụ vào WebApi):**
  ```bash
  dotnet add CultivationApi.WebApi package <PackageName>
  ```

#### Entity Framework Core (Database Migrations)
*Lưu ý: Các lệnh EF Core phải được chạy tại thư mục `src/CoreAPI/` và chỉ định đúng project.*
- **Tạo Migration mới (Khi thay đổi/thêm bảng trong Domain Entities):**
  ```bash
  dotnet ef migrations add <MigrationName> --project CultivationApi.Infrastructure --startup-project CultivationApi.WebApi
  ```
- **Cập nhật Database (Áp dụng Migration vào DB PostgreSQL):**
  ```bash
  dotnet ef database update --project CultivationApi.Infrastructure --startup-project CultivationApi.WebApi
  ```
- **Xóa Migration gần nhất (Nếu lỡ tạo sai và chưa update db):**
  ```bash
  dotnet ef migrations remove --project CultivationApi.Infrastructure --startup-project CultivationApi.WebApi
  ```

### 2.2. Dành cho Discord Bot (TypeScript)

Mở terminal và trỏ vào thư mục `src/DiscordBot/`:

- **Cài đặt dependencies:**
  ```bash
  npm install
  ```
- **Kiểm tra lỗi kiểu dữ liệu (Type-check) mà không xuất file:**
  ```bash
  npx tsc --noEmit
  ```
  *(Trên Windows, dùng `npx.cmd tsc --noEmit`)*
- **Build TypeScript sang JavaScript:**
  ```bash
  npm run build
  ```
- **Chạy Bot (Chế độ Development / Watch):**
  ```bash
  npm run dev
  ```
- **Cài đặt thêm package mới:**
  ```bash
  npm install <package_name>
  npm install -D @types/<package_name>  # Nếu cần types
  ```

---

## 3. Quy Trình Triển Khai Tính Năng Mới (Workflows)

### Workflow A: Thêm một API Endpoint Mới (.NET Core)
1. **Define DTOs:** Thêm Request/Response DTO vào thư mục `CultivationApi.Application/DTOs/`.
2. **Khai báo Interface:** Thêm method tương ứng vào Interface ở `CultivationApi.Application/Interfaces/` (vd: `IActivitiesService.cs`).
3. **Implement Logic:** Viết logic xử lý trong Service tương ứng ở `CultivationApi.Application/Services/`. Đảm bảo sử dụng `async/await` và `CancellationToken`.
4. **Tạo/Sửa Controller:** Mở file controller trong `CultivationApi.WebApi/Controllers/` và thêm HTTP Method (GET/POST/PUT). Return kết quả qua `Ok()` hoặc throw `DomainException`.
5. **Kiểm tra:** Chạy lệnh `dotnet build`.

### Workflow B: Thêm/Sửa Cấu Trúc Database (Entity)
1. **Sửa Entity:** Sửa hoặc tạo class mới trong `CultivationApi.Domain/Entities/`.
2. **Cập nhật DbContext:** Thêm `DbSet<T>` vào `AppDbContext.cs` trong `CultivationApi.Infrastructure/Data/`. Nếu cần mapping nâng cao, cấu hình trong method `OnModelCreating`.
3. **Chạy Migration:** Chạy lệnh `dotnet ef migrations add <TênMigration>` (xem mục 2.1).
4. **Đồng bộ Tài Liệu:** Mở file `docs/database_schema.md` và cập nhật lại Diagram Mermaid ngay lập tức (Bắt buộc theo rule).

### Workflow C: Thêm Tính Năng Lên Discord Bot
1. **Tạo API Client (Nếu cần):** Mở `src/api/CultivationApiClient.ts`, định nghĩa Interface hứng dữ liệu từ Backend và viết hàm `fetch()` tương ứng.
2. **Vẽ Giao Diện:** Mở `src/utils/embedBuilder.ts`, tạo hàm `build<TínhNăng>Embed(...)` trả về đối tượng `EmbedBuilder` với thông tin và màu sắc phù hợp.
3. **Xử lý Logic Nút Bấm/Menu:** Mở `src/controllers/cultivateController.ts`. Tìm (hoặc thêm mới) điều kiện if/else trong `interaction.isButton()` hoặc `interaction.isStringSelectMenu()`. 
4. **Khai báo Action ID:** Khi thêm nút, nhớ đặt `customId` theo chuẩn `cultivate_<action>`.
5. **Kiểm tra Types:** Chạy lệnh `npx.cmd tsc --noEmit` để đảm bảo không sai kiểu dữ liệu.

---

## 4. Troubleshooting & Best Practices

- **Lỗi Serialization (JSON):** Hãy chắc chắn các property trong class C# được ánh xạ đúng với Interface TypeScript. Backend .NET thường xuất JSON dạng `camelCase` (mặc định), nên TS interface phải tuân theo `camelCase`.
- **Game Config (game_data.json):** Mọi chỉ số level, phần thưởng boss... đều nằm ở `src/CoreAPI/CultivationApi.WebApi/game_data.json`. Đừng hardcode vào Bot hay code C#. Khi sửa file này, không cần build lại dự án.
- **Bắt buộc Document:** Bất cứ khi nào tạo API mới hay UI flow mới, **phải** cập nhật các file trong `docs/` (`user_flow.md` hoặc `system_architecture.md`).
- **Sanitize Lỗi (Security):** Bot không bao giờ được gửi stacktrace ra kênh chat công khai. Catch tất cả lỗi và in log qua `logger.error()`, chỉ gửi một chuỗi thân thiện cho người dùng trên Discord kèm `TraceId`.

---

## 5. Cẩm Nang Xóa Dữ Liệu Test (Database Cleanup)

Trong quá trình dev, đôi khi bạn cần reset lại dữ liệu của một người dùng (hoặc server) để test lại quy trình Onboard. PostgreSQL sử dụng khóa ngoại (Foreign Key) rất chặt chẽ, vì vậy phải xóa dữ liệu theo thứ tự từ bảng con đến bảng cha.

### Xóa toàn bộ dữ liệu của một User (Reset Onboard)
Giả sử bạn muốn xóa user có Discord ID là `295281134638596096`. Chạy các lệnh SQL sau trong trình quản lý CSDL (ví dụ pgAdmin, DBeaver) hoặc qua psql:

```sql
-- Bước 1: Xóa toàn bộ Inventory liên quan đến Character của user này
DELETE FROM "Inventories" 
WHERE "CharacterId" IN (
    SELECT "Id" FROM "Characters" WHERE "DiscordId" = '295281134638596096'
);

-- Bước 2: Xóa toàn bộ Audit Logs liên quan
DELETE FROM "AuditLogs" 
WHERE "CharacterId" IN (
    SELECT "Id" FROM "Characters" WHERE "DiscordId" = '295281134638596096'
);

-- Bước 3: Xóa Character của user này
DELETE FROM "Characters" 
WHERE "DiscordId" = '295281134638596096';

-- Bước 4: Xóa tài khoản Discord User (Để trigger lại luồng Onboard từ đầu)
DELETE FROM "DiscordUsers" 
WHERE "DiscordId" = '295281134638596096';
```

*(Lưu ý: Tên bảng có thể bọc trong dấu ngoặc kép `"TableName"` để giữ nguyên dạng viết hoa (PascalCase) được tạo bởi EF Core).*
