# INTRODUCTION
## INTRODUCTION TO THE TOPIC
### Problem statement
In the current context, information technology is growing strongly and playing an important role in most areas of life, including the fields of commerce and sales management. In particular, for retail stores such as electronics stores, the need to computerize management processes is an inevitable requirement to improve business efficiency, save costs, minimize errors and increase competitiveness in the market.
In fact, many small and medium-sized stores today still manage business activities through manual methods such as recording in books or using common software such as Excel. These methods often pose many risks such as data loss, data errors, difficulties in retrieving information, and failure to ensure consistency in warehouse, order, customer and revenue management. This leads to a decrease in labor productivity and directly affects business efficiency.
Based on that practical need, the topic "Electronics store management software" was carried out to build a comprehensive management support software system for stores selling electronic products such as: phones, iPads, computers, laptops, RAM, hard drives, and other electronic components,... The software is designed according to the three-layer model (3-Layer Architecture), combined with modern technologies and software development support tools to create a highly applicable solution, easy to use, easy to maintain and expand in the future.
The main objective of the topic is to build a user-friendly management software, with all the essential functions such as: product management, product category management, employee management, customers, orders, order details, revenue statistics, ... and at the same time support printing reports through the ReportViewer tool. Thereby, the software not only helps employees and store owners perform their work quickly and accurately but also helps improve the overall business performance of the store.
### Scope of the topic
The topic "Electronics store management software" focuses on developing and perfecting an application management software system in the WinForms environment with basic and popular functions in the electronic sales management model. Specifically, the scope of the topic includes:
- Product management: Includes functions to add, edit, delete, search and display product lists with detailed information such as product codes, product names, descriptions, prices, inventory quantities, images, and product categories.
- Product category management: Allows products to be classified into specific categories for convenient management and searching.
- Employee management: Stores and manages information of employees working at the store.
- Customer management: Stores customer information to serve the purpose of invoicing and managing purchase history.
- Order and order detail management: Supports creating sales invoices, calculating totals, tracking order status and products sold.
- Statistics - reporting: Perform statistics on revenue and products sold on invoices using the ReportViewer tool.
Data storage using SQL Server: All data is stored and processed through the SQL Server database management system, ensuring the integrity and security of information.
3-Layer Architecture: The software is organized into three layers including Presentation Layer (User Interface), Business Logic Layer (Business Processing), and Data Access Layer (Data Access), and the Data Transfer Object data transport support layer.
The topic does not go into detail about advanced features such as online sales integration, e-wallet payment, product barcodes or Web/Mobile technologies. However, the software is built on a scalable platform, and it is completely possible to integrate these functions in the future.
### Tools used
During the software design and development process, Huynh Quoc Huy student uses a number of tools, programming languages ​​and supporting libraries as follows:
- Visual Studio 2022: Is the main integrated development environment (IDE) used to build user interfaces, write program code and manage the entire project.
- .NET 8.0 WinForms: The main platform for building Windows Forms applications with a friendly graphical interface, supporting mouse and keyboard operations.
- SQL Server 2022: A relational database management system used to store, query and process data effectively.
- Entity Framework Core 8 (Microsoft.EntityFrameworkCore): Is an ORM (Object-Relational Mapping) used to create data models, map between tables in the database and object classes in the program, support data queries through LINQ.
- ReportViewerCore.WinForms: Report visualization tool for WinForms, helps create and export reports such as invoices, revenue, product lists.
- ClosedXML: Open source library supporting Excel file manipulation (XLSX), helps export report data or spreadsheets to Excel format easily.
- Microsoft.Extensions.DependencyInjection.Abstractions: Supports Dependency Injection (DI) techniques to improve extensibility, testability and code reuse.
- SlugGenerator: A library that supports converting product/category names into readable and URL-friendly “slugs” or identifiers.
The combination of these modern tools and libraries not only makes the software development process more professional and organized, but also ensures that the software can operate stably, easily maintained and upgraded later.
## THEORETICAL BASIS
### .NET
#### Introduction to the .NET platform
.NET is a software development platform developed by Microsoft, supporting the construction of many types of applications from desktop, web to mobile and distributed systems. This platform consists of two main branches: .NET Framework (launched in 2002) and .NET Core (launched in 2016, and merged into .NET 5+ in 2020 and now .NET 8).
Of which, .NET Framework is the version that is closely tied to the Windows operating system, while .NET Core/.NET 5+ is an open source, cross-platform platform that supports Windows, Linux and macOS, redesigned with high performance, lightweight, flexible and more suitable for modern architectures.
Moving from the old WinForms .NET Framework to WinForms .NET Core/.NET 8 brings many significant benefits in modern software development, especially when applying the multi-layer model (3-layer architecture).
1.2.1.2. Why use WinForms .NET Core instead of WinForms .NET Framework?
In this project, the author Huynh Quoc Huy chose WinForms .NET 8 (belonging to the .NET Core branch) instead of WinForms on the traditional .NET Framework, for the following important reasons:
High performance and better optimization: .NET Core (and especially from .NET 5 and above) is re-architected with a higher performance runtime, fast startup time, better memory management, suitable for software that requires a lot of data processing such as store management software.
Better support for Entity Framework Core: EF Core is a new generation ORM optimized for .NET Core, providing powerful LINQ, supporting async, caching, tracking... If using .NET Framework, you can only use EF6, which is no longer strongly developed. EF Core is fully compatible and easy to integrate with WinForms .NET Core.
Lightweight and flexible architecture: WinForms .NET Core allows for the separation of necessary components (runtime trimming, single-file deployment), helping to package the software more lightweight, easy to deploy without additional installation on the client.
Default Dependency Injection (DI) support: The ability to integrate DI helps build a tighter 3-layer architecture, making it easier to test business logic layers, separate dependencies, and follow the SOLID principle.
Especially the long-term development future: Microsoft has oriented to move the entire ecosystem to the new .NET branch (starting from .NET 5), so new features, tools and libraries are prioritized for support on .NET Core/.NET 8+. Choosing WinForms .NET Core helps ensure the ability to upgrade and expand the software in the future.
Future expansion: If you want to integrate Web API, Blazor, or switch to Web or cross-platform, .NET Core supports much better than .NET Framework.

