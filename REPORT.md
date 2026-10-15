# Practice Assignment 2: Building a Device Management Software
**Course**: Web Application Development / Software Engineering  
**Student Name**: Nguyen Duc Anh  
**Student ID**: 2031200076  
**Platform**: ASP.NET Core 10.0 MVC, Entity Framework Core with SQLite, Bootstrap 5  

---

## 1. Project Overview & Architecture

The **Device Management Software** is an ASP.NET Core MVC application developed to manage IT hardware, equipment categories, and employee device allocations. The solution is architected according to **Clean Code** principles and separation of concerns:

- **Models**: Defines business entities, domain validations via DataAnnotations, and ViewModels.
- **Data (ORM)**: Uses Entity Framework Core with SQLite (`AppDbContext`), configured with unique indexes, foreign key constraints, and automatic database initialization and seeding (`DbInitializer`).
- **Controllers**: Thin controllers handling HTTP requests asynchronously (`async`/`await`), applying business logic, validation, and passing data to views.
- **Views**: Razor Views utilizing Bootstrap 5 and Bootstrap Icons for a modern, responsive, and intuitive user experience.

### Directory Structure
```text
Lab2_NguyenDucAnh_2031200076/
│
├── DeviceManagement.sln               # Solution file
├── .gitignore                         # Standard git ignore for .NET
├── REPORT.md                          # Project implementation report
│
└── DeviceManagement/
    ├── Controllers/
    │   ├── HomeController.cs          # Dashboard with summary statistics
    │   ├── CategoriesController.cs    # Category CRUD and list devices by category
    │   ├── DevicesController.cs       # Device CRUD, search and filtering
    │   └── UsersController.cs         # User/Employee CRUD and assigned devices
    │
    ├── Data/
    │   ├── AppDbContext.cs            # EF Core DbContext with constraints
    │   └── DbInitializer.cs          # Database seeder with sample data
    │
    ├── Models/
    │   ├── DeviceStatus.cs            # Enum: InUse, Broken, UnderMaintenance
    │   ├── DeviceCategory.cs          # Category entity (Computer, Printer, Phone...)
    │   ├── Device.cs                  # Device entity (Name, Code, Status, EntryDate...)
    │   ├── User.cs                    # User entity (FullName, Email, Phone, Dept)
    │   └── ViewModels/
    │       ├── DeviceFilterViewModel.cs # ViewModel for search & filter
    │       └── DashboardViewModel.cs   # ViewModel for metrics dashboard
    │
    ├── Views/
    │   ├── Categories/                # Index, Create, Edit, Delete, Details, Devices
    │   ├── Devices/                   # Index, Create, Edit, Delete, Details
    │   ├── Users/                     # Index, Create, Edit, Delete, Details
    │   ├── Home/                      # Dashboard Index, Privacy
    │   └── Shared/                    # _Layout, _ValidationScriptsPartial
    │
    ├── appsettings.json               # SQLite connection string & config
    └── Program.cs                     # DI container, DB setup & middleware pipeline
```

---

## 2. Created Classes & Domain Model

### 2.1. `DeviceCategory`
- `Id`: Primary key.
- `Name`: Category name (e.g., Computer, Printer, Phone), marked with `[Required]`, `[StringLength(100)]`, and configured with a **Unique Index** in EF Core to avoid duplicates.
- `Description`: Optional description.
- `Devices`: One-to-Many navigation property (`ICollection<Device>`).

### 2.2. `DeviceStatus` (Enum)
- `InUse` (1, Display: "In use")
- `Broken` (2, Display: "Broken")
- `UnderMaintenance` (3, Display: "Under maintenance")

### 2.3. `Device`
- `Id`: Primary key.
- `Name`: Device name (e.g., Dell XPS 15), `[Required]`, max 100 chars.
- `Code`: Unique device identifier (e.g., `DEV-001`), validated with `[RegularExpression]` and a **Unique Index** in the database.
- `CategoryId`: Foreign key pointing to `DeviceCategory`.
- `Status`: `DeviceStatus` enum.
- `EntryDate`: `DateTime` representing the date the device was entered into the inventory (`[DataType(DataType.Date)]`).
- `UserId`: Optional foreign key pointing to `User` (employee who is currently assigned the device).
- `Description`: Optional technical specifications or notes.

