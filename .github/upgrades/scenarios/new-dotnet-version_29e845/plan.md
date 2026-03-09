# .NET Version Upgrade Plan

- Repository: `D:\Development\Projects\Desktop Apps\DownloadSorter`
- Plan path: `.github/upgrades/scenarios/new-dotnet-version_29e845/plan.md`

Table of Contents
- Executive Summary
- Migration Strategy
- Detailed Dependency Analysis
- Project-by-Project Plans
- Package Update Reference
- Breaking Changes Catalog
- Testing & Validation Strategy
- Risk Management
- Complexity & Effort Assessment
- Source Control Strategy
- Success Criteria

---

## Executive Summary

Scenario: Upgrade solution to .NET 10.0 (`net10.0`) with Windows desktop support for WinForms projects.

High-level metrics (from assessment):
- Total projects: 3 (all require framework update)
- Total NuGet packages: 6 (2 recommended for upgrade)
- Total lines of code: 713
- Projects: `Download Sorter UI` (WinForms), `DownloadSorter` (core app), `DownloadSorterTests` (test project)

Selected Strategy
- **All-At-Once Strategy** — All projects will be upgraded simultaneously in a single atomic operation.
- Rationale: Small solution (3 projects), shallow dependency depth, cohesive codebase, limited package complexity. Assessment shows recommended package updates are available and WinForms is supported on `net10.0-windows` when desktop support is enabled.

Key constraints and inputs
- Source branch: `master`
- Upgrade target branch: `upgrade-to-NET10`
- Pending changes: `commit` (as detected during initialization)
- Target frameworks proposed in assessment: `net10.0-windows` (UI), `net10.0-windows` (others where appropriate)

Critical issues flagged in assessment
- Significant API compatibility findings for WinForms APIs (NotifyIcon, ToolStripMenuItem, ToolStrip, etc.) — 108 binary-incompatible APIs flagged in UI project; plan accounts for expected code fixes discovered during compilation.
- Two NuGet packages recommended for update (see Package Update Reference).

---

## Migration Strategy

Approach: All-At-Once (atomic)
- Update TargetFramework for all projects to `net10.0` and enable Windows desktop support for WinForms project(s).
- Update all package references listed in the assessment to suggested versions.
- Restore and build the entire solution; identify and fix compilation errors caused by framework and package upgrades in the single operation.
- Run test projects and address test failures after build success.

Phases (logical, not sequential task splitting):
- Phase 0: Preparation — commit pending changes, create branch `upgrade-to-NET10`, ensure SDK availability (developer action).
- Phase 1: Atomic Upgrade — change project target frameworks and package versions for all projects in one coordinated change set.
- Phase 2: Build & Fix — restore, build, resolve compilation errors arising from the upgrade.
- Phase 3: Testing & Validation — run unit tests and any automated tests; verify no vulnerabilities remain.

Notes on Windows Desktop support
- UI project is WinForms and must target `net10.0-windows` and enable Windows desktop support. This can be achieved by setting project `TargetFramework` to `net10.0-windows` and ensuring `<UseWindowsForms>true</UseWindowsForms>` or equivalent SDK support is present. Document this in Project-by-Project Plans.

Source control guidance (summary):
- Perform the prerequisite commit of pending changes on `master` (or as configured), then create and switch to branch `upgrade-to-NET10`. All atomic changes should be produced in a single commit (or a small set of commits grouped logically) and pushed to the branch for PR/review.

---

## Detailed Dependency Analysis

Dependency summary (from assessment):
- `DownloadSorter.csproj` is a library/app used by `DownloadSorterTests`.
- `Download Sorter UI` is a WinForms app and is independent of project references in this solution (no project dependencies).
- Graph depth is shallow (leaves: `Download Sorter UI`, `DownloadSorter`).

Dependency-based considerations for All-At-Once:
- Although dependency-first rules normally require migrating leaves first, All-At-Once mandates updating all projects in one atomic pass. The plan will still respect dependency order when resolving build issues during fix pass but not split the upgrade into per-project commits.

Critical path
- The critical path runs through building the UI project (many API incompatibilities flagged). Fixes discovered during build will affect both UI and potentially core projects.

Circular dependencies
- None discovered.

---

## Project-by-Project Plans

Principles: For each project the plan lists current and target state, project-file changes, package updates, expected break categories, code areas to inspect, and validation checklist.

### Project: `Download Sorter UI` (`Download Sorter UI.csproj`)
Current state
- Current TargetFramework: `net7.0-windows7.0`
- Proposed TargetFramework: `net10.0-windows`
- SDK-style: True
- Project type: WinForms

