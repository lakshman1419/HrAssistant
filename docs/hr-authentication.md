# HR Assistant Authentication

The login endpoint verifies exactly two configured demo users in process memory: one `Admin` and one `Employee`. It is a local proof of concept, not production authentication. It has no identity provider, durable user store, password hashing policy, MFA, tokens, or server-enforced authorization. The Angular route guard is only a navigation convenience and is not a security boundary. Do not use this implementation for deployed HR data.

## Configure Local Accounts

The tracked `appsettings.json` values are placeholders and are not usable passwords. In the `HrAssistant` project directory, set local overrides with .NET User Secrets:

```powershell
dotnet user-secrets set "Authentication:Users:0:Username" "your-admin-username"
dotnet user-secrets set "Authentication:Users:0:Password" "your-local-admin-password"
dotnet user-secrets set "Authentication:Users:0:DisplayName" "Admin"
dotnet user-secrets set "Authentication:Users:1:Username" "your-employee-username"
dotnet user-secrets set "Authentication:Users:1:Password" "your-local-employee-password"
dotnet user-secrets set "Authentication:Users:1:DisplayName" "Employee"
```

The roles are fixed as `Admin` at index `0` and `Employee` at index `1`; the API rejects configuration unless there is exactly one of each and the usernames are distinct. Keep local passwords out of source control and logs. The service loads configuration into memory at startup; changing a configured account requires restarting the API.

## Run

From the repository root, start the API and client in separate terminals:

```powershell
dotnet run --project HrAssistant/HrAssistant.csproj --launch-profile http
```

```powershell
Push-Location ClientApp
npm start
Pop-Location
```

Open `http://localhost:4200`. Successful login returns only the configured username, display name, and role. The client stores that profile in `sessionStorage`; it never stores the password. Logout clears the client-side demo session. Neither the stored role nor a client route guard authorizes calls to the HR API.