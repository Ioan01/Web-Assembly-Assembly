﻿namespace asm.Asm;

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
            argument = argument.Remove(0, "offset R".Length);
            ValueType = Type.RegisterOffset;
            Value = int.Parse(argument);
        }
        // if is register (Rx) -> extract x ! careful not to use 'R' which is 82 (ascii)
        else if (argument.Contains('R') && !argument.Contains('\''))
        {
            ValueType = Type.Register;
            Value = int.Parse(argument[1].ToString());
        }
        // char instead of number
        else if (argument.Contains('\''))
        {
            ValueType = Type.Immediate;
            if (argument[1] == '\\')
                Value = GetSpecialCharacter(argument[2]);
            else Value = argument[1];
        }
        else
        {
            ValueType = Type.Immediate;

            if (argument.Contains("0x"))
                Value = Convert.ToInt32(argument, 16);
            else Value = int.Parse(argument);
        }
    }

    public Type ValueType { get; }

    public int Value { get; }

    private int GetSpecialCharacter(char c)
    {
        switch (c)
        {
            case 'n':
                return '\n';
            case '0':
                return 0;
            default:
                return -1;
        }
    }

    public override string ToString()
    {
        switch (ValueType)
        {
            case Type.Immediate:
                return Value.ToString();
            case Type.Register:
                return 'R' + Value.ToString();
            case Type.RegisterOffset:
                return "offset R" + Value;
        }

        return "";
    }
}