Therefore, using WinForms .NET 8 instead of the old .NET Framework was a strategic choice right from the beginning of the project, even though it was different from the old construction direction of the old lesson plan, helping to take advantage of new features, increase performance and ensure that the software can adapt well to future technology trends.
### Entity Framework Core
#### Introducing Entity Framework Core
Entity Framework Core (EF Core) is a modern ORM (Object-Relational Mapping) library developed by Microsoft, allowing programmers to manipulate databases through C# objects instead of writing pure SQL statements directly. EF Core is a completely revamped version of the classic Entity Framework, designed specifically for .NET Core and .NET 5+.

EF Core acts as an intermediate layer between the business logic layer and the database management system (SQL Server, MySQL, PostgreSQL, etc.). EF Core supports:

- Mapping between data tables and object classes (entities).

- Creating databases from models (Code First) or creating models from databases (Database First).

- Querying data using LINQ (Language Integrated Query).
Transactions, connections, and object-oriented data control.

#### Advantages of EF Core in 3-Layer Development
EF Core is an ideal tool when applying the 3-Layer Architecture model for the following reasons:
Clear separation between layers: EF Core operates at the data access layer (DAL), can integrate directly with repositories, helping to clearly separate business logic and interface.
Code First supports design from the model: Easily create tables from classes or use Migration to update the database structure, making it easy to maintain.
Good Dependency Injection support: Allows injecting DbContext through constructors, easy to configure in .NET Core's DI Container.
Powerful querying with LINQ: Allows writing data queries like pure C#, avoiding SQL syntax errors, increasing productivity and reducing errors.
High extensibility: Can add libraries like AutoMapper to convert between Entity and DTO, serving the ness Logic and Presentation Layer.
Change Tracking support: Easily manage entity status (Add, Edit, Delete), helping to simplify data operations.
Well integrated with ReportViewer and report export tools: Can easily export data from DbContext to dataset or collection for report export via RDLC or ClosedXML supporting Excel export.
Thanks to EF Core, building multi-layer models becomes neat, easy to maintain, easy to test and highly scalable. This is the core component that helps the software achieve modernity, flexibility and optimization in data management.
### Microsoft SQL Server
#### Overview
Microsoft SQL Server is a powerful relational database management system developed by Microsoft. SQL Server provides a stable, secure platform and is capable of handling large amounts of data, widely used in many software systems from small to large.
SQL Server provides intuitive database management tools such as SQL Server Management Studio (SSMS), supports T-SQL language for querying and manipulating data, along with many powerful features such as backup/restore, trigger, stored procedure, and integrates well with other Microsoft products.
1.2.3.2. Project role
In this project, SQL Server is used as a platform to store and manage all software data. Specifically:
- Store information about products, orders, customers, employees, categories, invoices, ...
- Tightly integrated with Entity Framework Core: makes it easy to create tables, map objects and process data with code.
- High stability and security, ensuring that data is not lost or distorted during use.
- Good scalability, can upgrade data scale to millions of records without affecting performance if properly optimized.
# SYSTEM ANALYSIS AND DESIGN
## USE CASE DIAGRAM
### General description of Use Case diagram
The Use Case diagram shows the functions that the system needs to provide to users through actors. In this e-store management system, there are two main actors:
Administrator (Admin): the person who has full access and management of all components in the system.
Employee (Employee/Member): the person who uses the system but is limited to certain rights (cannot manage employees, cannot view reports).
Each agent will interact with different functions:
- Category management
- Manufacturer management
- Customer management
- Employee management (Admin only)
- Product management
- Order management and order details
- View reports (Admin only)
### Main Use Case details

