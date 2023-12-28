namespace Shared.Asm;

public class MessageDisplay
{
    private static readonly Func<int, string, string, string> OutputMessage
        = (index, type, message) => $"{type} at line {index} : {message}";


    public List<string> ErrorList { get; } = new();
    public List<string> WarningList { get; } = new();


    public void AddWarning(int line, string message)
    {
        WarningList.Add(OutputMessage(line, "Warning", message));
    }

    public void AddError(int line, string message)
    {
        ErrorList.Add(OutputMessage(line, "Error", message));
    }
}