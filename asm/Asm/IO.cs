namespace asm.Asm
{
	public class IO
	{
        public void Write(uint port, int data)
        {
            Console.WriteLine($"Printed to port {port} data : {data}");
        }

        public int Read(uint instructionOperand1)
        {
            return new Random().Next();
        }
    }
}
