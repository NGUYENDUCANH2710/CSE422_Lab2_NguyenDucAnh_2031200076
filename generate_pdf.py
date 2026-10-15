import sys
from reportlab.lib.pagesizes import letter, A4
from reportlab.lib import colors
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, KeepTogether, HRFlowable
)
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.pdfgen import canvas

class NumberedCanvas(canvas.Canvas):
    def __init__(self, *args, **kwargs):
        super(NumberedCanvas, self).__init__(*args, **kwargs)
        self._saved_page_states = []

    def showPage(self):
        self._saved_page_states.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        num_pages = len(self._saved_page_states)
        for state in self._saved_page_states:
            self.__dict__.update(state)
            self.draw_page_number(num_pages)
            canvas.Canvas.showPage(self)
        canvas.Canvas.save(self)

    def draw_page_number(self, page_count):
        self.saveState()
        self.setFont("Helvetica", 8)
        self.setFillColor(colors.HexColor("#6c757d"))
        
        # Footer text
        footer_text = f"Practice Assignment 2: Device Management Software | Student: Nguyen Duc Anh (ID: 2031200076) | Page {self._pageNumber} of {page_count}"
        self.drawCentredString(A4[0] / 2.0, 25, footer_text)
        
        # Bottom rule
        self.setStrokeColor(colors.HexColor("#dee2e6"))
        self.setLineWidth(0.5)
        self.line(36, 36, A4[0] - 36, 36)
        
        self.restoreState()

