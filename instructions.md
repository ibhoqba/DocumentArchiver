# Document Archiver - Claude Project Memory

## 🧭 Purpose

This is a Windows desktop application for document scanning, OCR, PDF generation, and archival into a database.

Claude MUST prioritize:
- correctness over assumptions
- reading existing code before modifying behavior
- preserving current workflows

---

## 🏗️ Architecture Overview (IMPORTANT: VERIFY BEFORE USE)

The project is expected to include:

- Scanner integration ( TWAIN via NTwain)
- OCR processing ( Tesseract)
- PDF generation
- Database persistence ( SQL Server)
- WinForms UI

⚠️ Claude MUST NOT assume exact folder structure.
Always inspect actual files before referencing paths.

---

## 🔄 Scanning System Behavior

### Key Constraints

- TWAIN libraries (e.g., NTwain) are typically:
  - synchronous APIs
  - event-driven for image transfer

### HOWEVER:

Claude MUST verify in code:

- Is scanning wrapped in `Task`, `Thread`, or `async` methods?
- Is there a service layer abstracting scanner logic?
- Are events marshaled to UI thread?

### Rule

❗ DO NOT assume scanning is purely synchronous  
❗ DO NOT remove async wrappers without checking call chain  

---

## 🧵 Threading & UI Rules

- WinForms requires UI updates on main thread
- Look for:
  - `Invoke()`
  - `BeginInvoke()`

Before modifying:
- verify how background work is handled
- avoid introducing cross-thread exceptions

---

## 📂 File & Folder Handling

Claude MUST verify:

- Where scanned images are stored (temp vs final)
- When folders are created:
  - before scanning?
  - during processing?
- Naming conventions (GUID, sequence, metadata-based)

❗ Do not enforce folder rules unless confirmed in code

---

## 🧾 Document Processing Pipeline

Typical expected flow (VERIFY BEFORE MODIFYING):

1. Scan images (front/back)
2. Store temporarily
3. Run OCR
4. Generate PDF
5. Save to final storage path
6. Insert database record

Claude MUST confirm:
- actual order in code
- error handling points
- retry logic (if any)

---

## 🗄️ Database Layer

Claude MUST NOT assume schema.

Before making changes:
- locate database service/repository
- identify:
  - table names
  - required fields
  - transaction behavior

Check for:
- async DB calls
- error handling
- connection lifecycle

---

## 🧠 OCR System

Likely uses Tesseract or similar.

Claude MUST verify:
- language configuration
- image preprocessing steps
- where OCR results are stored

---

## ⚙️ Configuration & Settings

Check for:
- persisted user settings (branch, DPI, scanner)
- config files (JSON, XML, appsettings)
- runtime overrides

---

## 🚫 Critical Safety Rules

Claude MUST:

- NEVER rewrite scanning logic blindly
- NEVER assume method names exist
- NEVER enforce architecture from documentation alone
- ALWAYS read actual implementation before suggesting fixes

---

## ✅ Modification Workflow (MANDATORY)

Before any code change:

1. Locate relevant files
2. Trace execution flow
3. Identify dependencies
4. Confirm threading model
5. Apply minimal change

---

## 🧪 Validation Checklist

After any modification, ensure:

- scanning still triggers correctly
- no UI freezes or cross-thread errors
- images are still saved
- PDF generation still works
- DB insert still executes
- no silent failures introduced

---

## 📌 Notes for Claude

If something is unclear:

- ask for the file
- or request a code snippet

Do NOT guess.

---

## 🔚 Principle

This codebase is **event-driven + IO-heavy (scanner, OCR, DB)**.

Incorrect assumptions will break:
- scanning flow
- threading
- data integrity

Precision is required.