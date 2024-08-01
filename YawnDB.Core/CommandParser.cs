namespace YawnDB.Core;

using System;
using System.Text.RegularExpressions;

public static partial class CommandParser
{
    [GeneratedRegex(@"CREATE\s+TABLE\s+([`\[\w\]\.]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CreateTableRegex();

    [GeneratedRegex(@"INSERT\s+INTO\s+([`\[\w\]\.]+)\s+VALUES\s*\((.*)\)", RegexOptions.IgnoreCase)]
    private static partial Regex InsertRegex();

    [GeneratedRegex(@"SELECT\s+\*\s+FROM\s+([`\[\w\]\.]+)", RegexOptions.IgnoreCase)]
    private static partial Regex SelectRegex();

    [GeneratedRegex(@"UPDATE\s+([`\[\w\]\.]+)\s+SET\s+(.*)", RegexOptions.IgnoreCase)]
    private static partial Regex UpdateRegex();

    [GeneratedRegex(@"DELETE\s+FROM\s+([`\[\w\]\.]+)\s+WHERE\s+(.*)", RegexOptions.IgnoreCase)]
    private static partial Regex DeleteRegex();

    public static Command Parse(string sql)
    {
        var createTableMatch = CreateTableRegex().Match(sql);
        if (createTableMatch.Success)
        {
            return new Command
            {
                Type = CommandType.CreateTable,
                TableName = createTableMatch.Groups[1].Value
            };
        }

        var insertMatch = InsertRegex().Match(sql);
        if (insertMatch.Success)
        {
            var values = insertMatch.Groups[2].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim()).ToArray();

            return new Command
            {
                Type = CommandType.Insert,
                TableName = insertMatch.Groups[1].Value,
                Values = values
            };
        }

        var selectMatch = SelectRegex().Match(sql);
        if (selectMatch.Success)
        {
            return new Command
            {
                Type = CommandType.Select,
                TableName = selectMatch.Groups[1].Value
            };
        }

        var updateMatch = UpdateRegex().Match(sql);
        if (updateMatch.Success)
        {
            var values = updateMatch.Groups[2].Value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim()).ToArray();

            return new Command
            {
                Type = CommandType.Update,
                TableName = updateMatch.Groups[1].Value,
                Values = values
            };
        }

        var deleteMatch = DeleteRegex().Match(sql);
        if (deleteMatch.Success)
        {
            var values = deleteMatch.Groups[2].Value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim()).ToArray();

            return new Command
            {
                Type = CommandType.Delete,
                TableName = deleteMatch.Groups[1].Value,
                Values = values
            };
        }

        return Command.Unsupported;
    }
}