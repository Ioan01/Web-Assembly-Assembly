using asm.Asm;

namespace Shared.Asm;

public static class BitOperations
{
    public static uint GetBits(int number, int from, int to)
    {
        if (from > to) throw new ArgumentException("From parameter greater than to parameter.");
        var size = sizeof(int) * 8 - 1;

        int mask;

        if (number > 0)
            // right shift with uint to avoid leading adding 1's
            mask = (int)((~0u << (from + size - to)) >>> (size - to));
        else mask = (int)((~0u << (from + size - to)) >>> (size - to));

        var bits = (number & mask) >>> from;


        return (uint)bits;
    }

    public static uint GetRegisterValue(int number)
    {
        return GetBits(number, 0, 2);
    }

    public static uint Get26BitImmediateValue(int number)
    {
        return GetBits(number, 0, 25);
    }

    public static string ToBinary(uint number)
    {
        var str = Convert.ToString(number, 2);
        while (str.Length != sizeof(int) * 8) str = '0' + str;

        return str;
    }

    public static uint Get26ImmediateValueFromInstruction(Instruction instruction)
    {
        return Get26BitImmediateValue((int)instruction.Binary);
    }

    public static int GetSigned26ImmediateValueFromInstruction(Instruction instruction)
    {
        var unsignedValue = (int)Get26ImmediateValueFromInstruction(instruction);
        if ((unsignedValue & (1 << 25)) != 0)
            return (int)(0xFC000000 | unsignedValue);
        return unsignedValue;
    }
}