Planned project-file changes (what to prepare for executor)
1. Update `<TargetFramework>` to `net10.0-windows`.
2. Ensure Windows Forms support is enabled: set `<UseWindowsForms>true</UseWindowsForms>` if not present.
3. Verify MSBuild import patterns (Directory.Build.* or Directory.Packages) that may affect package versions.

Package updates (see consolidated table); apply those recommended for this project.

Expected breaking-change areas (from assessment)
- `NotifyIcon` properties/methods (BalloonTipTitle, BalloonTipText, ShowBalloonTip, Icon, Visible)
- `ToolStripMenuItem`/`ToolStrip` usage and event wiring
- `Form` properties: `ShowInTaskbar`, `WindowState`, `Text`, `ClientSize`
- `Application` high-DPI settings and startup calls (SetHighDpiMode, EnableVisualStyles, SetCompatibleTextRenderingDefault)

Code changes to plan for (examples)
- Replace/adjust uses of `NotifyIcon.ShowBalloonTip(int)` if overloads or behavior changed. Validate `System.Drawing.Icon` usage — may require ensuring `System.Drawing.Common` usage is supported on Windows.
- Confirm any designer-generated code is compatible; Windows Forms designer code may require migration adjustments.
- Re-evaluate direct file IO paths (no change expected but confirm). Keep exception handling as-is; consider logging improvements outside this plan.

Validation checklist
- [ ] Project file updated to `net10.0-windows` and `UseWindowsForms` enabled
- [ ] Project restores packages successfully
- [ ] Project builds without errors
- [ ] Key UI functionality compiles (NotifyIcon, ToolStrip, menu items)
- [ ] No outstanding package vulnerabilities for packages used by this project

---

### Project: `DownloadSorter` (`DownloadSorter.csproj`)
Current state
- Current TargetFramework: `net7.0-windows`
- Proposed TargetFramework: `net10.0-windows`
- SDK-style: True

Planned project-file changes
1. Update `<TargetFramework>` to `net10.0-windows` (or `net10.0` with `UseWindows` settings consistent with solution needs).
2. Update package references to suggested versions.

Expected break areas
- Minor API adjustments; assessment flagged no binary-incompatible APIs in this project but recommended package updates (including `Newtonsoft.Json`) should be applied.

Validation checklist
- [ ] TargetFramework updated
- [ ] Packages restored at target versions
- [ ] Project builds without errors

---

### Project: `DownloadSorterTests` (`DownloadSorterTests.csproj`)
Current state
- Current TargetFramework: `net7.0-windows`
- Proposed TargetFramework: `net10.0-windows`
- SDK-style: True

Planned project-file changes
1. Update `<TargetFramework>` to `net10.0-windows`.
2. Ensure test SDK and test adapters are compatible; update package references if necessary.

Validation checklist
- [ ] TargetFramework updated
- [ ] Tests run and pass (see Testing & Validation section)

---

## Package Update Reference

Apply the package updates discovered in assessment. Do not skip packages marked as "Suggested Version" in assessment.

| Package | Current Version | Suggested Version | Projects Affected | Notes |
|---|---:|---:|---|---|
| `Microsoft.Extensions.Hosting.WindowsServices` | 7.0.1 | 10.0.3 | `DownloadSorter.csproj` | Upgrade recommended to align hosting APIs with .NET 10 runtime.
| `Newtonsoft.Json` | 13.0.3 | 13.0.4 | `DownloadSorter.csproj` | Minor update recommended.
| `coverlet.collector` | 6.0.0 | (no change) | `DownloadSorterTests.csproj` | Compatible
| `Microsoft.NET.Test.Sdk` | 17.6.0 | (no change) | `DownloadSorterTests.csproj` | Compatible
| `MSTest.TestAdapter` | 3.0.4 | (no change) | `DownloadSorterTests.csproj` | Compatible
| `MSTest.TestFramework` | 3.0.4 | (no change) | `DownloadSorterTests.csproj` | Compatible

Notes
- Apply the package updates as part of the atomic upgrade. If package centralization (Directory.Packages.props) exists, update there and not per-project.
- Security vulnerabilities: none flagged as critical in the assessment output beyond recommended upgrades. If additional advisories appear during restore, address them during the fix pass.

---

## Breaking Changes Catalog (expected and prioritized)

This catalog highlights APIs flagged in the assessment for the UI project. Expect to discover concrete compile-time errors in these areas during the Build & Fix pass.

