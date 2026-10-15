# Device Management Software (.NET Core MVC)

**Practice Assignment 2 - Device Management Software**  
**Student**: Nguyen Duc Anh  
**Student ID**: 2031200076  

## Technologies Used
- **Framework**: .NET 10.0 ASP.NET Core MVC
- **Database**: SQLite with Entity Framework Core (Code-First)
- **Frontend**: Bootstrap 5, Bootstrap Icons, jQuery Validation

## Key Features Implemented
1. **Device Category Management**:
   - Add, edit, delete categories (e.g. Computer, Printer, Phone...).
   - View category list with device counts.
   - Display a list of devices by category.
2. **Device Management**:
   - Full CRUD for devices.
   - Device attributes: Device Name, Device Code, Category, Status (In use, Broken, Under maintenance), Date of Entry, Assigned User, Description.
3. **User Management**:
   - Add, edit, delete users/employees with Full name, Email, Phone number, Department.
   - View assigned devices per user.
4. **Search and Filter Data**:
   - Search devices by name or device code.
   - Filter devices by category or status.
5. **Dashboard & UX**:
   - Interactive dashboard with real-time statistics and status breakdown.
   - Client-side & server-side validation.
   - Clean, responsive UI with alerts and status badges.

## Quick Start
```bash
# Clone the repository
git clone <your-repo-link>
cd Lab2_NguyenDucAnh_2031200076

# Run the project (Database will auto-create and seed)
dotnet run --project DeviceManagement/DeviceManagement.csproj
```
Open browser at `http://localhost:5000` or `http://localhost:5123`.
