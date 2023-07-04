namespace asm.Asm
{
    public class Instruction
    {
        public uint Binary { get; }


        public int operand1;
        public int operand2;
        public int operand3;

        public Instruction(uint binary)
        {
            Binary = binary;
        }

        // fetch operands
        public Action Fetch { get; set; }


        //execute action on operands
        public Action Execute { get; set; }

        public string Type { get; set; }

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
