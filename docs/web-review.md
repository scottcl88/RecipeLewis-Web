# RecipeLewis-Web code review (2026-11-08)

Scope: Blazor WASM client, Services/, Helpers/, Models/, amplify.yml. Static read; app not run, no tests exist. Reviewer: AI assistant (Chief of Staff subagent).

## Security (priority order)
1. **HIGH – JWT + refresh token stored in browser localStorage** (`AuthenticationService`, `ILocalStorageService`). Readable by any XSS; refresh token is long-lived. Prefer httpOnly secure cookies issued by the API (needs API change) or at minimum shorten refresh lifetime/rotate and keep token in memory.
2. **HIGH – XSS via `MarkupString`** (`ViewRecipe.razor` 109/115 render `IngredientsHTML`/`DirectionsHTML`). Safety depends entirely on API-side sanitization (API uses HtmlSanitizer). Add client-side defense or confirm sanitizer on every write path; add CSP header in Amplify (customHttp).
3. **MEDIUM – AutoMapper 12.0.1 has known high-severity vuln (NU1903, GHSA-rvv3-g6hj-g44x).** Fix requires AutoMapper major upgrade (16.x, license/API changes) – not done in this PR; recommend a separate PR.
4. **MEDIUM – `amplify.yml` pipes an unpinned dotnet-install script (`-Channel LTS`)** and "curl -ssl" (not a valid TLS flag; use `-sSL`). LTS channel will move to .NET 10 and break the net8.0 build; pin `--channel 8.0`.
5. **LOW – Client logs** (`Console.WriteLine` in HttpService/AuthenticationService) leak flow info to the console; remove.
6. **LOW – Server error messages** thrown verbatim to UI (`throw new Exception(error["message"])`).
7. Hygiene: five near-identical appsettings.*.json files; only apiUrl is present (no secrets found in them). Confirm none hold secrets before adding config.

## Correctness
1. **Refresh loop** (`RefreshTokenLoop`): fire-and-forget `Task.Run`, unobserved exceptions; refreshes immediately on start (redundant after login); `Task.Delay(diff)` throws if diff is negative; `_authUser.User.Token` may be null after logout (NRE); `_cts` can be null in `RefreshToken` failure path; `_refreshTokenLoopStarted` never reset on logout, so re-login does not restart the loop.
2. **`HttpService.sendRequest`**: returns `default` on 401 (callers get null → NRE); error body parse assumes JSON dict with "message" (throws KeyNotFound/JsonException otherwise); reads localStorage on every request.
3. **Login password-visibility toggle was a no-op** (InputType hardcoded) – fixed in this PR.
4. Possible-null warnings (49 remain; see below) in Admin (Promote/Demote with null userId), Profile/ResetPassword/Register message handling, Recipes lambdas, AuthenticationService.
5. `AddEditRecipe.razor` line ~510 compares non-nullable bool to null (always false – dead branch).
6. Un-awaited call in MainLayout (CS4014) and async methods without await (CS1998, 8 places).

## Maintainability
1. No tests; no CI build in PRs (only Amplify build on deploy). Add a GitHub Actions `dotnet build` at least.
2. Page components are large with logic in `@code`; move to services/view-models. Repeated try/catch/message boilerplate across Login/Register/Profile/Reset/Verify.
3. Nullable reference types enabled but ~40 CS8618 warnings ignored; mark fields nullable or initialize.
4. Outdated packages not touched here: MudBlazor 6.11 (→ 9.x, breaking), AutoMapper 12 (vuln), Azure.Identity (appears unused in WASM client – verify and remove), AutoMapper.Extensions.Microsoft.DependencyInjection (deprecated), Toolbelt.Blazor.PWA.Updater. Target framework net8.0 (LTS ends Nov 2026) – plan .NET 10.
5. `HttpService` takes an unused `IConfiguration`; PostForm has debug logging.
