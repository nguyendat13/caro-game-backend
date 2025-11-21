# Gami Caro Server (gami-caro-server)

**Nền tảng game tổng hợp các trò chơi-Gami** – Hệ thống backend đầy đủ cho game Caro online đa nền tảng với tính năng chuyên nghiệp: realtime multiplayer, bảng xếp hạng, cộng đồng, giải đấu, chat, sự kiện...

### Tính năng chính
- Realtime multiplayer Caro (Socket.IO)
- Bảng xếp hạng toàn server (Top 100 + Mini Leaderboard)
- Hệ thống giải đấu chuyên nghiệp (tạo, đăng ký, bracket tự động)
- Cộng đồng + Chat realtime + Tạo kênh riêng
- Đăng bài viết, hướng dẫn, tuyển clan, thông báo lớn
- Hệ thống sự kiện đa dạng (tournament, livestream, update, recruit...)
- Authentication JWT + Refresh Token
- Role-based access (Admin, Moderator, User)
- API sạch, chuẩn REST + WebSocket
- Database SQL (SQL Server / PostgreSQL / MySQL)

### Công nghệ sử dụng
- .NET 8.0 (C#)
- ASP.NET Core Web API
- Entity Framework Core
- SignalR (Realtime)
- JWT Authentication
- AutoMapper
- Swagger / OpenAPI
- SQL Server (có thể đổi sang PostgreSQL/MySQL)

### Cấu trúc thư mục
gami-caro-server/
├── Controllers/          # API Controllers
├── Services/             # Business logic
├── Hubs/                 # SignalR Hubs (GameHub, ChatHub)
├── Models/               # Entity models
├── DTOs/                 # Data Transfer Objects
├── Data/                 # DbContext + Migrations
├── Utils/                # Helpers, extensions
├── wwwroot/              # Static files (nếu có)
├── Program.cs
└── appsettings.json


### Các API chính
| Route | Method | Mô tả |
|------|--------|------|
| `/api/Auth/login` | POST | Đăng nhập |
| `/api/Auth/register` | POST | Đăng ký |
| `/api/Game/start` | POST | Bắt đầu ván mới |
| `/api/Game/move` | POST | Đánh nước đi (realtime) |
| `/api/GameStats/leaderboard/{gameType}` | GET | Lấy bảng xếp hạng |
| `/api/GameStats/recent/{userId}` | GET | Lịch sử trận gần đây |
| `/api/Tournament/create` | POST | Tạo giải đấu |
| `/api/Events` | GET | Danh sách sự kiện |
| `/api/Chat` | WebSocket | Chat realtime |

### Hướng dẫn chạy dự án

## 1. Clone repo
git clone https://github.com/yourusername/gami-caro-server.git
cd gami-caro-server

## 2.Cài đặt dependencies
dotnet restore

## 3.Cấu hình Connection String trong appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GamiCaroDB;Trusted_Connection=true;TrustServerCertificate=true;"
}

## 4.Chạy migrations
dotnet ef database update

## 5.Chạy server
dotnet run

## Liên hệ

Nếu bạn có bất kỳ thắc mắc hoặc góp ý nào, vui lòng tạo issue hoặc liên hệ trực tiếp qua:

Email: dat48421@gmail.com

GitHub: nguyendat13
