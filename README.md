# DbUp Database Migration Tool

A robust database migration utility built with DbUp that provides automated database schema upgrades with comprehensive logging and error handling.

## Features

- **Multi-Database Support**: Configure and upgrade multiple databases in a single run
- **Comprehensive Logging**: Detailed console output with color-coded messages and persistent log files
- **Error Handling**: Graceful error handling with detailed error reporting
- **Transaction Safety**: Each script runs in its own transaction for rollback safety
- **Timestamped Logs**: All operations are timestamped for audit trails
- **Execution Timeout**: Configurable timeout (5 minutes default) to prevent hanging operations

## Prerequisites

- .NET Core 9
- SQL Server database
- DbUp NuGet package

## Installation

1. Clone or download this project
2. Install the required NuGet package:
   ```bash
   dotnet add package DbUp
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

## Project Structure

```
??? Program.cs              # Main application entry point
??? DBScripts/             # Directory containing SQL migration scripts
?   ??? 001_CreateTables.sql
?   ??? 002_AddIndexes.sql
?   ??? ...
??? AllDBLogs/             # Generated log files (created automatically)
    ??? UpgradeLog_yyyy-MM-dd_HH-mm-ss.txt
```

## Automatic Database Creation
One of the powerful features of this tool is automatic database creation using DbUp's EnsureDatabase functionality. If a target database doesn't exist, the tool will automatically create it before running migrations.
How It Works
The tool uses the following line in the code to ensure database existence:
`EnsureDatabase.For.SqlDatabase(db.ConnectionString);`
This automatically:

- Connects to the SQL Server instance
- Checks if the specified database exists
- Creates the database if it doesn't exist
- Uses default database settings (can be customized)


## Configuration

### Database Configuration

Edit the `databases` array in `Program.cs` to configure your target databases:

```csharp
var databases = new[]
{
    new { Name = "ProductionDB", ConnectionString = "Server=prod-server;Database=MyApp;Trusted_Connection=true;", ScriptPath = "DBScripts" },
    new { Name = "StagingDB", ConnectionString = "Server=staging-server;Database=MyApp;Trusted_Connection=true;", ScriptPath = "DBScripts" },
    // Add more databases as needed
};
```

### Connection String Examples

**SQL Server with Windows Authentication:**
```
Server=localhost;Database=MyDatabase;Trusted_Connection=true;
```

**SQL Server with SQL Authentication:**
```
Server=localhost;Database=MyDatabase;User Id=myuser;Password=mypassword;
```

## SQL Script Guidelines

### Naming Convention
- Use numbered prefixes for ordering: `001_`, `002_`, etc.
- Use descriptive names: `001_CreateUserTable.sql`, `002_AddUserIndexes.sql`
- Scripts are executed in alphabetical order

### Script Structure
```sql
-- 001_CreateUserTable.sql
-- Description: Create the initial user table

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO
```

### Best Practices
- Always check for existence before creating objects (`IF NOT EXISTS`)
- Use transactions where appropriate
- Include rollback scripts as comments
- Test scripts on development environment first
- Keep scripts idempotent (safe to run multiple times)

## Usage

### Basic Usage
```bash
dotnet run
```

### Command Line Usage
```bash
# Run the executable directly
./YourProjectName.exe

# Or with dotnet
dotnet YourProjectName.dll
```

## Logging

The tool provides two types of logging:

### Console Output
- **Cyan**: Database section headers
- **Green**: Successful operations
- **Red**: Failed operations
- **Yellow**: Warning messages

### Log Files
- Located in `AllDBLogs/` directory
- Named with timestamp: `UpgradeLog_2024-01-15_14-30-22.txt`
- Contains all console output for permanent record
- Automatically flushed for real-time viewing

### Sample Log Output
```
====== Upgrading testDB ======
2024-01-15 14:30:22 | testDB | 001_CreateUserTable.sql | SUCCESS
2024-01-15 14:30:23 | testDB | 002_AddIndexes.sql | SUCCESS
All databases upgraded successfully.
```

## Error Handling

### Exit Codes
- `0`: Success - All databases upgraded successfully
- `-1`: Error - One or more databases failed to upgrade

### Error Scenarios
- **Connection Failures**: Database unreachable or credentials invalid
- **Script Errors**: SQL syntax errors or constraint violations
- **Timeout**: Scripts taking longer than 5 minutes
- **File System**: Script files not found or inaccessible

### Recovery
- Fix the problematic script
- DbUp tracks executed scripts, so only failed/new scripts will run
- Re-run the tool after fixing issues

## Customization

### Timeout Configuration
Change the execution timeout in `Program.cs`:
```csharp
.WithExecutionTimeout(TimeSpan.FromMinutes(10)) // 10 minutes instead of 5
```

### Log Directory
Modify the logs folder location:
```csharp
var logsFolder = "CustomLogPath";
```

### Script Location
Change script directory per database:
```csharp
new { Name = "MyDB", ConnectionString = "...", ScriptPath = "CustomScripts" }
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add your changes with appropriate tests
4. Update documentation
5. Submit a pull request

## Author
Ahmed Elmaadawy
Backend .NET Developer
GitHub: @ahmedelmaadawy
LinkedIn: Ahmed Elmaadawy