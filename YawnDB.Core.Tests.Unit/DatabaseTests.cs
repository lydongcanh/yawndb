namespace YawnDB.Core.Tests.Unit;

public class DatabaseTests : IDisposable
{
    private readonly string _testDatabaseDirectory;
    private readonly Database _database;

    public DatabaseTests()
    {
        _testDatabaseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Console.WriteLine(_testDatabaseDirectory);
        _database = new Database(_testDatabaseDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDatabaseDirectory))
        {
            Directory.Delete(_testDatabaseDirectory, true);
        }
    }

    #region Commands

    [Fact]
    public void Execute_CreateTable_Command_ShouldCreateTable()
    {
        // Arrange
        var command = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };

        // Act
        _database.Execute(command);

        // Assert
        Assert.True(File.Exists(Path.Combine(_testDatabaseDirectory, "TestTable.json")));
    }

    [Fact]
    public void Execute_Insert_Command_ShouldInsertRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = ["1", "John Doe"]
        };

        // Act
        _database.Execute(insertCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[\"1\",\"John Doe\"]", tableData);
    }

    [Fact]
    public void Execute_Select_Command_ShouldOutputTableContents()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = ["1", "John Doe"]
        };
        _database.Execute(insertCommand);

        var selectCommand = new Command
        {
            Type = CommandType.Select,
            TableName = "TestTable"
        };

        // Act
        var sw = new StringWriter();
        Console.SetOut(sw);
        _database.Execute(selectCommand);

        // Assert
        var output = sw.ToString().Trim();
        Assert.Contains("[1, John Doe]", output);
    }

    [Fact]
    public void Execute_Update_Command_ShouldUpdateRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = ["1", "John Doe"]
        };
        _database.Execute(insertCommand);

        var updateCommand = new Command
        {
            Type = CommandType.Update,
            TableName = "TestTable",
            Values = ["1", "Jane Doe"]
        };

        // Act
        _database.Execute(updateCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[\"1\",\"Jane Doe\"]", tableData);
        Assert.DoesNotContain("[\"1\",\"John Doe\"]", tableData);
    }

    [Fact]
    public void Execute_Delete_Command_ShouldDeleteRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = ["1", "John Doe"]
        };
        _database.Execute(insertCommand);

        var deleteCommand = new Command
        {
            Type = CommandType.Delete,
            TableName = "TestTable",
            Values = ["1"]
        };

        // Act
        _database.Execute(deleteCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.DoesNotContain("[\"1\",\"John Doe\"]", tableData);
    }

    #endregion

    #region Raw Sql

    [Fact]
    public void Execute_WithSQL_CreateTable_ShouldParseAndExecute()
    {
        // Arrange
        const string sql = "CREATE TABLE TestTable";

        // Act
        _database.Execute(sql);

        // Assert
        Assert.True(File.Exists(Path.Combine(_testDatabaseDirectory, "TestTable.json")));
    }

    [Fact]
    public void Execute_WithSQL_Insert_ShouldParseAndExecute()
    {
        // Arrange
        _database.Execute("CREATE TABLE TestTable");
        const string sql = "INSERT INTO TestTable VALUES (1, 'John Doe')";

        // Act
        _database.Execute(sql);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[[\"1\",\"\\u0027John Doe\\u0027\"]]", tableData);
    }

    [Fact]
    public void Execute_WithSQL_Select_ShouldParseAndExecute()
    {
        // Arrange
        _database.Execute("CREATE TABLE TestTable");
        _database.Execute("INSERT INTO TestTable VALUES (1, 'John Doe')");
        const string sql = "SELECT * FROM TestTable";

        // Act
        var sw = new StringWriter();
        Console.SetOut(sw);
        _database.Execute(sql);

        // Assert
        var output = sw.ToString().Trim();
        Assert.Contains("[1, 'John Doe']", output);
    }

    [Fact]
    public void Execute_WithSQL_Update_ShouldParseAndExecute()
    {
        // Arrange
        _database.Execute("CREATE TABLE TestTable");
        _database.Execute("INSERT INTO TestTable VALUES (1, 'John Doe')");
        const string sql = "UPDATE TestTable SET 1 = 'Jane Doe'";

        // Act
        _database.Execute(sql);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[[\"1\",\"\\u0027Jane Doe\\u0027\"]]", tableData);
        Assert.DoesNotContain("[[\"1\",\"\\John Doe\\u0027\"]]", tableData);
    }

    [Fact]
    public void Execute_WithSQL_Delete_ShouldParseAndExecute()
    {
        // Arrange
        _database.Execute("CREATE TABLE TestTable");
        _database.Execute("INSERT INTO TestTable VALUES (1, 'John Doe')");
        const string sql = "DELETE FROM TestTable WHERE 1 = 'John Doe'";

        // Act
        _database.Execute(sql);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.DoesNotContain("[\"1\",\"John Doe\"]", tableData);
    }

    [Fact]
    public void Execute_Command_CreateTable_ShouldCreateTable()
    {
        // Arrange
        var command = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };

        // Act
        _database.Execute(command);

        // Assert
        Assert.True(File.Exists(Path.Combine(_testDatabaseDirectory, "TestTable.json")));
    }

    [Fact]
    public void Execute_Command_Insert_ShouldInsertRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = new[] { "1", "John Doe" }
        };

        // Act
        _database.Execute(insertCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[\"1\",\"John Doe\"]", tableData);
    }

    [Fact]
    public void Execute_Command_Select_ShouldOutputTableContents()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = new[] { "1", "John Doe" }
        };
        _database.Execute(insertCommand);

        var selectCommand = new Command
        {
            Type = CommandType.Select,
            TableName = "TestTable"
        };

        // Act
        var sw = new StringWriter();
        Console.SetOut(sw);
        _database.Execute(selectCommand);

        // Assert
        var output = sw.ToString().Trim();
        Assert.Contains("[1, John Doe]", output);
    }

    [Fact]
    public void Execute_Command_Update_ShouldUpdateRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = new[] { "1", "John Doe" }
        };
        _database.Execute(insertCommand);

        var updateCommand = new Command
        {
            Type = CommandType.Update,
            TableName = "TestTable",
            Values = new[] { "1", "Jane Doe" }
        };

        // Act
        _database.Execute(updateCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.Contains("[\"1\",\"Jane Doe\"]", tableData);
        Assert.DoesNotContain("[\"1\",\"John Doe\"]", tableData);
    }

    [Fact]
    public void Execute_Command_Delete_ShouldDeleteRecord()
    {
        // Arrange
        var createTableCommand = new Command
        {
            Type = CommandType.CreateTable,
            TableName = "TestTable"
        };
        _database.Execute(createTableCommand);

        var insertCommand = new Command
        {
            Type = CommandType.Insert,
            TableName = "TestTable",
            Values = new[] { "1", "John Doe" }
        };
        _database.Execute(insertCommand);

        var deleteCommand = new Command
        {
            Type = CommandType.Delete,
            TableName = "TestTable",
            Values = new[] { "1" }
        };

        // Act
        _database.Execute(deleteCommand);

        // Assert
        var tableData = File.ReadAllText(Path.Combine(_testDatabaseDirectory, "TestTable.json"));
        Assert.DoesNotContain("[\"1\",\"John Doe\"]", tableData);
    }

    #endregion
}