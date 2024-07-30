namespace YawnDB.Core;

public class Database
{
    private readonly Dictionary<string, Table> _tables = new();

    public void Execute(string sql)
    {
        var command = Parse(sql);
        switch (command.Type)
        {
            case CommandType.CreateTable:
                CreateTable(command.TableName);
                break;
            case CommandType.Insert:
                Insert(command.TableName, command.Values);
                break;
            case CommandType.Select:
                Select(command.TableName);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private static Command Parse(string sql)
    {
        // Basic SQL parsing logic (to be expanded)
        if (sql.StartsWith("CREATE TABLE"))
        {
            var tableName = sql.Split(' ')[2];
            return new Command { Type = CommandType.CreateTable, TableName = tableName };
        }
        
        if (sql.StartsWith("INSERT INTO"))
        {
            var parts = sql.Split(' ');
            var tableName = parts[2];
            var values = sql.Split('(')[2].Split(')')[0].Split(',');
            return new Command { Type = CommandType.Insert, TableName = tableName, Values = values };
        }
        
        if (sql.StartsWith("SELECT * FROM"))
        {
            var tableName = sql.Split(' ')[3];
            return new Command { Type = CommandType.Select, TableName = tableName };
        }

        throw new InvalidOperationException("Unknown SQL command.");
    }

    private void CreateTable(string tableName)
    {
        _tables[tableName] = new Table();
        Console.WriteLine($"Table {tableName} created.");
    }

    private void Insert(string tableName, string[] values)
    {
        var table = _tables[tableName];
        var row = new Dictionary<string, object> { { "id", int.Parse(values[0]) }, { "name", values[1].Trim() } };
        table.Rows.Add(row);
        Console.WriteLine($"Inserted into {tableName}: {string.Join(", ", values)}");
    }

    private void Select(string tableName)
    {
        var table = _tables[tableName];
        foreach (var row in table.Rows)
        {
            Console.WriteLine($"{row["id"]}, {row["name"]}");
        }
    }
}