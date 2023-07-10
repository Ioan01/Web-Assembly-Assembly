namespace asm.Asm;

public class Memory
{
    public const int Size = 256 * 16;
    public const int StackSize = Size / 16;


    private readonly uint[] words = new uint[Size];

    public uint Read(uint address)
    {
        ErrorHandler.VerifyAddress(address);
        return words[address];
    }

    public void Write(uint address, uint value)
    {
        ErrorHandler.VerifyAddress(address);
        words[address] = value;
    }


    public void Reset()
    {
        for (var i = 0; i < Size; i++) words[i] = 0;
    }
}