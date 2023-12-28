namespace Shared.Asm;

public class Memory
{
    public const int Size = 256 * 16;
    public const int StackSize = Size / 16;


    private readonly uint[] words = new uint[Size];
    private readonly ErrorHandler _errorHandler;

    public Memory(ErrorHandler errorHandler)
    {
        _errorHandler = errorHandler;
    }

    public uint Read(uint address)
    {
        _errorHandler.VerifyAddress(address);
        return words[address];
    }

    public void Write(uint address, uint value)
    {
        _errorHandler.VerifyAddress(address);
        words[address] = value;
    }


    public void Reset()
    {
        for (var i = 0; i < Size; i++) words[i] = 0;
    }
}