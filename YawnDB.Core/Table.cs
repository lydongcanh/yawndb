namespace YawnDB.Core;

public class Table
{
    public List<Dictionary<string, object>> Rows { get; } = [];

    // Add a new row to the table
    public void AddRow(Dictionary<string, object> row)
    {
        Rows.Add(row);
    }

    // Update a row based on a condition
    public void UpdateRow(Func<Dictionary<string, object>, bool> condition, Dictionary<string, object> newValues)
    {
        foreach (var row in Rows.Where(condition))
        {
            foreach (var key in newValues.Keys)
            {
                row[key] = newValues[key];
            }
        }
    }

    // Delete rows based on a condition
    public void DeleteRows(Func<Dictionary<string, object>, bool> condition)
    {
        Rows.RemoveAll(row => condition(row));
    }

    // Retrieve rows based on a condition
    public List<Dictionary<string, object>> SelectRows(Func<Dictionary<string, object>, bool> condition)
    {
        return Rows.Where(condition).ToList();
    }

    // Retrieve all rows
    public List<Dictionary<string, object>> SelectAllRows()
    {
        return Rows.ToList();
    }

    // Retrieve specific columns from rows based on a condition
    public List<Dictionary<string, object>> SelectColumns(Func<Dictionary<string, object>, bool> condition,
        List<string> columns)
    {
        return Rows.Where(condition)
            .Select(row => row.Where(kvp => columns.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            .ToList();
    }
}