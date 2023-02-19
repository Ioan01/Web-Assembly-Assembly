namespace asm.Asm
{
	public class Memory
	{
		private uint[] words = new uint[256 * 256];

		public uint Read(UInt16 address)
		{
			ErrorHandler.VerifyAddress(address);
			return words[address];
		}

		public void Write(UInt16 address, uint value)
		{
			ErrorHandler.VerifyAddress(address);
			words[address] = value;
		}

		public void Write(UInt16 startingAdress,byte len, byte[] bytes)
		{
			 
		}
		
		


	}
}
