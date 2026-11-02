# TKS Thực Tập V11 — TKELog WMS

> Hệ thống quản lý kho vận (Warehouse Management System) — bài tập thực tập nhóm đợt 11.
> Xây dựng trên ASP.NET Core Blazor Server, Telerik UI, Telerik Reporting và SQL Server.

![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%206.0-512BD4?logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor&logoColor=white)
![Telerik](https://img.shields.io/badge/Telerik-Reporting%2017.2-5CE500)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC2927?logo=microsoftsqlserver&logoColor=white)
![License](https://img.shields.io/badge/license-Proprietary-red)

---

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Tính năng](#tính-năng)
- [Kiến trúc](#kiến-trúc)
- [Tech stack](#tech-stack)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Setup tự động (1 lệnh)](#setup-tự-động-1-lệnh--khuyến-nghị)
- [Setup thủ công (A-Z)](#setup-thủ-công-a-z)
- [Cấu hình](#cấu-hình)
- [Chạy ứng dụng](#chạy-ứng-dụng)
- [Các tác vụ thường gặp](#các-tác-vụ-thường-gặp)
- [Testing](#testing)
- [Deployment](#deployment)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)
- [Contributing](#contributing)
- [License](#license)
- [Liên hệ](#liên-hệ)

---

## Giới thiệu

**TKELog WMS** là ứng dụng web quản lý kho vận nội bộ, được xây dựng trong khuôn khổ chương trình thực tập nhóm đợt 11. Ứng dụng hỗ trợ các nghiệp vụ quản lý kho cơ bản, xuất báo cáo và thống kê qua dashboard.

**Điểm nổi bật:**

- Kiến trúc **modular feature** — mỗi phân hệ là 1 project riêng biệt
- Giao diện **Blazor Server** — SPA không cần JavaScript phức tạp
- **Telerik UI** cho grid, form, chart chuyên nghiệp
- **Telerik Reporting** cho báo cáo PDF/Excel
- **ADO.NET** thuần — không dùng ORM, kiểm soát SQL hoàn toàn
- Cấu hình **User Secrets** — không lộ credentials lên git

**Trạng thái:** Đang phát triển (WIP) — bài tập thực tập.

---

## Tính năng

### Phân hệ Danh mục (`Web_Danh_Muc`)

- Quản lý danh mục kho, kệ, vị trí
- Quản lý danh mục hàng hoá, đơn vị tính
- Quản lý danh mục nhà cung cấp, khách hàng
- CRUD đầy đủ + tìm kiếm fuzzy (FuzzySharp) + phân trang

### Phân hệ Dashboard (`Web_Dashboard`)

- Thống kê tồn kho realtime
- Biểu đồ nhập/xuất theo thời gian (Telerik Charts)
- KPI: tổng hàng, giá trị tồn, hàng sắp hết
- Export dashboard ra Excel

### Phân hệ Hệ thống (`Web_Sys`)

- Đăng nhập / đăng xuất
- Phân quyền theo role (admin, manager, staff)
- Quản lý người dùng
- Cấu hình tham số hệ thống
- Audit log

### Phân hệ Báo cáo (`Report`)

- Báo cáo tồn kho theo kỳ (Telerik Reporting)
- Báo cáo nhập/xuất
- Export PDF, Excel (EPPlus)
- Template có sẵn, có thể custom qua Telerik Report Designer

### Khác

- Gửi email qua SMTP (thông báo, cảnh báo)
- Import/Export Excel
- In phiếu (Append.Blazor.Printing)

---


### Phụ thuộc giữa các project

| Project | Phụ thuộc vào |
|---------|---------------|
| `Data_Access` | (không — độc lập) |
| `Report` | (không — độc lập) |
| `Web_Common` | (không) |
| `Web_Control` | `Web_Common` |
| `Web_Danh_Muc` | `Web_Control`, `Web_Common`, `Data_Access` |
| `Web_Dashboard` | `Web_Control`, `Web_Common`, `Data_Access` |
| `Web_Sys` | `Web_Control`, `Web_Common`, `Data_Access` |
| `Web` (host) | Tất cả các project trên |

### Nguyên tắc thiết kế

1. **Modular feature** — mỗi phân hệ độc lập, dễ tách/mở rộng
2. **Layered** — tầng UI không gọi trực tiếp tầng data
3. **No ORM** — ADO.NET thuần để kiểm soát query
4. **Shared base** — `Web_Control` chứa base class dùng chung
5. **Config in secrets** — không hardcode credential

---

## Tech stack

| Tầng | Công nghệ | Version |
|------|-----------|---------|
| Runtime Web | ASP.NET Core + Blazor Server | .NET 8.0 |
| Runtime Data/Report | .NET | 6.0 |
| UI Components | Telerik UI for Blazor | 5.0.1 |
| Reporting | Telerik Reporting + ReportViewer.Blazor | 17.2.23 |
| Data Access | Microsoft.Data.SqlClient (ADO.NET) | 6.1.4 |
| Excel | EPPlus | 7.0.5 |
| JSON | Newtonsoft.Json | 13.0.3 |
| Fuzzy search | FuzzySharp | 2.0.2 |
| Printing | Append.Blazor.Printing | 6.2.0 |
| Logging | Serilog (hoặc built-in) | — |

---

## Yêu cầu hệ thống

### Phần cứng tối thiểu

- CPU: 4 core
- RAM: 8 GB (khuyến nghị 16 GB nếu chạy SQL Server cùng máy)
- Disk: 5 GB trống (cho SDK, NuGet cache, DB)

### Phần mềm bắt buộc

| Thành phần | Phiên bản | Ghi chú |
|-----------|-----------|---------|
| Windows | 10/11 | Chỉ hỗ trợ Windows (Telerik) |
| .NET SDK | 8.0 LTS | Bắt buộc |
| .NET Runtime | 6.0.x | Bắt buộc (cho Data_Access, Report) |
| SQL Server | 2019+ / LocalDB | Bắt buộc |
| Visual Studio | 2022 17.8+ | Khuyến nghị (hoặc VS Code) |
| Telerik Account | Trial / Commercial | Bắt buộc (nuget.telerik.com) |

### Verify trước khi setup

Mở PowerShell, chạy:

```powershell
# .NET
dotnet --version
# Kỳ vọng: 8.x.x

dotnet --list-runtimes
# Kỳ vọng thấy: Microsoft.NETCore.App 6.0.x và 8.0.x

# SQL Server
sqllocaldb info
# Kỳ vọng: có ít nhất 1 instance (MSSQLLocalDB)

# Hoặc nếu dùng SQL Server full:
Get-Service MSSQL*


