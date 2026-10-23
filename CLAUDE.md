# CLAUDE.md

QREST (Quality Review and Exchange System for Tribes) is a tribal air-quality data platform. It polls data loggers, imports and averages n-minute/hourly data, runs validation and alerts, supports multi-phase data review and QC, and submits to EPA AQS (via the Exchange Network/CDX) and AirNow. See `README.md` for features and the change log. Recent changes go in the README "Change Log" section; older ones are in `CHANGELOG.md`.

## Tech stack

- **.NET Framework 4.8**, C#, old-style (non-SDK) `.csproj` files with `packages.config` NuGet. The `packages/` folder sits at the solution root.
- **ASP.NET MVC 5.3** with Razor views, plus **Web API 2** for the public API. Hosted on IIS / System.Web, with OWIN only for auth.
- **ASP.NET Identity 2 + OWIN cookie auth** (`App_Start/Startup.Auth.cs`, `IdentityConfig.cs`, `Models/IdentityModels.cs`).
- **Entity Framework 6.5 (Database-First, EDMX)** against **SQL Server** (SQL Express locally). `EntityFramework.BulkInsert` handles bulk loads.
- Front end: **Bootstrap 3.4**, **jQuery 3.7**, DataTables, toastr, select2, CanvasJS, cleave, dirtyforms, TinyMCE, Font Awesome. Assets are bundled with `System.Web.Optimization` in `App_Start/BundleConfig.cs`. There is no npm/webpack build. JS libraries are checked in under `QREST/Scripts` and `QREST/wwwroot/lib`.
- Other libraries: ClosedXML/OpenXml (Excel), iTextSharp (PDF), SSH.NET (SFTP to AirNow), HtmlAgilityPack, UnitsNet, Newtonsoft.Json, Resend for email (`BLL/UtilsEmail.cs`), and SOAP web references to the EPA CDX node.

## Solution layout (`QREST.sln`)

| Project | Type | Purpose |
|---|---|---|
| `QREST/` | ASP.NET MVC web app | UI, Web API, auth. References `QRESTModel`. |
| `QRESTModel/` | Class library | All data access (EF model + `db_*` classes), business logic, logger communications. Shared by every other project. |
| `QREST_Service/` | Windows Service (`WinExe`) | Background task runner (logger polling, imports, hourly validation, AirNow push). |

### QREST (web)
- `Controllers/`: one controller per functional area (`Site`, `Data`, `Admin`, `Reports`, `Dashboard`, `Account`, `Training`, …). Large files (e.g. `DataController`, `SiteController`) are split into sections with `//*****` banner comments.
- `Controllers/apiController.cs`: `QRESTapiController : ApiController` at `api/{controller}/{action}/{id}`. It authenticates with `Authorization: Bearer <api_key>`, which is matched to `T_QREST_ORG_USERS` via `db_Account.GetT_QREST_ORG_USERS_ByAPIKey`.
- `Models/*ViewModels.cs`: one file per controller. View model classes are named `vm<Controller><Action>` (e.g. `vmSiteOrgEdit`). `ddlHelpers.cs` builds `SelectList`s for dropdowns.
- `App_Logic/`: web-only helpers (Excel/PDF generation, HTML helpers, `GetIP`).
- `App_Start/`: routing, bundles, filters, Identity config, and a custom `QREST.AuthorizeAttribute`, which sends authenticated-but-unauthorized users to `Error/AccessDenied`.
- `Views/<Controller>/`: Razor views. Shared layouts are `_Layout`, `_LayoutPub` (public site), `_LayoutLogin` and `_LayoutHelp`. `_Alerts` shows `TempData` messages.
- `Global.asax.cs`: global `Application_Error` logs to `T_QREST_SYS_LOG` and renders through `ErrorController`.
- `Web.config` plus transforms `Web.Debug/Release/ReleaseTest.config`. The `q-env` app setting marks the environment.

