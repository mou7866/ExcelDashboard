# Excel Dashboard

Upload `.xlsx` files, store the data, and view it on a dashboard with a paginated table and chart.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) (for the Angular UI)
- [PostgreSQL](https://www.postgresql.org/) (or use the default connection below)

## Excel format

- **Row 1:** Header (e.g. Category, Amount, Date)
- **From row 2:** Data
  - **Column A:** Category (text)
  - **Column B:** Amount (number)
  - **Column C:** Date (date)

A sample file is included: `sample-data.xlsx`. You can also generate one with:

```bash
cd tools && dotnet run
```

## Database

Default connection string (override with config or env):

```
Host=localhost;Port=5432;Database=ExcelDashboard;Username=excel;Password=excel
```

Create the database and user if needed, then run migrations at startup (the API applies them automatically on first run).

## Run the API

From the repository root:

```bash
dotnet run --project src/Host/ExcelDashboard.HttpApi.Host.csproj
```

API runs at **http://localhost:5000** (or the port shown in the console). Swagger: http://localhost:5000/swagger

## Run the Angular UI

```bash
cd ui/excel-dashboard-ui/excel-dashboard-ui
npm install
npm start
```

UI runs at **http://localhost:4200**. The app calls the API at `http://localhost:5000`; if your API uses another port, update `baseUrl` in `src/app/api.service.ts`.

## Usage

1. Open the UI and go to **Data Upload**.
2. Choose an `.xlsx` file and a currency (e.g. USD), then upload.
3. Open **My Dashboard** to see your data in a table and a bar chart by category.

## Solution structure

- `src/Domain` – Entities, value objects, repository interfaces
- `src/Application` – App services, Excel parsing orchestration
- `src/Application/Contracts` – DTOs and service contracts
- `src/Infrastructure` – EF Core, PostgreSQL, Excel parsing (ClosedXML), repositories
- `src/Host` – ASP.NET Core API
- `ui/excel-dashboard-ui/excel-dashboard-ui` – Angular SPA
- `tools` – Sample Excel generator

## CI

GitHub Actions workflow in `.github/workflows/ci.yml`: restore and build the .NET solution on push/PR to `main` or `master`.