- Use Case: Product category management
o Agent: Admin, Employee
o Description: Add, edit, delete product types
- Use Case: Product management
o Agent: Admin, Employee
o Description: Allows adding, editing, deleting, searching and viewing product information.
- Use Case: Order management
o Agent: Admin, Employee
o Description: Create new orders, update, view order details, calculate total amount.
- Use Case: Employee Management
o Actor: Admin Only
o Description: Add, edit, delete employees, assign roles.

- Use Case: Manufacturer Management
o Actor: Admin, Employee
o Description: Add, edit, delete manufacturers.
- Use Case: View reports
o Actor: Admin Only
o Description: Product statistics, revenue by day/month.
- Use Case: Login
o Actor: Admin, Employee
o Description: Authenticate users to the system, assign permissions after logging in.

## CLASS DIAGRAM
### Object-Oriented Class Design

The system is designed in an object-oriented manner with main classes corresponding to tables in the database. Each class represents an object and contains corresponding properties and methods.
More specifically, with the project under construction, Huynh Quoc Huy designed the class as follows:
- Manufacturers:
o Properties: ID, ManufacturerName, ManufacturerAddress, ManufacturerPhone, ManufacturerEmail
o Methods: Add(), Update(), Delete(), GetAll(), FindById(), GetAll().
- Categories:
o Properties: ID, CategoryName.
o Methods: Add(), Update(), Delete(), GetAll(), FindById(), GetAll().
- Products
o Properties: ID, ProductName, Price, Quantity, Image, Description, CategoryID, ManufacturerID
o Methods: Add(), Update(), Delete(), GetAll(), FindById(), GetAll().
- Employees:
o Properties: ID, FullName, EmployeePhone, EmployeeAddress, UserName, Password, Role.
o Methods: Add(), Update(), Delete(), GetAll(), FindById(), GetAll().
o
- Orders
o Properties: ID, EmployeeID, CustomerID, Date, Note
o Methods: CreateOrder(), UpdateOrder(), GetOrderDetails(), GetAll().

- Order Details:
o Properties: ID, OrderID, ProductID, Quantity, Price
o Methods: Insert(), AddRange(), Update(), GetAll().

### Relationships between classes
The classes will have the following relationships:
- The Order class has a 1-n relationship with Order_Details
- The Product class has an n-1 relationship with Manufacturer and Category
- The Employee class manages orders (1-n)
- The Customer class is the person who places the order (1-n)
The classes can use the DTO (Data Transfer Object) model to exchange data between layers (UI - BLL - DAL)
## DIAGRAM
  DATABASE RELATIONSHIPS

### Description of the ER (Entity Relationship) diagram
The relationship diagram clearly shows how the data tables are linked together through primary keys and foreign keys:
Product is linked to Manufacturer and Category through ManufacturerID and CategoryID.
Order_Details is a sub-table between Order and Product, with foreign keys OrderID and ProductID.
Order is linked to Customer and Employee through CustomerID and EmployeeID.

### Relationship design rules
The relationship design rules are established as follows:
- All relationships are 1-n or n-1 relationships.
- All tables have an auto-incrementing primary key (ID).
- Data is normalized to avoid duplicate information.
- Use foreign keys to ensure data integrity (referential integrity).

