namespace YawnDB.Core;

public record Command
{
    public CommandType Type { get; set; }
    public string TableName { get; set; } = "";
    public string[] Values { get; set; } = [];

    public static Command Unsupported => new Command { Type = CommandType.Unsupported };
}