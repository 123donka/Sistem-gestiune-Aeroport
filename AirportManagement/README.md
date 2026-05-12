# AirportManagement (WinForms)

Proiect WinForms skeleton pentru gestiunea unui aeroport (autentificare, dashboard, CRUD, MySQL).

Setup:
- Instalează .NET SDK compatibil (target: net10.0-windows).
- Configurează MySQL și creează baza `airportdb` cu tabelele folosite (ex: `utilizatori`, `zboruri`, `pasageri`, `resurse`, `alerte`, `logactivitati`).
- Actualizează `appsettings.json` cu string-ul tău de conexiune MySQL.

Build & Run:
```
dotnet restore AirportManagement
dotnet build AirportManagement
dotnet run --project AirportManagement
```

Notă: imaginea `Resources/logo.png` este un placeholder; adaugă un logo real în folderul `AirportManagement/Resources`.
