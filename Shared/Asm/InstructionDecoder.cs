using asm.Asm;

namespace Shared.Asm;

public class InstructionDecoder
{
    private readonly Func<Instruction, int, Emulator, Action> _fetchAlu = (instruction, variant, emulator) =>
    {
        if (variant == 0)
            return () =>
            {
                instruction.operand1 = (int)instruction.GetBits(0, 22);
                instruction.operand2 = (int)instruction.GetBits(23, 25);
            };
        return () =>
        {
            instruction.operand1 = emulator.Registers[instruction.GetBits(0, 2)];
            instruction.operand2 = (int)instruction.GetBits(3, 5);
        };
    };

    private readonly Func<Instruction, Action> _ioFetch = instruction => () =>
    {
        instruction.operand1 = (int)instruction.GetBits(0, 7);
        instruction.operand2 = (int)instruction.GetBits(8, 10);
    };


    private readonly Func<Func<bool>, Instruction, Emulator, Action> _branching = (func, instruction, emulator) => () =>
    {
        var relativeAddress = BitOperations.GetSigned26ImmediateValueFromInstruction(instruction);

        if (func.Invoke())
            emulator.Branch(relativeAddress);
    };

    // operand 1 is address to load/store
    // operand 2 is source/destionation register
    private readonly Func<Instruction, int, Action> _fetchStoreLoad = (instruction, variant) => () =>
    {
        var bits = (int)instruction.Binary;


        switch (variant)
        {
            case 0:
                instruction.operand1 = (int)instruction.GetBits(0, 22);
                instruction.operand2 = (int)instruction.GetBits(23, 25);
                break;
            case 1:
                instruction.operand1 = (int)BitOperations.GetRegisterValue(bits);
                instruction.operand2 = (int)instruction.GetBits(3, 5);
                break;
            case 2:
                instruction.operand1 = (int)(BitOperations.GetRegisterValue(bits) + instruction.GetBits(3, 22));
                instruction.operand2 = (int)instruction.GetBits(23, 25);
                break;
        }
    };

    public Emulator Emulator { get; set; }

