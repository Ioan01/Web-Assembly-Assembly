using System.Text;

namespace asm.Asm;

internal class Ports
{
    public const uint NumericConsole = 0;

    public const uint ASCIIConsole = 1;

    public const uint HexConsole = 2;

    public const uint BinaryConsole = 3;

    public const uint Screen = 4;

    public const uint Audio = 5;
}

public class IO
{
    private readonly StringBuilder sb = new(4096);

    public void Write(uint port, int data)
    {
        switch (port)
        {
            case Ports.NumericConsole:
                sb.Append(data);
                break;
            case Ports.ASCIIConsole:
                sb.Append((char)data);
                break;
            case Ports.HexConsole:
                sb.Append("0x");
                sb.Append(Convert.ToString(data, 16));
                break;
            case Ports.BinaryConsole:
                sb.Append("0b");
                sb.Append(Convert.ToString(data, 2));
                break;
        }
    }

    public int Read(uint instructionOperand1)
    {
        return new Random().Next();
    }

    public string? GetBuffer()
    {
        if (sb.Length == 0) return null;


        var str = sb.ToString();
        sb.Clear();

        return str;
    }
}