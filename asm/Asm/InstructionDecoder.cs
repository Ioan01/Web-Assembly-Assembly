using MudBlazor;

namespace asm.Asm
{

    public class InstructionDecoder
	{
        public Instruction DecodeInstruction(uint instructionBinary)
        {
            var opcode = BitOperations.GetBits((int)instructionBinary, 31, 26);

            var instructionType = "HLT";
            var variant = 0;

            var instruction = new Instruction();


            if (Instructions.OpcodesReverse.ContainsKey(opcode))
            {
                instructionType = Instructions.OpcodesReverse[opcode];
            }
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

            switch (instructionType)
            {
                case Instructions.Output:
                    DecodeOutput(instruction);
                    break;
                    case Instructions.Input:
                        DecodeInput(instruction);
                        break;
                case Instructions.LoadRegister:
                    DecodeLoad(instruction,variant);
                    break;
                case Instructions.StoreRegister:
                    DecodeStore(instruction,variant);
                    break;
                case Instructions.Halt:
                    DecodeHalt(instruction);
                    break;
                case Instructions.JumpToSubroutine:
                    DecodeJump(instruction);
                    break;
                case Instructions.PushToStack:
                    DecodePush(instruction, variant);
                    break;
                case Instructions.PopFromStack:
                    DecodePop(instruction, variant);
                    break;
                case Instructions.ReturnFromSubroutine:
                    DecodeReturn(instruction);
                    break;
                case Instructions.Compare:
                    DecodeCompare(instruction, variant);
                    break;
                case Instructions.BranchAlways:
                    DecodeBranch(instruction);
                    break;




            }

            return instruction;
        }

        // operand 1 is address to load/store
        // operand 2 is source/destionation register
        private Func<Instruction, int, Action> _fetchStoreLoad = (instruction, variant) => () =>
        {
            var bits = (int)instruction.Binary;


            switch (variant)
            {
                case 0:
                    instruction.operand1 = BitOperations.GetBits(bits, 0, 22);
                    instruction.operand2 = BitOperations.GetBits(bits, 23, 25);
                    break;
                case 1:
                    instruction.operand1 = BitOperations.GetRegisterValue(bits);
                    instruction.operand2 = BitOperations.GetBits(bits, 3, 5);
                    break;
                case 2:
                    instruction.operand1 = BitOperations.GetRegisterValue(bits) + BitOperations.GetBits(bits,3,22);
                    instruction.operand2 = BitOperations.GetBits(bits, 23, 25);
                    break;
            }
        };


        private void DecodeStore(Instruction instruction, int variant)
        {
            instruction.Fetch = _fetchStoreLoad(instruction, variant);

            instruction.Execute = () =>
            {
                Emulator.Memory.Write((ushort)instruction.operand1,instruction.operand2);
            };
        }

        private void DecodeLoad(Instruction instruction, int variant)
        {
            instruction.Fetch = _fetchStoreLoad(instruction,variant);

            instruction.Execute = () =>
            {
                Emulator.Registers[instruction.operand2] = (int)Emulator.Memory.Read((ushort)instruction.operand1);
            };
        }

        private readonly Func<Instruction, Action> _ioFetch = instruction => () =>
        {
            instruction.operand1 = BitOperations.GetBits((int)instruction.Binary, 0, 7);
            instruction.operand2 = BitOperations.GetBits((int)instruction.Binary, 8, 10);
        };

        private void DecodeOutput(Instruction instruction)
        {
            instruction.Fetch = _ioFetch(instruction);

            instruction.Execute = () =>
            {
                Emulator.Io.Write(instruction.operand1, Emulator.Registers[instruction.operand2]);
            };
        }

        private void DecodeInput(Instruction instruction)
        {
            instruction.Fetch = _ioFetch(instruction);
            instruction.Execute = () =>
            {
                var input = Emulator.Io.Read(instruction.operand1);
                Emulator.Registers[instruction.operand1] = input;
            };
        }

        public Emulator Emulator { get; set; }
    }
}