Primary areas (high priority):
- `System.Windows.Forms.NotifyIcon` — properties `BalloonTipTitle`, `BalloonTipText`, `Icon`, `Visible`, method `ShowBalloonTip(int)`.
- `System.Windows.Forms.ToolStripMenuItem` and related `ToolStrip` items — constructors, `Click` events, `Text`, `Name`, `Size`.
- `System.Windows.Forms.Application` — `SetHighDpiMode`, `SetCompatibleTextRenderingDefault`, `EnableVisualStyles`, `Run` behavior.
- Designer-generated form constructs — ensure `InitializeComponent` and designer partial classes remain compatible.

Secondary areas (medium priority):
- `System.Drawing.Icon` usage — check constructor overloads and disposal patterns.
- `Form` properties like `ShowInTaskbar`, `WindowState`, `ClientSize`, `Text` — verify designer code compiles.

For each compile error encountered during the atomic build, document the error and map to one of these categories for prioritized fixes.

---

## Testing & Validation Strategy

Testing levels
1. Project-level build verification: ensure each project restores and builds cleanly.
2. Unit tests: run `DownloadSorterTests` after build success; address failing tests.
3. Integration/smoke checks (developer responsibility): validate basic UI startup and tray/notify flows manually if automated tests are not present.

Automated test run list
- `DownloadSorterTests` (MSTest) — execute and report pass/fail.

Validation checklist (solution-level)
- [ ] Solution restores and builds successfully with `net10.0` targets and updated packages
- [ ] All unit tests pass in CI or local run
- [ ] No outstanding package vulnerabilities reported by restore tools

---

## Risk Management

Risk summary per project
- `Download Sorter UI` — High risk: large number of API incompatibilities flagged (108 binary-incompatible). Mitigation: allocate build-and-fix pass focused on UI, verify designer code, and verify Windows Forms runtime behavior.
- `DownloadSorter` — Low risk: package updates recommended; no API breaks flagged.
- `DownloadSorterTests` — Low risk: update to test SDK versions verified as compatible.

Mitigations
- Run the atomic upgrade in a feature branch (`upgrade-to-NET10`) with one consolidated commit for easier review and rollback.
- Keep a backup of current branch (commit pending changes) before switching.
- Use feature branch PR review and CI build to detect regressions early.
- If UI breakage is extensive, consider branching a follow-up task to fix UI-specific issues while keeping core projects upgraded.

Rollback plan (high-level)
- If the atomic upgrade introduces blocking regressions, revert the upgrade commit on `upgrade-to-NET10` and investigate fixes in a separate branch; do not merge until fixes are validated.

---

## Complexity & Effort Assessment

Relative complexity ratings (no time estimates):
- `Download Sorter UI`: High complexity (many API incompatibilities flagged; designer and UI code requires careful attention).
- `DownloadSorter`: Low complexity.
- `DownloadSorterTests`: Low complexity.

Implication: The largest portion of effort during the atomic fix pass will be in the UI project.

---

## Source Control Strategy

Required preparatory steps (to be performed by executor before applying changes):
1. Ensure workspace is on `master` and commit pending changes (default action: `commit`).
2. Create and switch to branch: `upgrade-to-NET10` (as returned by initialization).
3. Apply all project file and package updates in this branch as a single atomic change set.
4. Push branch and open PR for review. Include test results and build logs.

Commit guidance
- Use a single commit (or a minimal set of logically grouped commits) for the atomic upgrade. Commit message should reference: "Atomic upgrade to .NET 10.0 — update TargetFramework and package versions (All-At-Once)".
- Keep designer-generated code changes in the same commit to avoid mismatches.

Review gates
- Require CI build success and unit test pass before merging.

---

## Success Criteria

The migration is considered complete when all technical success criteria below are met:
- All projects updated to their proposed `net10.0` target frameworks as specified in the plan.
- All package updates from the Package Update Reference applied.
- Solution restores and builds with 0 compilation errors.
- Unit tests (`DownloadSorterTests`) pass.
- No remaining security vulnerabilities flagged by package audit tools.

Operational criteria
- Upgrade changes are present in branch `upgrade-to-NET10` and PR is created.
- All merge checks (CI/build/tests) are green before merging.

---

## Notes, Assumptions & Open Questions

- Assumption: Developer machine(s) and CI have .NET 10 SDK installed before execution. If not, add prerequisite to Phase 0.
- Assumption: Windows Forms support via `net10.0-windows` is acceptable and the Windows Desktop runtime will be available in target execution environment.
- Open: Confirm whether the solution uses centralized package management (Directory.Packages.props). If so, update package versions there.
- Open: Confirm desired branch naming and pending-change handling; current defaults from initialization are `master`, `upgrade-to-NET10`, and `commit`.


<!-- End of plan -->