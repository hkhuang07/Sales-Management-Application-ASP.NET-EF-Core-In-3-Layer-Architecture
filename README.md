# ⚡ Electronics Store Management System (.NET 8, WinForms, EF Core)

> 🚀 **Transforming retail management for the digital era!**  
> Say goodbye to manual tracking and hello to a seamless, modern inventory and sales system tailored for electronic stores. Built on robust technologies like .NET 8, WinForms, and EF Core — this project sets the standard for desktop retail management solutions.

---

## 📥 Getting Started

To initialize the database and get the application running locally, follow these steps:

1. **Restore all dependencies via NuGet**:
   ```bash
   MSBuild /t:restore
   ```
Apply Entity Framework Core Migrations:

```bash
Copy code
Add-Migration KhoiTaoCSDL
Update-Database
```
If you are using a factory pattern (recommended), make sure your ElectronicsStoreContextFactory is implemented correctly to allow design-time DBContext creation.

📌 Project Overview
This project is a complete 3-layer (Three-Tier) architecture desktop application that allows a local electronics store to manage their:

🔧 Products

📂 Categories

🏭 Manufacturers

👥 Customers

👨‍💼 Employees

🧾 Orders & Order Details

📊 Revenue & Product Statistics

It also provides user authentication, role-based access, and report generation using RDLC ReportViewer — making it an all-in-one retail store management software.

🏗️ Technology Stack
Layer	Technology
Presentation (UI)	.NET 8 WinForms
Business Logic (BLL)	C# Services with AutoMapper, DTOs
Data Access (DAL)	Entity Framework Core 8, LINQ, SQL Server
Reporting	ReportViewerCore.WinForms, ClosedXML
Dependency Injection	Microsoft.Extensions.DependencyInjection
Database	SQL Server 2022
ORM	Entity Framework Core 8 (Code-First with Migrations)

📂 Features
✅ Authentication & Roles
Login system with secure password hashing (using BCrypt)

Role-based access: Admin & Employee

🗃 Product Management
Add, update, delete, search products

Display product image

Filter by category or manufacturer

🧾 Order Management
Create, update, delete orders

Manage order details (items, quantity, unit price)

Automatically calculate total price

Reduce stock after successful order

📁 Category & Manufacturer Management
Add/edit/delete categories & manufacturers

Bind dropdowns to relevant forms

👨‍💼 Employee Management (Admin only)
Add new employees

Assign roles

Update password

👥 Customer Management
Track customer info

View customer order history

📊 Reporting & Statistics
Generate revenue reports by date range

Export invoices to PDF

Export Excel product or order lists

View top-selling items by category/manufacturer

🖥️ UI Screens
Form	Description
Main Form	Navigation with menus, tabs, and sidebar
Login Form	Login with auto-focus, enter-to-submit
Category/Product/Customer Forms	CRUD operations with DataGridView
Order Form	View list of orders, filter, export to PDF
Order Detail Form	Add/edit product lines to an order
Confirm Form	Collect customer & employee info
Sale Form	Main POS interface (add to cart, remove, confirm order)
Reports	Product statistics and revenue reports
About Box	Software info: version, author, licensing

🧱 Database Design
📑 Entity Overview
Products: ID, Name, Price, Quantity, Description, Image, CategoryID, ManufacturerID

Categories: ID, Name

Manufacturers: ID, Name, Address, Phone, Email

Employees: ID, FullName, Phone, Address, Role, Username, Password

Customers: ID, Name, Address, Phone, Email

Orders: ID, EmployeeID, CustomerID, Date, Note

Order_Details: ID, OrderID, ProductID, Quantity, Price

🔗 Relationships
One Category ↔ Many Products

One Manufacturer ↔ Many Products

One Employee ↔ Many Orders

One Customer ↔ Many Orders

One Order ↔ Many Order_Details

One Product ↔ Many Order_Details

🧠 Software Architecture
3-Layer Architecture
Presentation Layer (UI)

Forms built using WinForms

Handles user interaction and displays data from DTOs

Business Logic Layer (BLL)

Validates business rules

Uses AutoMapper to map between Entity & DTO

Interfaces with Repository Layer

Data Access Layer (DAL)

Repository pattern using EF Core

Handles all DB interactions

💡 Design Highlights
AutoMapper for DTO ↔ Entity mapping

EF Core Migrations with ElectronicsStoreContextFactory for CLI database management

SlugGenerator for readable identifiers

ClosedXML for Excel exporting

ReportViewer for advanced reporting

🚧 Future Improvements
🌐 Build ASP.NET Core WebAPI or Blazor frontend

📱 Add Xamarin/.NET MAUI mobile client

📦 Barcode/QR integration for fast checkout

📤 Email confirmation for orders

🔒 Logging and audit trails

📈 Revenue analytics & smart product suggestions

🔗 Real-time multi-user environment (Client-Server or SignalR)

👨‍💻 Developer Info
Author: Huỳnh Quốc Huy
Project ID: DTH225650
University: An Giang University – Faculty of Information Technology
Mentors: Nguyễn Hoàng Tùng
Date: May 2025

❤️ Final Thoughts
This project represents the culmination of a deep dive into modern desktop application development. It demonstrates a strong understanding of scalable software design, real-world database management, and clean code architecture — all while delivering a practical tool that can genuinely assist electronic store operations.

We hope this software inspires future improvements, integrations, and even full-scale product deployment in small-to-medium retail businesses.

💬 Feedback, suggestions or collaboration ideas are warmly welcome!
