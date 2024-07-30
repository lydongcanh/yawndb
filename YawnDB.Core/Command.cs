namespace YawnDB.Core;

public class Command
{
    public CommandType Type { get; set; }
    public string TableName { get; set; }
    public string[] Values { get; set; }
}