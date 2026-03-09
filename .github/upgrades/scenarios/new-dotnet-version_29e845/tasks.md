# DownloadSorter .NET 10.0 Upgrade Tasks

## Overview

This document tracks execution of the solution upgrade to .NET 10.0. All project files and package references will be updated in a single coordinated change, followed by build fixes and automated test validation.

**Progress**: 1/3 tasks complete (33%) ![0%](https://progress-bar.xyz/33)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-09 12:20)*
**References**: Plan §Phase 0, Plan §Notes, Assumptions & Open Questions

- [✓] (1) Verify .NET 10 SDK is installed on the machine per Plan §Phase 0
- [✓] (2) Runtime/SDK version meets minimum requirements (**Verify**)
- [✓] (3) Check for presence of `global.json` and verify it does not pin an SDK older than .NET 10 (Plan §Phase 0)
- [✓] (4) Detect centralized package management (e.g., `Directory.Packages.props`) presence and confirm update approach per Plan §Package Update Reference (**Verify**)

### [▶] TASK-002: Atomic framework and package upgrade with compilation fixes
**References**: Plan §Migration Strategy, Plan §Project-by-Project Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update `TargetFramework` for all projects to `net10.0-windows` (per Project-by-Project Plans) and enable Windows Forms support (`<UseWindowsForms>true</UseWindowsForms>`) for the UI project
- [ ] (2) Update package references to the suggested versions per Plan §Package Update Reference (if centralized package management exists, apply updates there) 
- [ ] (3) Restore dependencies (dotnet restore) for the solution
- [ ] (4) Build the solution and fix all compilation errors caused by framework/package changes (reference Plan §Breaking Changes Catalog for UI-specific changes)
- [ ] (5) Solution builds with 0 errors (**Verify**)
- [ ] (6) Commit changes with message: "TASK-002: Atomic upgrade to .NET 10.0 — update TargetFramework and package versions"

### [ ] TASK-003: Run test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy, Plan §Project-by-Project Plans, Plan §Breaking Changes Catalog

- [ ] (1) Run tests in `DownloadSorterTests` project per Plan §Testing & Validation Strategy
- [ ] (2) Fix any failing tests (reference Plan §Breaking Changes Catalog for common issues)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)
- [ ] (5) Commit test fixes with message: "TASK-003: Complete testing and validation"