def create_report():
    pdf_filename = "Report_NguyenDucAnh_2031200076.pdf"
    
    # 36 pt = 0.5 inch margins
    doc = SimpleDocTemplate(
        pdf_filename,
        pagesize=A4,
        leftMargin=36,
        rightMargin=36,
        topMargin=32,
        bottomMargin=42
    )

    styles = getSampleStyleSheet()
    
    # Custom Typography Styles
    title_style = ParagraphStyle(
        'DocTitle',
        parent=styles['Heading1'],
        fontName='Helvetica-Bold',
        fontSize=15,
        leading=18,
        textColor=colors.HexColor("#0d6efd"),
        spaceAfter=2
    )
    
    subtitle_style = ParagraphStyle(
        'DocSubtitle',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=9,
        leading=12,
        textColor=colors.HexColor("#495057"),
        spaceAfter=6
    )

    h1_style = ParagraphStyle(
        'SectionH1',
        parent=styles['Heading2'],
        fontName='Helvetica-Bold',
        fontSize=11,
        leading=13,
        textColor=colors.HexColor("#0d6efd"),
        spaceBefore=6,
        spaceAfter=4,
        keepWithNext=True
    )
    
    h2_style = ParagraphStyle(
        'SectionH2',
        parent=styles['Heading3'],
        fontName='Helvetica-Bold',
        fontSize=9.5,
        leading=12,
        textColor=colors.HexColor("#212529"),
        spaceBefore=4,
        spaceAfter=2,
        keepWithNext=True
    )

    body_style = ParagraphStyle(
        'BodyDark',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=8.5,
        leading=11.2,
        textColor=colors.HexColor("#212529"),
        spaceAfter=4
    )

    body_bold = ParagraphStyle(
        'BodyBold',
        parent=body_style,
        fontName='Helvetica-Bold'
    )

    bullet_style = ParagraphStyle(
        'BulletDark',
        parent=body_style,
        leftIndent=12,
        firstLineIndent=-8,
        spaceAfter=2.5
    )

    code_style = ParagraphStyle(
        'CodeSnippet',
        parent=styles['Code'],
        fontName='Courier',
        fontSize=7.5,
        leading=9.5,
        textColor=colors.HexColor("#1e293b"),
        backColor=colors.HexColor("#f8f9fa"),
        borderColor=colors.HexColor("#e2e8f0"),
        borderWidth=0.5,
        borderPadding=4,
        spaceBefore=3,
        spaceAfter=4
    )

    table_header_style = ParagraphStyle(
        'TableHeader',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=8,
        leading=10,
        textColor=colors.HexColor("#1e293b")
    )

    table_cell_style = ParagraphStyle(
        'TableCell',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=8,
        leading=10,
        textColor=colors.HexColor("#334155")
    )

    elements = []

    # ========================== PAGE 1 ==========================
    # Header Banner Table
    header_data = [
        [
            Paragraph("Practice Assignment 2: Device Management Software", title_style),
            Paragraph("<b>Student:</b> Nguyen Duc Anh<br/><b>Student ID:</b> 2031200076", table_cell_style)
        ],
        [
            Paragraph(".NET Core 10.0 MVC &bull; Entity Framework Core &bull; Clean Code &bull; SQLite", subtitle_style),
            Paragraph("<b>Date:</b> October 2026", table_cell_style)
        ]
    ]
    header_table = Table(header_data, colWidths=[370, 153])
    header_table.setStyle(TableStyle([
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('ALIGN', (1, 0), (1, -1), 'RIGHT'),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 0),
        ('TOPPADDING', (0, 0), (-1, -1), 0),
    ]))
    elements.append(header_table)
    elements.append(HRFlowable(width="100%", thickness=1.5, color=colors.HexColor("#0d6efd"), spaceBefore=4, spaceAfter=6))

    # Section 1: Overview & Architecture
    elements.append(Paragraph("1. Project Overview & Architecture", h1_style))
    elements.append(Paragraph(
        "The <b>Device Management Software</b> is built using <b>ASP.NET Core 10.0 MVC</b> with <b>Entity Framework Core</b> and <b>SQLite</b>. "
        "The architecture implements Clean Code principles and Separation of Concerns (SoC) across distinct layers:",
        body_style
    ))

    arch_data = [
        [Paragraph("Folder / Component", table_header_style), Paragraph("Key Classes & Files", table_header_style), Paragraph("Architectural Responsibility", table_header_style)],
        [Paragraph("<b>Models/</b>", table_cell_style), Paragraph("Device, DeviceCategory, User, DeviceStatus", table_cell_style), Paragraph("Domain entities with DataAnnotations validation & enum definition.", table_cell_style)],
        [Paragraph("<b>ViewModels/</b>", table_cell_style), Paragraph("DeviceFilterViewModel, DashboardViewModel", table_cell_style), Paragraph("Encapsulates search queries, select lists, and dashboard metrics.", table_cell_style)],
        [Paragraph("<b>Data/</b>", table_cell_style), Paragraph("AppDbContext, DbInitializer", table_cell_style), Paragraph("EF Core Code-First DbContext with unique constraints & data seeding.", table_cell_style)],
        [Paragraph("<b>Controllers/</b>", table_cell_style), Paragraph("Home, Categories, Devices, Users", table_cell_style), Paragraph("Async controllers handling CRUD, search/filter, and business rules.", table_cell_style)],
        [Paragraph("<b>Views/</b>", table_cell_style), Paragraph("Razor Views + Bootstrap 5 + Icons", table_cell_style), Paragraph("Responsive UI, search toolbars, status badges, and feedback alerts.", table_cell_style)]
    ]
    arch_table = Table(arch_data, colWidths=[95, 175, 253])
    arch_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor("#f1f5f9")),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor("#cbd5e1")),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('TOPPADDING', (0, 0), (-1, -1), 3),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 3),
    ]))
    elements.append(arch_table)
    elements.append(Spacer(1, 4))

    # Section 2: Created Classes & Domain Model
    elements.append(Paragraph("2. Created Classes & Domain Model", h1_style))
    class_cards = [
        [
            Paragraph("<b>DeviceCategory</b><br/>&bull; <code>Id</code> (int, PK)<br/>&bull; <code>Name</code> (string, Required, Unique Index)<br/>&bull; <code>Description</code> (string?)<br/>&bull; <code>Devices</code> (ICollection&lt;Device&gt;)", table_cell_style),
            Paragraph("<b>User (Employee)</b><br/>&bull; <code>Id</code> (int, PK)<br/>&bull; <code>FullName</code> (string, Required, max 100)<br/>&bull; <code>Email</code> (string, Unique, EmailAddress)<br/>&bull; <code>PhoneNumber</code> (string, Required, Phone)<br/>&bull; <code>Devices</code> (ICollection&lt;Device&gt;)", table_cell_style)
        ],
        [
            Paragraph("<b>DeviceStatus (Enum)</b><br/>&bull; <code>InUse = 1</code> (Display: 'In use')<br/>&bull; <code>Broken = 2</code> (Display: 'Broken')<br/>&bull; <code>UnderMaintenance = 3</code> (Display: 'Under maintenance')", table_cell_style),
            Paragraph("<b>Device</b><br/>&bull; <code>Id</code> (PK), <code>Name</code>, <code>Code</code> (Unique Index, regex)<br/>&bull; <code>CategoryId</code> (FK to Category), <code>Status</code> (Enum)<br/>&bull; <code>EntryDate</code> (Date), <code>UserId</code> (Nullable FK to User)", table_cell_style)
        ]
    ]
    class_table = Table(class_cards, colWidths=[261, 262])
    class_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, -1), colors.HexColor("#f8fafc")),
        ('BOX', (0, 0), (0, 0), 0.5, colors.HexColor("#cbd5e1")),
        ('BOX', (1, 0), (1, 0), 0.5, colors.HexColor("#cbd5e1")),
        ('BOX', (0, 1), (0, 1), 0.5, colors.HexColor("#cbd5e1")),
        ('BOX', (1, 1), (1, 1), 0.5, colors.HexColor("#cbd5e1")),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('TOPPADDING', (0, 0), (-1, -1), 4),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ]))
    elements.append(class_table)
    elements.append(Spacer(1, 4))

    # Section 3: Feature Implementation Details (Feature 1 & 2 start)
    elements.append(Paragraph("3. Feature Implementation Details", h1_style))
    
    elements.append(Paragraph("<b>Feature 1: Device Category Management</b>", h2_style))
    elements.append(Paragraph("&bull; <b>CRUD Operations:</b> <code>CategoriesController</code> provides full Add, Edit, Delete, and List actions.", bullet_style))
    elements.append(Paragraph("&bull; <b>Duplicate Validation:</b> Validates uniqueness of category names using EF Core async queries before saving.", bullet_style))
    elements.append(Paragraph("&bull; <b>Data Integrity Protection:</b> Checks if devices are assigned to a category before deletion; blocks deletion with a friendly alert if devices exist, preventing orphaned records.", bullet_style))
    elements.append(Paragraph("&bull; <b>Display Devices by Category:</b> Dedicated action <code>CategoriesController.Devices(int id)</code> filters and renders all hardware units belonging to the specified category.", bullet_style))

    # ========================== PAGE 2 ==========================
    elements.append(PageBreak())

    elements.append(Paragraph("<b>Feature 2: Device Management</b>", h2_style))
    elements.append(Paragraph("&bull; <b>Attributes Tracked:</b> Device name, unique device code (e.g. DEV-001), category selection, status (In use, Broken, Under maintenance), date of entry, and assigned employee.", bullet_style))
    elements.append(Paragraph("&bull; <b>Input Validation:</b> Server-side Model validation + client-side jQuery unobtrusive validation. Enforces unique device code check against the database.", bullet_style))
    elements.append(Paragraph("&bull; <b>Details & Deletion:</b> <code>Details</code> view displays hardware specifications and user contact; <code>Delete</code> view provides clear confirmation safeguards.", bullet_style))

    elements.append(Paragraph("<b>Feature 3: User / Employee Management</b>", h2_style))
    elements.append(Paragraph("&bull; <b>User Attributes:</b> Full name, unique email address, phone number, and organizational department.", bullet_style))
    elements.append(Paragraph("&bull; <b>Device Allocation:</b> Users are associated with devices via a foreign key relation. <code>Users/Details.cshtml</code> lists all devices currently in possession of the selected user.", bullet_style))
    elements.append(Paragraph("&bull; <b>Cascade Protection:</b> Configured <code>OnDelete(DeleteBehavior.SetNull)</code> in EF Core: deleting an employee releases assigned devices back to unassigned inventory without deleting the equipment.", bullet_style))

    elements.append(Paragraph("<b>Feature 4: Search and Filter Data</b>", h2_style))
    elements.append(Paragraph(
        "Implemented dynamically in <code>DevicesController.Index</code> using LINQ query composition:",
        body_style
    ))
    code_text = (
        "var query = _context.Devices.Include(d => d.Category).Include(d => d.User).AsQueryable();\n"
        "if (!string.IsNullOrWhiteSpace(searchTerm))\n"
        "    query = query.Where(d => d.Name.ToLower().Contains(searchTerm.ToLower()) || d.Code.ToLower().Contains(searchTerm.ToLower()));\n"
        "if (categoryId.HasValue && categoryId.Value > 0)\n"
        "    query = query.Where(d => d.CategoryId == categoryId.Value);\n"
        "if (status.HasValue)\n"
        "    query = query.Where(d => d.Status == status.Value);"
    )
    elements.append(Paragraph(code_text.replace("\n", "<br/>").replace(" ", "&nbsp;"), code_style))
    elements.append(Paragraph("&bull; <b>Search:</b> Searches simultaneously across both device name and device code.", bullet_style))
    elements.append(Paragraph("&bull; <b>Filter:</b> Filters simultaneously by Category and Status. Includes an instant Reset filter button.", bullet_style))
    elements.append(Spacer(1, 4))

    # Section 4: Clean Code & Coding Standards
    elements.append(Paragraph("4. Clean Code Principles & Coding Standards Applied", h1_style))
    elements.append(Paragraph("&bull; <b>Single Responsibility Principle (SRP):</b> Each controller is solely responsible for one entity aggregate (Categories, Devices, Users).", bullet_style))
    elements.append(Paragraph("&bull; <b>Don't Repeat Yourself (DRY):</b> Centralized navigation in <code>_Layout.cshtml</code>, reusable validation partials, and unified ViewModels.", bullet_style))
    elements.append(Paragraph("&bull; <b>Naming Conventions:</b> Strict adherence to C# standards: PascalCase for classes/methods/properties, camelCase for local variables and parameters.", bullet_style))
    elements.append(Paragraph("&bull; <b>Asynchronous I/O:</b> All controller actions utilize <code>async/await</code> and EF Core async methods (<code>ToListAsync</code>, <code>AnyAsync</code>, <code>SaveChangesAsync</code>) for high throughput.", bullet_style))
    elements.append(Paragraph("&bull; <b>Defensive Programming:</b> Graceful error handling, TempData alerts for success/error feedback, and FK integrity constraints.", bullet_style))
    elements.append(Paragraph("&bull; <b>Intuitive UI & UX:</b> Responsive Bootstrap 5 layout, color-coded status badges, and an interactive Home Dashboard summarizing inventory statistics.", bullet_style))
    elements.append(Spacer(1, 4))

    # Section 5: Execution & Submission
    elements.append(Paragraph("5. Execution Guide & Submission", h1_style))
    elements.append(Paragraph("&bull; <b>Automatic Database Initialization:</b> <code>DbInitializer</code> automatically creates the SQLite database (<code>devicemanagement.db</code>) and seeds sample categories, users, and devices upon startup.", bullet_style))
    elements.append(Paragraph("&bull; <b>Run Command:</b> <code>dotnet run --project DeviceManagement/DeviceManagement.csproj</code> then open <code>http://localhost:5000</code>.", bullet_style))
    elements.append(Paragraph("&bull; <b>Submission Package:</b> Source code archive ready for Moodle and initialized with Git for public GitHub repository submission.", bullet_style))

    # Build document
    doc.build(elements, canvasmaker=NumberedCanvas)
    print("Report PDF generated successfully.")

if __name__ == "__main__":
    create_report()