## DESCRIPTION OF TABLES IN THE DATABASE
### Categories table
ID: primary key, category identifier.
CategoryName: category name (eg: Laptop, Phone…).
### Manufacturers table
ID: primary key.
ManufacturerName: manufacturer name.
ManufacturerAddress, ManufacturerPhone, ManufacturerEmail: contact information.
### Customers table
ID: primary key.
CustomerName, CustomerAddress, CustomerPhone, CustomerEmail: customer information.
### Employees table
ID: primary key.
FullName, EmployeePhone, EmployeeAddress: personal information.
UserName, Password, Role: login information and authorization (Admin / Employee).
### Products table
ID: primary key.
ManufacturerID: foreign key to Manufacturers.
CategoryID: foreign key to Categories.
ProductName, Price, Quantity, Image, Description: product information.
### Orders table
ID: primary key.
EmployeeID, CustomerID: foreign keys to Employees and Customers.

Date, Note: order date and note.

### Order_Details table

ID: primary key.

OrderID, ProductID: foreign key.

Quantity, Price: quantity and price at the time of order.

# DESIGN

## INTERFACE DESIGN

### Main Form

The main form is designed with 3 main option areas: menu, option tabs, and siderbar. These 3 areas perform similar functions. The options will include:

- System: Log in, Log out, Change password, Data: Backup, Restore, Exit.
- Order: form Sale performs the function of ordering products

- Management: Manage tables: categories, manufactures, products, employees, customers, orders…

- Report-Statistics: product statistics, revenue statistics.

- Help: Software information, open web help…
### Form Login

Form Login supports the feature of saving information, enter in the Password textbox to log in to help improve user experience. Button Log in to log in, cancel to close the form.

### Form Flash

The form displayed before the user enters the main interface of the software supports displaying Processbar and symbolic images

### Form Aboutbox

The form displays information about the software configured in the project. Displays information such as software name, copyright, version…
### Form Categories

The management form of the Categories table supports the functions of adding, editing, and deleting tables recorded in the form.

- Button Add to clear fields, mark as added.
- Button Update to mark as edited and get the ID of the current record.
- Button Save, depending on the Add or Update option, will process Add or Update data.
- Button Cancel to cancel actions by reloading the page. Button Close to close the form.
In addition, the form also supports the navigation bar feature bound to datagridView and controls, here you can search for records, or import/export data in excel format.
### Form Manufactures

Form Manufactures also has the same functions as form Categories.
### Form Customers

Form Customers also has the same functions as form Categories.
### Form Employees

Form Employee also has the same functions as form Categories.
### Form Products

Form Product also has the same functions as form Categories. Supported binding and displaying product images for more intuitive management.
### Form Orders

Form Orders contains a navigation bar binding to datagridview and adds the function of exporting and searching for Orders.
Next to it is the Create order button to open the Order Details form below. The Print order button to export the invoice in PDF format at the selected line on the datagridView.
- Update button to open the Order Details form to update if the customer changes the previous invoice.
- Delete button to delete the invoice, Close button to close the form.
In addition, when double-clicking on the Details column at a certain line, the OrderDetails form of that invoice will be displayed, which also aims to improve the user experience.
### Form Order Detais

This form is displayed when the user clicks Create order at the order form or clicks to update the selected order as well as double-clicks on the selected line.
This form allows users to edit invoice details. Button Confirm to add the selected product on the combobox to the invoice details list. Delete to delete that detail from the invoice.
Save Order to update the old invoice or add a new invoice depending on the options in the Order form. Close to close the form

### Form Print Order

This form y is displayed when the user selects Print order in the Order form to issue an invoice to the customer at the selected line on the datagridview. It is similar when the user selects Print in the Sale form (sales service).
The form supports issuing invoices according to the PDF report design.
### Sale Form

This form performs the main function of the software, which is sales. The form has a grid displaying products.
Add search features. Add to card button to add products to the invoice details list.
Delete button to reduce the quantity of products. Or delete from the details list if the quantity <= 0.
Add/subtract button on UserControl to increase or decrease the quantity or delete products from the invoice details list.

Order button performs the function of adding a new order. First, the form will open the Confimr form below to get employee and user information to create an invoice. When the Confirm form is closed, a new invoice will be created for the customer from the Confirm form, then the invoice will be issued according to the user's preferences.

### Confirm Form

This form is intended to get information to create an order.

- Add button to mark Add if the customer is new, clear all fields.

- Update button to update if the customer is old. Confirm to confirm and close the form to return to the Sale form to complete the order. Here the Print Order form will be displayed if the user checks Print Invoice.

### Product Statistic Form

The form lists products that are filtered by manufacture by category. If the two comboboxes have blank fields, load all

### Revenue Statistic Form

The revenue statistics form is filtered by start date and end date

By default, it will load all time.

### Change Password Form

The form has the function of helping users change their passwords.

