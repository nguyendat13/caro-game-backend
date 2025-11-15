# 🎮 Caro Game Server

Backend của **Caro Game Online/Offline**, xây dựng bằng **C# .NET 8**, sử dụng **Entity Framework Core** cho database, **SignalR** cho realtime gameplay, và **JWT** cho bảo mật.

---

## 🚀 Giới thiệu
Caro (Gomoku) là trò chơi dân gian được số hóa:
- Chơi offline hoặc online với người khác
- Hệ thống tài khoản, xếp hạng, lịch sử trận đấu
- Quản lý Role, phân quyền, bảo mật OTP
- API RESTful dễ tích hợp frontend (React, Vue, Next.js)

---

## 🛠 Công nghệ
- **Ngôn ngữ:** C# (.NET 8)
- **Database:** SQL Server + EF Core
- **Realtime:** SignalR
- **Bảo mật:** JWT, Hash password, OTP
- **Kiến trúc:** RESTful API, Service + Controller

---

## 📂 Cấu trúc dự án
backend/
├── Controllers/ # Xử lý API
├── DTOs/ # Data Transfer Objects
├── Models/ # Entity + DbContext
├── Services/ # Business logic + Interface
├── Migrations/ # EF Core migrations
├── Program.cs
└── backend.csproj

---

## ⚙️ Cài đặt

```bash
git clone https://github.com/nguyendat13/caro-backend.git
cd backend
dotnet ef database update
dotnet run

## Liên hệ

Nếu bạn có bất kỳ thắc mắc hoặc góp ý nào, vui lòng tạo issue hoặc liên hệ trực tiếp qua:

Email: dat48421@gmail.com

GitHub: nguyendat13