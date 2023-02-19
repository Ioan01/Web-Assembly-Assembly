namespace asm.Asm
{
    public enum Type
    {
        Immediate,
        Register,
        RegisterOffset
    }
    
    
    public class Argument
    {
        public Argument(string argument)
        {
            argument = argument.Trim();
            // if register offset (offset Rx) -> extract x
            if (argument.Contains("offset"))
            {
                argument = argument.Remove(0,"offset R".Length);
                ValueType = Type.RegisterOffset;
                Value = int.Parse(argument);
            }
            // if is register (Rx) -> extract x ! careful not to use 'R' which is 82 (ascii)
            else if (argument.Contains('R') && !argument.Contains('\''))
            {
                ValueType = Type.Register;
                Value = int.Parse(argument[1].ToString());
            }
            else if (argument.Contains('\''))
            {
                ValueType = Type.Immediate;
                Value = argument[1];
            }
            else
            {
                ValueType = Type.Immediate;
                Value = int.Parse(argument);
            }
        }

        public Type ValueType { get; }

        public int Value { get; }
    }
}
