namespace Shared.Asm;

public class ErrorHandler
{
    public void VerifyAddress(uint address)
    {
        if (address > Memory.Size)
            throw new InvalidOperationException($"Memory address given to access memory is too large {address}");
    }

    public void ValidatePush(uint stackPointer)
    {
        if (stackPointer == Memory.Size - Memory.StackSize)
            throw new InvalidOperationException("Cannot push when stack is full");
    }

    public void ValidatePop(uint stackPointer)
    {
        if (stackPointer >= Memory.Size)
            throw new InvalidOperationException("Cannot pop when stack is empty");
    }
}