### QRESTModel (data + logic)
- `DAL/QRESTModel.edmx`: the EF Database-First model. `QRESTModel.tt` / `QRESTModel.Context.tt` **generate** the `T_QREST_*.cs` entity classes, the view classes (`MONITOR_SNAPSHOT`, `SITE_HEALTH`, …), the `SP_*_Result.cs` stored-procedure result types, and `QRESTEntities` (`QRESTModel.Context.cs`). **Never hand-edit generated files.** Change the database, then run "Update Model from Database" in the EDMX designer, which re-runs the T4 templates.
- `DAL/db_Account.cs`, `db_Air.cs`, `db_Ref.cs`, `db_Train.cs`: hand-written static data-access classes, grouped by domain (users/orgs, air data/sites/monitors/imports/QC, reference data/settings/logs/tasks, training). DTO/"display" types such as `SiteDisplay` and `SitePollingConfigType` are declared at the top of these files.
- `DAL/logEF.cs`: `logEF.LogEFException(ex)` writes the innermost error (or EF validation errors) to `T_QREST_SYS_LOG`, tagged with the caller name.
- `BLL/`: AQS submission (`AQSHelper`), import parsing, unit conversion, crypto (`SimpleAES`), email, SMS, notifications, zip, DataTable generation, and `EFHelperAsync`, which adds a `ToListAsync` extension for EF stored-procedure `ObjectResult`s.
- `COMM/LoggerComm.cs`: communications with data loggers (ZENO, SUTRON, Campbell/LoggerNet, ESC, MetOne BAM).
- `App_Data/1_…5_*.sql`: database creation scripts, run in numbered order. Also `QREST_index_maintenance.txt`.
- `Web References/net.epacdxnode.testngn`: generated SOAP proxy for the EPA CDX node.

### QREST_Service (background tasks)
- `QRESTService.OnTimer` fires every 60 s and reads due tasks from `T_QREST_APP_TASKS` (`db_Ref.GetT_VCCB_TASKS_ReadyToRun`). For each task it reflects `Type.GetType("QRESTServiceCatalog." + TASK_NAME)` and calls its public `RunService()` method.
- Task classes (`TaskLoggerPolling`, `TaskImport`, `TaskHourlyValidation`, `TaskAirNow`) live in namespace **`QRESTServiceCatalog`**, unlike the rest of the project, which uses `QREST_Service`. To add a task: create a class in that namespace with a `RunService()` method and add a row to `T_QREST_APP_TASKS` whose `TASK_NAME` equals the class name.
- Logs go to flat files under the exe directory (`Logs/`, `Polls/`, `AirNow/`) through `General.WriteToFile` etc.

## Database / data access

- SQL Server database `QREST`. There are two connection strings, both in `QREST/Web.config` (and `QREST_Service/App.config` for the service):
  - `QRESTEntities`: the EF EDMX connection (metadata `res://*/DAL.QRESTModel.*`), used by all `db_*` code.
  - `IdentityConnection`: a plain connection used by ASP.NET Identity's `ApplicationDbContext`. The Identity tables are remapped to `T_QREST_USERS`, `T_QREST_ROLES`, `T_QREST_USER_ROLES`, `T_QREST_USER_LOGINS` and `T_QREST_USER_CLAIMS`.
- Table naming: `T_QREST_<AREA>_<NAME>`, with reference tables `T_QREST_REF_*`. Column conventions: `*_IDX` for keys (mostly `Guid`; some `int` identities), `*_ID` for business identifiers, `*_IND` for boolean flags, `*_DT` / `*_DTTM` for dates (`DATA_DTTM_LOCAL` / `DATA_DTTM_UTC`), `*_CD` for codes, and `CREATE_USER_IDX` / `CREATE_DT` / `MODIFY_USER_IDX` / `MODIFY_DT` for auditing.
- **Data-access pattern.** Every method is `public static` on a `db_*` class, opens its own short-lived context, and swallows exceptions after logging:
  ```csharp
  public static T_QREST_SITES GetT_QREST_SITES_ByID(Guid siteIdx)
  {
      using (QRESTEntities ctx = new QRESTEntities())
      {
          try
          {
              return (from a in ctx.T_QREST_SITES where a.SITE_IDX == siteIdx select a).FirstOrDefault();
          }
          catch (Exception ex)
          {
              logEF.LogEFException(ex);
              return null;   // or 0 / false / empty, depending on the return type
          }
      }
  }
  ```
  - Method names follow `Get<Table>_By<Field>`, `Get<Table>_<qualifier>`, `InsertUpdate<Table>`, `Delete<Table>` and `Create<Table>` (logs).
  - `InsertUpdate*` methods take one parameter per column (often many; pass `null` to leave a field unchanged). They look up the record by key, `new` it if it is missing (`insInd = true`), assign only non-null arguments, set the audit fields, call `SaveChanges()`, and return the key (or `0`/`null` on failure). Parameters are named after the column with the first letter lowercased (`sETTING_IDX`, `oRG_ID`). This is deliberate, and IDE1006 naming warnings are disabled in `.editorconfig`.
  - Callers check the return value (`if (succId == 1)`, `!= null`) rather than catching exceptions.
  - Queries mostly use LINQ query syntax (`from … where … select …`). Projections into display DTOs are common.
  - Stored procedures are EDMX function imports called as `ctx.SP_*()`. Long-running ones raise `ctx.Database.CommandTimeout` (600–900 s). Async reports use `await ctx.SP_X(...).ToListAsync()` through `EFHelperAsync`.
  - Bulk imports: `ctx.BulkInsert<T_QREST_DATA_IMPORT_TEMP>(list)` into a temp table, then `ExecuteSqlCommand` / SPs for dupe detection and the move into `T_QREST_DATA_HOURLY` / `T_QREST_DATA_FIVE_MIN`.
  - Raw SQL uses `ctx.Database.ExecuteSqlCommand("... {0}", arg)`. **Use the `{0}` parameter placeholders**, not string concatenation. A few existing import statements concatenate a `Guid`; do not extend that pattern to strings.
