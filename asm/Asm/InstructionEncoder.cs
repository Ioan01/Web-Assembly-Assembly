namespace asm.Asm;

public class InstructionEncoder
{
    public uint EncodeInstruction(IntermediaryInstruction instruction)
    {
        var variant = 0u;
        var _26Bits = 0u;

        switch (instruction.Alias)
        {
            case Instructions.Input:
            case Instructions.Output:
                _26Bits = EncodeIO(instruction, ref variant);
                break;
            case Instructions.LoadRegister:
            case Instructions.StoreRegister:
                _26Bits = EncodeLoadAndStore(instruction, ref variant);
                break;
            case Instructions.ReturnFromSubroutine:
            case Instructions.Halt:
                break;
            case Instructions.PushToStack:
                _26Bits = EncodePsh(instruction, ref variant);
                break;
            case Instructions.PopFromStack:
                _26Bits = BitOperations.GetRegisterValue(instruction.Arguments[0].Value);
                break;
            case Instructions.Compare:
                _26Bits = EncodeCmp(instruction, ref variant);
                break;
            case Instructions.BranchAlways:
            case Instructions.JumpToSubroutine:
            case Instructions.BranchIfEqual:
            case Instructions.BranchIfZero:
            case Instructions.BranchIfMinus:
            case Instructions.BranchIfPlus:
            case Instructions.BranchIfLessThan:
                _26Bits = BitOperations.Get26BitImmediateValue(instruction.Arguments[0].Value);
                break;
            case Instructions.Add:
            case Instructions.Subtract:
            case Instructions.Multiply:
            case Instructions.Divide:
            case Instructions.Modulo:
            case Instructions.BitwiseAnd:
            case Instructions.BitwiseOr:
            case Instructions.BitwiseExclusiveOr:
            case Instructions.RightShift:
            case Instructions.LeftShift:
            case Instructions.MoveToRegister:
                _26Bits = EncodeALU(instruction, ref variant);
                break;
        }

        var binary = ((Instructions.Opcodes[instruction.Alias] + variant) << 26) | _26Bits;


        return binary;
    }

    private uint EncodeALU(IntermediaryInstruction instruction, ref uint variant)
    {
        if (instruction.Arguments[1].ValueType == Type.Immediate)
            return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
                   (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);

        variant = 1;
        return BitOperations.GetRegisterValue(instruction.Arguments[1].Value) |
               (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3);
    }

    private uint EncodeCmp(IntermediaryInstruction instruction, ref uint variant)
    {
        if (instruction.Arguments[1].ValueType == Type.Register)
        {
            variant = 1;
            return (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3) |
                   BitOperations.GetRegisterValue(instruction.Arguments[1].Value);
        }

        return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
               (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);
    }

    private uint EncodePsh(IntermediaryInstruction instruction, ref uint variant)
    {
        if (instruction.Arguments[0].ValueType == Type.Register)
            return BitOperations.GetRegisterValue(instruction.Arguments[0].Value);

        variant = 1;
        return BitOperations.Get26BitImmediateValue(instruction.Arguments[0].Value);
    }

    private uint EncodeIO(IntermediaryInstruction instruction, ref uint variant)
    {
        return (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 8)
               | BitOperations.GetBits(instruction.Arguments[1].Value, 0, 7);
    }


    private uint EncodeLoadAndStore(IntermediaryInstruction instruction, ref uint variant)
    {
        // LDR/STR Rx,Raddress, variant 1
        if (instruction.Arguments[1].ValueType == Type.Register)
        {
            variant = 1;
            return (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3)
                   | BitOperations.GetRegisterValue(instruction.Arguments[1].Value);
        }

        if (instruction.Arguments[2].ValueType == Type.RegisterOffset)
        {
            variant = 2;

            // offset register 
            return BitOperations.GetRegisterValue(instruction.Arguments[2].Value) |
                   (BitOperations.GetBits(instruction.Arguments[1].Value, 0, 19) << 3) |
                   (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);
        }
        // LDR Rx,immediate

        return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
               (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);
    }
}