### 2.4. `User`
- `Id`: Primary key.
- `FullName`: Full name, `[Required]`, max 100 chars.
- `Email`: Email address, `[Required]`, `[EmailAddress]`, unique constraint in DB.
- `PhoneNumber`: Phone number, `[Required]`, `[Phone]`, regex-checked.
- `Department`: Optional department string.
- `Devices`: One-to-Many navigation property for assigned devices.

### 2.5. `DeviceFilterViewModel`
Encapsulates search parameters (`SearchTerm`), filter dropdown selections (`CategoryId`, `Status`, `UserId`), the filtered device list (`Devices`), and dropdown `SelectList` items.

---

## 3. Implementation of Main Features

### Feature 1: Device Category Management
- **List Categories**: `CategoriesController.Index` fetches all categories with `.Include(c => c.Devices)` to display the number of devices in each category.
- **Add Category**: `CategoriesController.Create` validates inputs using `ModelState` and checks for duplicate category names before inserting.
- **Edit Category**: `CategoriesController.Edit` permits renaming and updating description while preventing collision with other existing category names.
- **Delete Category**: Includes business protection logic: checks if any device is linked to the category; if so, deletion is blocked with a friendly alert (`TempData["ErrorMessage"]`), preventing orphan data.
- **Display Devices by Category**: `CategoriesController.Devices(int id)` specifically retrieves all devices under the given category and renders them in a dedicated view `Views/Categories/Devices.cshtml`.

### Feature 2: Device Management
- **Add Device**: `DevicesController.Create` presents a clean form with dropdowns for Category, Status enum, and User assignment. Validates that the device code is unique.
- **Edit Device**: `DevicesController.Edit` updates device attributes while enforcing unique code constraints.
- **Delete Device**: `DevicesController.Delete` asks for confirmation before removing the device.
- **Device Details**: Displays all mandatory metadata (Name, Code, Category, Status badge, Date of Entry) and assigned employee info.

### Feature 3: User Management
- **User CRUD**: `UsersController` manages users/employees with validation on name, email format, and phone number.
- **Device Association**: Users can be linked to devices. In `UsersController.Details`, the system displays all devices currently assigned to that user.
- **Safe Deletion**: When a user is deleted, EF Core foreign key behavior `OnDelete(DeleteBehavior.SetNull)` safely sets `Device.UserId` to `null` so devices remain in inventory rather than being accidentally deleted.

### Feature 4: Search and Filter Data
- Implemented dynamically in `DevicesController.Index`:
  - **Search by Name or Device Code**:
    ```csharp
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        var search = searchTerm.Trim().ToLower();
        query = query.Where(d => d.Name.ToLower().Contains(search) || d.Code.ToLower().Contains(search));
    }
    ```
  - **Filter by Category**:
    ```csharp
    if (categoryId.HasValue && categoryId.Value > 0)
    {
        query = query.Where(d => d.CategoryId == categoryId.Value);
    }
    ```
  - **Filter by Status**:
    ```csharp
    if (status.HasValue)
    {
        query = query.Where(d => d.Status == status.Value);
    }
    ```
- The view `Devices/Index.cshtml` combines search and dropdown filters in a single intuitive toolbar with a "Filter" button and a "Reset" button.

---

## 4. Application of Clean Code Principles & Coding Standards

1. **Meaningful Naming**: Controllers, models, properties, and methods use standard C# conventions (PascalCase for classes/methods/properties, camelCase for local variables).
2. **Separation of Concerns (SoC)**: Controllers contain no raw SQL or HTML generation; views only format data; entities and ViewModels model business structures.
3. **Single Responsibility Principle (SRP)**: Each controller is responsible for a single domain entity (`CategoriesController`, `DevicesController`, `UsersController`).
4. **DRY (Don't Repeat Yourself)**: Shared layout `_Layout.cshtml`, partial views for validation `_ValidationScriptsPartial`, and reusable helper methods.
5. **Data Integrity & Validation**: Double-layer validation (client-side unobtrusive jQuery validation + server-side DataAnnotations and duplicate checks).
6. **Graceful User Feedback**: TempData alerts for success and error handling.

---

## 5. How to Run the Application

1. Open a terminal in the project directory:
   ```bash
   cd DeviceManagement
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. Open your browser and navigate to the displayed URL (e.g., `http://localhost:5000` or `http://localhost:5123`). The SQLite database `devicemanagement.db` will be created and seeded automatically.
