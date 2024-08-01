using System.Text.Json;

namespace YawnDB.Core;

public class Database
{
    private readonly string _databaseDirectory;

    public Database(string databaseDirectory)
    {
        _databaseDirectory = databaseDirectory;
        Directory.CreateDirectory(_databaseDirectory);
    }

    public void Execute(string sql)
    {
        var command = CommandParser.Parse(sql);
        Execute(command);
    }
    
    public void Execute(Command command)
    {
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
            case CommandType.Update:
                Update(command.TableName, command.Values);
                break;
            case CommandType.Delete:
                Delete(command.TableName, command.Values);
                break;
            case CommandType.Unsupported:
            default:
                Console.WriteLine("Unsupported command type.");
                break;
        }
    }

    private void CreateTable(string tableName)
    {
        var tablePath = GetTablePath(tableName);
        if (File.Exists(tablePath))
        {
            Console.WriteLine($"Table '{tableName}' already exists.");
            return;
        }

        File.WriteAllText(tablePath, "[]"); // Initialize empty table as JSON array
        Console.WriteLine($"Table '{tableName}' created.");
    }

    private void Insert(string tableName, string[] values)
    {
        var tableData = LoadTableData(tableName);
        tableData.Add(values);

        SaveTableData(tableName, tableData);
        Console.WriteLine($"Inserted into '{tableName}': [{string.Join(", ", values)}]");
    }

    private void Select(string tableName)
    {
        var tableData = LoadTableData(tableName);
        Console.WriteLine($"Contents of '{tableName}':");
        foreach (var record in tableData)
        {
            Console.WriteLine($"[{string.Join(", ", record)}]");
        }
    }

    private void Update(string tableName, string[] values)
    {
        var tableData = LoadTableData(tableName);
        var updated = false;

        for (var i = 0; i < tableData.Count; i++)
        {
            if (tableData[i][0] == values[0]) // Assuming first value is a unique key
            {
                tableData[i] = values;
                updated = true;
                break;
            }
        }

        if (updated)
        {
            SaveTableData(tableName, tableData);
            Console.WriteLine($"Updated record in '{tableName}': [{string.Join(", ", values)}]");
        }
        else
        {
            Console.WriteLine($"No matching record found in '{tableName}' to update.");
        }
    }

    private void Delete(string tableName, string[] values)
    {
        var tableData = LoadTableData(tableName);
        var initialCount = tableData.Count;

        tableData = tableData.Where(record => record[0] != values[0]).ToList();

        if (tableData.Count < initialCount)
        {
            SaveTableData(tableName, tableData);
            Console.WriteLine($"Deleted record from '{tableName}' with key: {values[0]}");
        }
        else
        {
            Console.WriteLine($"No matching record found in '{tableName}' to delete.");
        }
    }

    private string GetTablePath(string tableName) => Path.Combine(_databaseDirectory, $"{tableName}.json");

    private List<string[]> LoadTableData(string tableName)
    {
        var tablePath = GetTablePath(tableName);
        if (!File.Exists(tablePath))
        {
            throw new FileNotFoundException($"Table '{tableName}' does not exist.");
        }

        var json = File.ReadAllText(tablePath);
        return JsonSerializer.Deserialize<List<string[]>>(json) ?? new List<string[]>();
    }

    private void SaveTableData(string tableName, List<string[]> tableData)
    {
        var tablePath = GetTablePath(tableName);
        var json = JsonSerializer.Serialize(tableData);
        File.WriteAllText(tablePath, json);
    }
}