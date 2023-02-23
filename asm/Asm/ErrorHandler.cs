namespace asm.Asm;

public static class ErrorHandler
{
    
    public static void VerifyAddress(uint address)
    {
        if (address > Memory.Size)
            throw new InvalidOperationException($"Memory address given to access memory is too large {address}");
    }

    public static void ValidatePush(uint stackPointer)
    {
        if (stackPointer == Memory.Size - Memory.StackSize)
        {
            throw new InvalidOperationException("Cannot push when stack is full");
        }
    }

    public static void ValidatePop(uint stackPointer)
    {
        if (stackPointer >= Memory.Size)
            throw new InvalidOperationException("Cannot pop when stack is empty");
    }
}