## DATA DESIGN
### Three-layer Architecture
In the sales management system, separating the source code according to the 3-layer model makes the software easy to maintain, expand and test. The layers are divided as follows:
Presentation Layer (User Interface): This is where users interact with the system, including Forms and Controls in WinForms. Data is displayed from the intermediate layer as DTO and sent through Service methods.
Business Logic Layer (BLL - Business processing): Contains classes/services that handle business logic such as checking data validity, calling the repository to get data from the DB, mapping back and forth between Entity and DTO. This layer uses AutoMapper to convert between layers.
Data Access Layer (DAL - Data Access): Contains classes that interact directly with the database through Entity Framework Core. Each entity corresponds to a table in the database.
### Entity Model in Entity Framework Core
#### Context
 ElectronicsStoreContext class
This class represents the central class that communicates with the database, manages entities, and maps entities <-> tables.
When EF Core executes the Add-Migration command, it will read information from DbSet to generate the migration file.
When performing Update-Database, EF Core will use ElectronicsStoreContext to connect to the database and execute the corresponding SQL from the created migration.
public class ElectronicsStoreContext : DbContext
{
public DbSet<Categories> Category { get; set; }
public DbSet<Manufacturers> Manufacturer { get; set; }
public DbSet<Products> Product { get; set; }
public DbSet<Employees> Employee { get; set; }
public DbSet<Customers> Customer { get; set; } 
public DbSet<Orders> Order { get; set; set; } 
public DbSet<Order_Details> Order_Details { get; set; set; } 

public ElectronicsStoreContext() { } 

public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options) 
:base(options) 
{ 
} 

// If you want to keep OnConfiguring for actual running 
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
{ 
if (!optionsBuilder.IsConfigured) 
{ 
optionsBuilder.UseSqlServer("Data Source=.;Database=ElectronsStore;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"); 
}
}
}

Database initialization command using Entity Framework Core Migration:
Add-Migration CreateDatabase
Update-Database
Add-Migration KhoiTaoCSDL: create migration file from defined entities.
Update-Database: apply migration, create actual database. This allows to build database in sync with code, easy to maintain, support rollback when needed.
 ElectronicsStoreContextFactory class
This class implements the IDesignTimeDbContextFactory interface, meaning it is called automatically by EF Core at "design-time", such as when you run the command: Add-Migration, then run the command: Update-Database
Purpose: Help EF Core know how to create a DbContext object if the main program cannot provide a DbContext object.
This is especially important when you run migrations in a WinForms or Console app, as they don't use DI like ASP.NET Core does.

public class ElectronicsStoreContextFactory : IDesignTimeDbContextFactory<ElectronicsStoreContext>
{
public ElectronicsStoreContext CreateDbContext(string[] args)
{
var optionsBuilder = new DbContextOptionsBuilder<ElectronicsStoreContext>();

optionsBuilder.UseSqlServer(
"Data Source=.;Database=ElectronsStore;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
);

return new ElectronicsStoreContext(optionsBuilder.Options);
}
}

#### Categories
Entity description: Categories class represents Categories table in database.
Main attributes:
 ID: primary key, auto-incrementing.
 CategoryName: category name.
 Products: list of related products (1-to-many relationship).
Relationship: A Category has many Products (1 - N).
Repository and handling The ICategoryRepository interface defines the operations: GetAll, GetById, Add, Update, Delete. The CategoryRepository class implements these operations, interacting with ElectronicsStoreContext.

public class Categories
{ 
public int ID { get; set; set; } 
public string CategoryName { get; set; set; } = string.Empty; 
public virtual ObservableCollectionListSource<Products> Products { get; } = new();
}

public class CategoryRepository : ICategoryRepository
{ 
private readonly ElectronicsStoreContext _context; 

public CategoryRepository() 
{ 
_context = new ElectronicsStoreContext(); 
} 
//Other methods of the class…
} 

List<Categories> GetAll(); 
Categories? GetById(int id); 
void Add(Categories category); 
void Update(Categories category);
void Delete(Categories category);

#### Manufactueres
Entity description: Manufacturers class represents Manufacturers table in database.
Main attributes:
 ID: primary key, auto-incrementing.
 ManufacturerName: manufacturer name.
 ManufacturerAddress: manufacturer address (optional).
 ManufacturerPhone: manufacturer phone number (optional).
 ManufacturerEmail: manufacturer email (optional).
 Product: list of products provided by this manufacturer (1 - N relationship).
Relationship: a Manufacturer can have many Products (1 - N).

