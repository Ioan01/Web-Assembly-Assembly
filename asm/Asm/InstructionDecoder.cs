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
                    DecodeLoad();
                    break;
                case Instructions.StoreRegister:
                    DecodeStore();
                    break;
            }

            return instruction;
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
