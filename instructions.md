Based on the Claude Code documentation you shared, I will create a comprehensive `CLAUDE.md` file for your Document Archiver project. This file will act as persistent memory for Claude, ensuring it understands the project's architecture, constraints, and workflows across all future sessions.

---

# `CLAUDE.md` for Document Archiver (Windows Forms / .NET 8)

```markdown
# Document Archiver - Project Memory

## 🏗️ Project Overview

**Document Archiver** is a Windows Forms desktop application for high-volume document scanning and archiving. It integrates with TWAIN scanners, performs OCR using Tesseract, generates PDFs, and stores records in a SQL Server database.

**Target Framework:** .NET 8 (Windows Forms)
**Primary Language:** C#
**UI Culture:** Arabic (Right-to-Left layout)

## 📁 Core Architecture

```
DocumentArchiever/
├── UI/Forms/           # WinForms UI layer
├── Core/Scanning/      # TWAIN/WIA scanner abstraction
├── Core/OCR/           # Tesseract OCR engine wrapper
├── Core/Processing/    # Document processing & PDF generation
├── Data/               # Database service & entities
├── Services/           # Logging, updates, settings
├── Helpers/            # Image, path, validation utilities
└── Configuration/      # App settings
```

## 🔑 Critical Implementation Rules

### 1. NTwain v4 is NOT Async
Unlike typical .NET async patterns, NTwain v4 uses **synchronous methods** with event-driven callbacks.

```csharp
// ✅ CORRECT
_twainSession = new TwainAppSession(appThreadContext: SynchronizationContext.Current);
_twainSession.OpenDsm();                    // NOT OpenDsmAsync()
_twainSession.OpenSource(selectedSource);   // NOT OpenSourceAsync()
_twainSession.EnableSource(SourceEnableOption.NoUI);  // NOT EnableSourceAsync()

// ❌ WRONG - No Async versions exist
await _twainSession.OpenDsmAsync();  // Compile error!
```

### 2. Scanner Events Flow
Always attach event handlers BEFORE opening the DSM:

```csharp
_twainSession.StateChanged += OnStateChanged;
_twainSession.Transferred += OnTransferred;      // Each scanned image
_twainSession.SourceDisabled += OnSourceDisabled; // Scan complete
_twainSession.TransferError += OnTransferError;
```

### 3. Image Data Handling
Use `TakeDataOwnership()` and `AsStream()` to process images:

```csharp
private void OnTransferred(object sender, TransferredEventArgs e)
{
    using (var data = e.TakeDataOwnership())
    using (var img = Image.FromStream(data.AsStream()))
    {
        // Save to temp location first
        string tempPath = Path.Combine(Path.GetTempPath(), $"scan_{Guid.NewGuid()}.jpg");
        img.Save(tempPath, ImageFormat.Jpeg);
        _scannedImages.Add(tempPath);
    }
}
```

### 4. Folder Creation Must Happen BEFORE Scanning
When `StartScanningAsync` is called, the save directory must already exist:

```csharp
private ScanSession CreateScanSession()
{
    var session = new ScanSession { ... };
    CreateSessionFolders(session);  // ← CRITICAL: Create now!
    return session;
}
```

### 5. Database Save Flow
Documents are saved to database **after** PDF generation completes successfully:

```csharp
// In DocumentProcessor.SaveDocumentAsync:
await _pdfGenerator.GenerateAsync(pdfPath, frontImage, backImage);
bool saved = await _databaseService.SaveDocumentAsync(document);
if (!saved) throw new Exception("DB save failed");
```

## 🔧 Build & Run Commands

```bash
# Build solution
dotnet build DocumentArchiever.sln -c Release

# Run application
dotnet run --project DocumentArchiever.csproj

# Publish self-contained
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 🗄️ Database Schema (SQL Server)

Required tables (run on first setup):

```sql
CREATE TABLE tblBranches (
    ID INT PRIMARY KEY IDENTITY(1,1),
    BranchName NVARCHAR(100) NOT NULL
);

CREATE TABLE tblDocArchive (
    ID INT PRIMARY KEY IDENTITY(1,1),
    TheNumber BIGINT NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    FileName NVARCHAR(200) NOT NULL,
    BranchID INT NOT NULL,
    DocType INT NOT NULL,      -- 9=Transfer, 18=Cash, 91=InterBranch
    TheYear INT NOT NULL,
    EnterTime DATETIME NOT NULL DEFAULT GETDATE()
);
```

## 📝 Coding Standards

- **Naming:** PascalCase for classes/methods, camelCase for parameters
- **UI Thread:** Use `Control.Invoke()` when updating UI from background threads
- **Arabic Text:** All UI labels should be in Arabic (RightToLeft = Yes)
- **Logging:** Use `ILogger` interface, never `Console.WriteLine`
- **Disposal:** All scanners, OCR engines, and streams must implement `IDisposable`

## 🧪 Testing Checklist for Claude

Before considering a task complete, verify:

1. [ ] Scanner connects and disconnects properly
2. [ ] Duplex scanning captures both front and back
3. [ ] Sequential numbering increments correctly for cash documents
4. [ ] Express numbers (363xxxxxx) are extracted for transfer documents
5. [ ] PDFs are saved to correct year/branch/date/docType folder
6. [ ] Database record is created with correct TheNumber, FilePath, FileName
7. [ ] Settings persist after restart (last branch, last date, DPI, color mode)
8. [ ] Application exits completely with no background threads hanging
9. [ ] Progress bar stops after scan completes

## ⚠️ Known Constraints

- **TWAIN only works on Windows** (no Linux/macOS support)
- **Tesseract language files** (ara.traineddata, eng.traineddata) must be in `./tessdata/`
- **Ghostscript optional** but recommended for PDF text extraction
- **.NET 8 required** - will not run on older frameworks

## 🔗 Related Files

- Main scanning logic: `Core/Scanning/TwainScanner.cs`
- OCR implementation: `Core/OCR/TesseractOcrEngine.cs`
- Document processing: `Core/Processing/DocumentProcessor.cs`
- Database operations: `Data/DatabaseService.cs`
- Main UI: `UI/Forms/MainForm.cs`

## 📚 Imported Guidelines

@README.md
@docs/CODING_STANDARDS.md
```

---

## How to Use This File

1. **Save the content above** as `CLAUDE.md` in the **root of your project folder**.

2. **Optional:** To keep it out of version control (for personal settings), save it as `CLAUDE.local.md` instead and add it to `.gitignore`.

3. **To make Claude load it automatically**, ensure you run Claude Code from the same directory (or a subdirectory). Claude reads `CLAUDE.md` files by walking up the directory tree.

4. **Verify it's loaded:** In your Claude session, type `/memory` to see which memory files are active.

## What This File Does

- Tells Claude your project uses **.NET 8 WinForms** (not a web framework)
- Explains that **NTwain v4 is synchronous**, preventing Claude from wrongly suggesting `await` on TWAIN methods
- Documents the **critical folder creation** step that must happen before scanning
- Provides **build commands** so Claude can test code changes
- Lists the **database schema** so Claude understands the data layer
- Includes a **testing checklist** Claude can use to verify fixes
- Defines **coding standards** for consistent code generation

This file will be loaded at the start of every Claude session, giving Claude persistent memory of your project's unique architecture and constraints.