Repository and processing. The IManufacturerRepository interface defines the following operations: GetAll(), GetById(int id), Add(Manufacturers manufacturer), Update(Manufacturers manufacturer), Delete(Manufacturers manufacturer). The ManufacturerRepository class implements the above methods, performing query, add, edit, and delete operations from the database through ElectronicsStoreContext.

public class Manufacturers
{ 
public int ID { get; set; set; } 
public string ManufacturerName { get; set; set; } 
public string? ManufacturerAddress { get; set; set; } 
public string? ManufacturerPhone { get; set; set; } 
public string? ManufacturerEmail { get; set; set; } 
public virtual ObservableCollectionListSource<Products> Product { get; } = new();

public class ManufacturerRepository: IManufacturerRepository 
{ 
private readonly ElectronicsStoreContext _context; 

public ManufacturerRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
//Other methods of the class…
}
public interface IManufacturerRepository
{
List<Manufacturers> GetAll();
Manufacturers? GetById(int id);
void Add(Manufacturers category);
void Update(Manufacturers category);
void Delete(Manufacturers manufacturer);

}

#### Customers
Entity description: The Customers class represents the Customers table in the database.
Main attributes:
 ID: primary key, auto-incrementing.
 CustomerName: customer name.
 CustomerAddress, CustomerPhone, CustomerEmail: contact information.
Order: list of related orders (1-to-many relationship).
Relationship: A Customer can have many Orders (1 - N). Repository and processing: ICustomerRepository defines the operations: GetAll, GetById, Add, Update, Delete. CustomerRepository implements the above operations and interacts with ElectronicsStoreContext.


public class Customers 
{ 
public int ID { get; set; set; } 
public string CustomerName { get; set; set; } 
public string? CustomerAddress { get; set; set; } 
public string? CustomerPhone { get; set; set; } 
public string? CustomerEmail { get; set; set; } 
public virtual ObservableCollectionListSource<Orders> Order { get; } = new();
}
public class CustomerRepository : ICustomerRepository
{ 
private readonly ElectronicsStoreContext _context; 

public CustomerRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
//Other methods of the class…
}
public interface ICustomerRepository
{ 
List<Customers> GetAll(); 
Customers? GetById(int id); 
void Add(Customers customer); 
void Update(Customers customer);
void Delete(Customers customer);
}

#### Employees
Entity description: Employees class represents the Employees table in the database.
Main attributes:
 ID: primary key, auto-incrementing.
 FullName, EmployeePhone, EmployeeAddress: personal information.
 UserName, Password, Role: login account and permissions.
Order: list of processed orders (1-to-many relationship).
Relationship: An Employee can process many Orders (1 - N). Repository and processing: IEmployeeRepository definition: GetAll,GetById, GetByUserName, Add, Update, UpdatePassword, Delete. EmployeeRepository implements these operations, working with ElectronicsStoreContext. 

public class Employees 
{ 
public int ID { get; set; set; } 
public string FullName { get; set; set; } 
public string? EmployeePhone { get; set; set; } 
public string? EmployeeAddress { get; set; set; } 
public string UserName { get; set; set; } 
public string Password { get; set; set; } 
public bool Role { get; set; set; } 
public virtual ObservableCollectionListSource<Orders> Order { get; } = new(); 
}
public class EmployeeRepository: IEmployeeRepository 
{ 
private readonly ElectronicsStoreContext _context; 

public EmployeeRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
}
public interface IEmployeeRepository
{
List<Employees> GetAll();
Employees? GetById(int id);
Employees? GetbyUserName(string userName);

void Add(Employees employee);
void Update(Employees employee);
void UpdatePassword(int id, string hashedPassword);
void Delete(Employees employee);
}

#### Products
Entity description: Products class represents Products table in database.
Main attributes:
 ID: primary key, auto-incrementing.
 ManufacturerID, CategoryID: link to manufacturer and category.
 ProductName, Price, Quantity, Image, Description: product information.
Relationship: Many products belong to one Category (N - 1). Many products belong to one Manufacturer (N - 1). A product can have many Order_Details (1 - N).
Repository and handling: IProductRepository defines: GetAll, GetById, Add, Update, UpdateImage, Delete, GetAllWithCategoryManufacturer. ProductRepository implements the above operations through ElectronicsStoreContext.

public class Products 
{ 
public int ID { get; set; set; } 
public int ManufacturerID { get; set; set; } 
public int CategoryID { get; set; set; } 
public string ProductName { get; set; set; } 
public int Price { get; set; set; } 
public int Quantity { get; set; set; } 
public string? Image { get; set; set; } 
public string? Description { get; set; set; } 
public virtual ObservableCollectionListSource<Order_Details> Order_Details { get; } = new(); 
public virtual Categories Category { get; set; set; } = null!; 
public virtual Manufacturers Manufacturer { get; set; set; } = null!; 
}
public class ProductRepository : IProductRepository
{ 
private readonly ElectronicsStoreContext _context; 
public ProductRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
}
public interface IProductRepository
{ 
List<Products> GetAll(); 
Products? GetById(int id); 
void Add(Products product); 
void Update(Products product); 
void UpdateImage(int productId, string imageFileName); 

void Delete(Products product); 
List<Products> GetAllWithCategoryManufacturer();

}

3.2.2.7. Orders
Entity description: the Orders class represents the Orders table in the database.
Main Attributes:
 ID: primary key, auto-increment.
 EmployeeID, CustomerID: links to employees and customers.
 Date, Note: order date and order notes.
 ViewDetails: list of invoice details (1 - N).
Relationship: An Order belongs to a Customer (N - 1). An Order belongs to an Employee (N - 1).
Repository and processing: IOrderRepository defines: GetAll, GetAllWithDetails, GetById, Add, Insert, Update, Delete. OrderRepository implements methods and works with ElectronicsStoreContext.

public class Orders
{
public int ID { get; set; }
public int EmployeeID { get; set; }
public int CustomerID { get; set; }
public DateTime Date { get; set; }
public string? Note { get; set; }
public virtual ObservableCollectionListSource<Order_Details> ViewDetails { get; } = new();
public virtual Customers Customer { get; set; } = null!; 
public virtual Employees { get; set; set; } = null!; 
} 
public class OrderRepository : IOrderRepository 
{ 
private readonly ElectronicsStoreContext _context; 

public OrderRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
}
public interface IOrderRepository 
{ 
List<Orders> GetAllWithDetails(); 
List<Orders> GetAll(); 
Orders? GetById(int id); 
void Add(Orders entity); 
int Insert(Orders order); 
void Update(Orders order); 
void Delete(Orders order);
}


#### OrderDetails
Entity description: the Order_Details class represents the Order_Details table in the database.
Main Attributes:
 ID: primary key, auto-increment.
 OrderID, ProductID: link to order and product.
 Quantity, Price: quantity and unit price at the time of order.
Relationship: An Order_Detail belongs to an Order (N - 1). An Order_Detail belongs to a Product (N - 1).
Repository and handling: IOrderDetailsRepository defines: GetByOrderID, DeleteByOrderID, AddRange, Insert. OrderDetailsRepository implements these methods to interact with ElectronicsStoreContext.

public class Order_Details
{
public int ID { get; set; }
public int OrderID { get; set; set; } 
public int ProductID { get; set; set; } 
public short Quantity { get; set; set; } 
public int Price { get; set; set; } 
public virtual Orders Order { get; set; set; } = null!; 
public virtual Products Product { get; set; set; } = null!;
}
public class OrderDetailsRepository : IOrderDetailsRepository 
{ 
private readonly ElectronicsStoreContext _context; 

public OrderDetailsRepository() 
{ 
_context = new ElectronicsStoreContext(); 
}
}
public interface IOrderDetailsRepository
{ 
List<Order_Details> GetByOrderID(int orderId); 
void DeleteByOrderID(int orderId); 
void AddRange(List<Order_Details> details); 
void Insert(Order_Details detail);
}
## PROCESSING DESIGN
### Login processing flow
Purpose: authenticate users (employees) when logging into the system.
Processing flow:
1. User enters UserName and Password in the login form.
2. Form sends information to EmployeeService (BLL).
3. EmployeeService calls EmployeeRepository.GetbyUserName(string userName) to get employee information.
4. If UserName exists, check the password by comparing it with the encrypted password (using BCrypt).
5. If correct, return Employees object, allow login; if incorrect, return error message.
Processing flow diagram:
### Product management processing flow
Functions include: Add, edit, delete, display product list, including images, manufacturers, categories.
1. Add product:
User enters product information and selects image.
The interface calls ProductService.Add(ProductsDTO productDto) to check the data.
If valid, ProductService maps to Products entity and calls ProductRepository.Add(product). Save image if available, and display new product.

2. Edit product:
User selects product to edit. Data is loaded into form, allowing editing. Call ProductService.Update(productDto) to process update. If there is new image, call UpdateImage(productId, fileName).

3. Delete product:
The interface confirms deleting product.
Call ProductService.Delete(productId) → ProductRepository.Delete.
## Order processing flow (Create invoice)
Order process:
1. At the frmOrderDetails interface, the user selects an old customer or adds a new customer, selects an employee and adds products.
2. Click the “Save invoice” button:
 Call OrderService.Insert(OrderDTO orderDto):
 Check validity (is the product in stock? is the quantity valid?)
 Create Orders → OrderRepository.Insert(order) → get back the OrderID.
 Call OrderDetailsService.AddRange(List<Order_Details>) to add invoice details.
 Update the product inventory quantity.

### Invoice statistics processing flow
1. The user selects the time period to be counted at frmReport.
2. Interface calls OrderService.GetAllWithDetails() or according to filter conditions.

3. Data returns a list of orders with details.

4. Send data to ReportViewer, use rdlc template to display statistics on revenue, orders, and products sold.

### Customer management processing flow
1. Add new customer: enter information in form → CustomerService.Add() → CustomerRepository.Add().
2. Update information: upload data to form → edit → Update().
3. Delete customer: Delete() (if not related to invoice).

### Employee management processing flow
1. Add new employee: enter information in form → CustomerService.Add() → CustomerRepository.Add().
2. Update information: upload data to form → edit → Update().

3. Delete employee: Delete() (if not related to invoice).

4. Update password: call UpdatePassword(id, hashedPassword) at EmployeeRepository.

# SUMMARY
##. ACHIEVED RESULTS
After the process of researching, designing and building the electronic store management system, our group (or individuals) has completed many important contents in both theory and practice, specifically as follows:
## Understand the architecture and development process of management software
During the project, I had the opportunity to research and apply the 3-tier software architecture model (Three-Tier Architecture) including: Presentation Layer (UI), Business Logic Layer (BLL) and Data Access Layer (DAL). This architecture helps to clearly separate roles in the software, supporting maintenance, expansion and reuse of source code in the future.

In addition, the use of the ORM tool Entity Framework Core has helped reduce the work of directly manipulating the database, strongly supporting the mapping of tables to model classes (entities) automatically and clearly.

### Complete construction of electronic sales management software
The software is designed and installed with all the basic and necessary functions for an electronic store, including:
- Managing product categories: products, product types, manufacturers;
- Managing customers, employees;
- Managing orders and order details;
- Reporting statistics on revenue, products sold;
- Login function, assigning employee rights according to roles (admin/employee).

The software interface is built with WinForms (.NET 8), user-friendly, easy to use and highly intuitive. All operations are checked and data is validated to ensure accuracy.h accuracy during data entry.
###. Improve professional skills and practical work
Through this project, we have improved our programming skills with C#, .NET Core, SQL Server, as well as practiced the skills of designing standardized databases, organizing projects according to real models. In addition, getting familiar with AutoMapper, DTO, and reporting operations using RDLC Report Designer helps to expand our thinking and ability to develop practical applications in a professional direction. The skills of independent work, systematic thinking, work organization and software testing have also been significantly improved after the project implementation period.
## DEVELOPMENT DIRECTION
Although the software has been completed at a basic level, to better meet the practical requirements of electronic stores or expand the scale of the business, the system still has a lot of potential for further development. Some typical development directions include:
###. Developing Web or Mobile components
In the context of increasingly strong digital transformation, expanding the system to a Web platform (ASP.NET Core Web App or Web API) or Mobile application (Xamarin/Maui) will help customers order online, employees can manage remotely, thereby improving the experience and operational efficiency.
### Expanding advanced functions
In the next versions, the software can be supplemented with many advanced functions such as:
- Integrating QR code/barcode to support fast sales;

- Sending order confirmation emails or promotional notifications to customers;
- Building Log function, logging.
- Processing invoice status. Such as ready, completed, canceled ...
- Smart statistics such as: revenue chart, period comparison, inventory suggestions, best-selling items.

### Multi-user connection
Currently, the software mainly operates in a single-machine or simple LAN environment. In the future, the software can be deployed as a multi-user client-server, with a more detailed authorization mechanism such as: only allowing sales staff to view, but not edit old orders; only admins have the right to make statistics, export reports, etc.
In addition, it is possible to integrate the operation logging feature, serving the purpose of tracking history and securing the system.
## GENERAL CONCLUSION
The project has helped me have a more realistic view of the process of building management software in a business environment. With the knowledge accumulated from school, combined with the ability to self-study, we have built an electronic store management system that is highly applicable, meets technical standards, and has the potential to develop in the future.
