namespace asm.Asm;

public static class MessageDisplay
{
    private static Func<int, string, string, string> OutputMessage 
        = (index, type, message) => $"{type} at line {index} : {message}";



    public static List<string> ErrorList { get; private set; } = new List<string>();
    public static List<string> WarningList { get; private set; } = new List<string>();


    public static void AddWarning(int line, string message)
    {
        WarningList.Add(OutputMessage(line, "Warning", message));

    }

    public static void AddError(int line,string message)
    {
        ErrorList.Add(OutputMessage(line,"Error",message));
    }
}