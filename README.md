# Shopping List API
This is the backend of the [Shopping List app](https://github.com/akali013/shopping-list) that handles all user and item updates within the app.

## Technologies Used
- API: [.NET Core v9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Database: SQL Server Express and [SQL Server Management Studio 21](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## How to Run
> [!Note]
> Before proceeding, make sure you have a container runtime like Docker Desktop.

1. Clone this repository via `git clone https://github.com/akali013/shopping-list` in a terminal.
2. Navigate to the `shopping-list` directory with `cd .\shopping-list\` in a terminal.
3. Run `docker compose up --build -d`.

## Credits
[Backend JWT Authentication](https://jasonwatmore.com/net-6-jwt-authentication-with-refresh-tokens-tutorial-with-example-api) by Jason Watmore