- App settings live in `T_QREST_APP_SETTINGS` and are read with `db_Ref.GetT_QREST_APP_SETTING("NAME")`. Values flagged `ENCRYPT_IND` are transparently AES-decrypted via `SimpleAES`. Runtime config such as email servers, AQS/CDX credentials and AirNow credentials comes from here, not from `Web.config`.
- User activity is logged with `db_Ref.CreateT_QREST_SYS_LOG_ACTIVITY(...)` and errors with `db_Ref.CreateT_QREST_SYS_LOG(...)`.

## Building and running

- Requires Windows, **Visual Studio 2022** (the solution targets VS 17), the .NET Framework 4.8 developer pack, and SQL Server / SQL Express. `msbuild` is not on PATH by default, so use a VS Developer PowerShell. `git` is also not on PATH in the default shell.
- Solution configurations: `Debug`, `Release` and `ReleaseTest`. `ReleaseTest` maps to the test site and `Web.ReleaseTest.config`.
- Build from a Developer PowerShell:
  ```powershell
  nuget restore QREST.sln          # or let VS restore packages.config
  msbuild QREST.sln /p:Configuration=Debug
  ```
- Run the web app with IIS Express from VS (start project `QREST`). Deployment uses VS Web Deploy publish profiles in `QREST/Properties/PublishProfiles` (`qrest`, `qrest-test`).
- Windows service: build `QREST_Service`, then install with `installutil QREST_Service.exe` (`ProjectInstaller`). There is also a ClickOnce `publish/` folder.
- New database setup (`QREST/Content/Docs/DeployGuide.txt`): run `QRESTModel/App_Data/1_createDatabase.sql` through `5_populateDataLocal.sql` in order, then point the connection strings at the database. The first registered account becomes the global admin. Then import AQS reference data under Admin → Import Ref Data.
- There are no automated tests. To verify a change, build and exercise the app manually.

## Coding conventions

- Thin-ish controllers call static `db_*` methods directly. There is no repository or service layer, no DI container, and no interfaces. Follow that style instead of introducing new abstractions.
- Authorization:
  - Use `[Authorize]` at the class level. `[Authorize(Roles = "GLOBAL ADMIN")]` guards Admin and some Reports actions.
  - Org-level permissions are checked in code with `db_Account` helpers such as `IsAnOrgAdmin(UserIDX, orgId)`. On failure, set `TempData["Error"]` and redirect to `Dashboard/Index`.
  - Get the current user with `User.Identity.GetUserId()` (`string UserIDX`).
- POST form actions use `[HttpPost, ValidateAntiForgeryToken]`, check `ModelState.IsValid`, set `TempData["Success"]` / `TempData["Error"]`, and either repopulate the dropdowns and return `View(model)` or `RedirectToAction`.
- Grids: the view uses jQuery DataTables with an `ajax` POST to a `<Name>Data` action, which returns `Json(new { draw, recordsFiltered, recordsTotal, data })`. Delete endpoints are `[HttpPost] JsonResult` actions that return `Json("Success")` or an error string, and they pair with `confirm-delete.js`.
- Views set `ViewBag.Title`, `ViewBag.SubTitle` and `ViewBag.HelpLink`, use Bootstrap 3 `panel panel-qrest` sections, and put page JS in `@section Scripts { @Scripts.Render("~/bundles/...") <script>…</script> }`. Register new JS/CSS libraries in `BundleConfig.cs`.
- Naming:
  - Entity and property names mirror the uppercase DB columns (`ORG_ID`, `SITE_IDX`), and view-model properties often do too.
  - Local variables for fetched entities are prefixed with `_` (`_org`, `_site`, `_tasks`).
  - View-model dropdown properties are named `ddl_*`.
- Comments: sections are separated by repeated `//*****…NAME…*****` banners, there are brief `//` notes, and `/// <summary>` appears on non-obvious methods. Commented-out legacy code is common, so leave existing blocks alone unless you are cleaning them up on purpose.
- Use `System.DateTime.Now` for audit timestamps. Air data stores both local and UTC timestamps plus a site time-zone offset, so keep both in sync when you write data.
- Braces and indentation are 4-space Allman style. Single-statement `if`/`else` bodies often omit braces.
- When you add a user-visible feature or fix, add a dated entry at the top of the README "Change Log" in the existing bold-heading bullet style.
