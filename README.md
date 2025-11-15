# ExpenseTrackerAPI

RESTful API which allows users to create, read, update, and delete expenses.

Solution for the roadmap.sh project: [Expense Tracker API](https://roadmap.sh/projects/expense-tracker-api)

## Features

- Sign up as a new user.
- Generate and validate JWTs for handling authentication and user session.
- List and filter your past expenses. You can add the following filters:
  - Past week
  - Past month
  - Last 3 months
  - Custom (to specify a start and end date of your choosing).
- Add a new expense
- Remove existing expenses
- Update existing expenses

## Requirements

- .NET 8 SDK or later
- Visual Studio 2022 or another IDE of choice
- SQL Server 2022

## Installations

### 1. Clone the repository

```bash
git clone https://github.com/UnUsuarioMas67/ASP.NET-ExpenseTrackerAPI.git
cd ASP.NET-ExpenseTrackerAPI
```

### 2. Setup Environment Variables

Add the following properties to `appsettings.Development.json`:

```
  "Jwt": {
    "Secret": "YOUR SECRET KEY",
    ...
  },
  "ConnectionStrings": {
    "ExpenseTracker": "YOUR CONNECTION STRING"
  }
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
cd ExpenseTrackerAPI
dotnet run
```
