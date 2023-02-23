namespace asm.Asm
{
    public struct Instruction
    {
        public readonly uint Binary { get; }


        public uint operand1;
        public uint operand2;
        public uint operand3;

        public Instruction(uint instructionBinary, uint binary)
        {
            Binary = binary;
        }

        // fetch operands
        public Action Fetch { get; set; }


        //execute action on operands
        public Action Execute { get; set; }

        public void Run()
        {
            if (Fetch != null)
                Fetch();

            Execute();
        }


        public uint GetBits(int from, int to)
        {
            return BitOperations.GetBits((int)Binary, from, to);
        }
    }
}