    public Instruction DecodeInstruction(uint instructionBinary)
    {
        var opcode = BitOperations.GetBits((int)instructionBinary, 26, 31);

        var instructionType = "HLT";
        var variant = 0;

        var instruction = new Instruction(instructionBinary);


        if (Instructions.OpcodesReverse.ContainsKey(opcode))
        {
            instructionType = Instructions.OpcodesReverse[opcode];
        }
        // instruction can have variants, whose opcodes are similar
        else
        {
            if (Instructions.OpcodesReverse.ContainsKey(opcode - 1))
            {
                instructionType = Instructions.OpcodesReverse[opcode - 1];
                variant = 1;
            }
            else
            {
                instructionType = Instructions.OpcodesReverse[opcode - 2];
                variant = 2;
            }
        }

        instruction.Type = instructionType;


        switch (instructionType)
        {
            case Instructions.Output:
                DecodeOutput(instruction);
                break;
            case Instructions.Input:
                DecodeInput(instruction);
                break;
            case Instructions.LoadRegister:
                DecodeLoad(instruction, variant);
                break;
            case Instructions.StoreRegister:
                DecodeStore(instruction, variant);
                break;
            case Instructions.Halt:
                instruction.Execute = () => Emulator.Stop();
                break;
            case Instructions.JumpToSubroutine:
                DecodeJump(instruction);
                break;
            case Instructions.PushToStack:
                DecodePush(instruction, variant);
                break;
            case Instructions.PopFromStack:
                DecodePop(instruction);
                break;
            case Instructions.ReturnFromSubroutine:
                DecodeReturn(instruction);
                break;
            case Instructions.Compare:
                DecodeCompare(instruction, variant);
                break;
            case Instructions.BranchAlways:
                instruction.Execute = _branching(() => true, instruction, Emulator);
                break;
            case Instructions.BranchIfEqual:
                instruction.Execute = _branching(() => Emulator.Zero && Emulator.Carry, instruction, Emulator);
                break;
            case Instructions.BranchIfZero:
                instruction.Execute = _branching(() => Emulator.Zero, instruction, Emulator);
                break;
            case Instructions.BranchIfMinus:
                instruction.Execute = _branching(() => Emulator.Negative, instruction, Emulator);
                break;
            case Instructions.BranchIfPlus:
                instruction.Execute = _branching(() => Emulator.Carry, instruction, Emulator);
                break;
            case Instructions.BranchIfLessThan:
                instruction.Execute = _branching(() => Emulator.Negative, instruction, Emulator);
                break;
            case Instructions.BranchIfGreaterThan:
                instruction.Execute = _branching(() => Emulator.Carry, instruction, Emulator);
                break;
            case Instructions.Add:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i + i1);
                break;
            case Instructions.Subtract:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i - i1);
                break;
            case Instructions.Multiply:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i1 * i);
                break;
            case Instructions.Divide:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i % i1);
                break;
            case Instructions.Modulo:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i % i1);
                break;
            case Instructions.BitwiseAnd:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i1 & i);
                break;
            case Instructions.BitwiseOr:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i1 | i);
                break;
            case Instructions.BitwiseExclusiveOr:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i1 ^ i);
                break;
            case Instructions.RightShift:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i >> i1);
                break;
            case Instructions.LeftShift:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i << i1);
                break;
            case Instructions.MoveToRegister:
                DecodeAlu(instruction, Emulator, variant, (i, i1) => i1);
                break;
        }


        return instruction;
    }

    private void DecodeAlu(Instruction instruction, Emulator emulator, int variant, Func<int, int, int> execution)
    {
        instruction.Fetch = _fetchAlu(instruction, variant, emulator);

        instruction.Execute = () =>
            emulator.Registers[instruction.operand2] =
                execution(emulator.Registers[instruction.operand2], instruction.operand1);
    }


    private void DecodeCompare(Instruction instruction, int variant)
    {
        if (variant == 0)
            instruction.Fetch = () =>
            {
                instruction.operand1 = (int)instruction.GetBits(0, 22);
                instruction.operand2 = Emulator.Registers[instruction.GetBits(23, 25)];
            };
        else
            instruction.Fetch = () =>
            {
                instruction.operand2 = Emulator.Registers[BitOperations.GetRegisterValue((int)instruction.Binary)];
                instruction.operand1 = (int)instruction.GetBits(3, 5);
            };

        instruction.Execute = () =>
        {
            if (instruction.operand1 == instruction.operand2)
            {
                Emulator.Carry = true;
                Emulator.Zero = true;
                return;
            }

            if (instruction.operand2 < instruction.operand1)
            {
                Emulator.Negative = true;
                return;
            }

            Emulator.Carry = true;
        };
    }

    private void DecodeReturn(Instruction instruction)
    {
        instruction.Execute = () => Emulator.Return();
    }

    private void DecodePop(Instruction instruction)
    {
        instruction.Fetch = () => instruction.operand1 = (int)BitOperations.GetRegisterValue((int)instruction.Binary);

        instruction.Execute = () => Emulator.PopStack((uint)instruction.operand1);
    }

    private void DecodePush(Instruction instruction, int variant)
    {
        if (variant == 0)
            instruction.Fetch = () =>
                instruction.operand1 = Emulator.Registers[BitOperations.GetRegisterValue((int)instruction.Binary)];
        else
            instruction.Fetch = () =>
                instruction.operand1 = (int)instruction.GetBits(0, 25);

        instruction.Execute = () => Emulator.PushToStack((uint)instruction.operand1);
    }

    private void DecodeJump(Instruction instruction)
    {
        instruction.Fetch = () =>
            instruction.operand1 = (int)instruction.GetBits(0, 25);

        instruction.Execute = () => Emulator.Jump((uint)instruction.operand1);
    }


    private void DecodeStore(Instruction instruction, int variant)
    {
        instruction.Fetch = _fetchStoreLoad(instruction, variant);

        instruction.Execute = () =>
        {
            Emulator.Memory.Write((ushort)instruction.operand1, (uint)instruction.operand2);
        };
    }

    private void DecodeLoad(Instruction instruction, int variant)
    {
        instruction.Fetch = _fetchStoreLoad(instruction, variant);

        instruction.Execute = () =>
        {
            Emulator.Registers[instruction.operand2] = (int)Emulator.Memory.Read((ushort)instruction.operand1);
        };
    }

    private void DecodeOutput(Instruction instruction)
    {
        instruction.Fetch = _ioFetch(instruction);

        instruction.Execute = () =>
        {
            Emulator.Io.Write((uint)instruction.operand1, Emulator.Registers[instruction.operand2]);
        };
    }

    private void DecodeInput(Instruction instruction)
    {
        instruction.Fetch = _ioFetch(instruction);
        instruction.Execute = () =>
        {
            var input = Emulator.Io.Read((uint)instruction.operand1);
            Emulator.Registers[instruction.operand1] = input;
        };
    }
}