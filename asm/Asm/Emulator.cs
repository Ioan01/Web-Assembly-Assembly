namespace asm.Asm
{
	public enum EmulatorState
	{
		Idle,
		Ready,
		Loading,
		Running,
	}
	public class Emulator
	{
		private InstructionDecoder instructionDecoder;

		private InstructionEncoder instructionEncoder;

		private CodeProcessor codeProcessor;


		public Memory Memory { get; set; } = new Memory();

		public IO Io { get; set; } = new IO();

		public EmulatorState State { get; set; } = EmulatorState.Idle;

        public int[] Registers { get; set; } = new int[8];


        public Emulator(InstructionDecoder instructionDecoder, InstructionEncoder instructionEncoder, CodeProcessor codeProcessor)
		{
			this.instructionDecoder = instructionDecoder;

			this.instructionDecoder.Emulator = this;

			this.instructionEncoder = instructionEncoder;


			this.codeProcessor = codeProcessor;
		}
		
		public void LoadEmulator(string code)
		{
			var instructions = codeProcessor.ProcessInstructions(code);
			
			ushort index = 0;

			instructions.ForEach(instruction =>
			{
				var instructionBinary = instructionEncoder.EncodeInstruction(instruction);
				Memory.Write(index++,instructionBinary);
			});


			//State = EmulatorState.Ready;

		}

		public async Task Run()
		{
			await Task.Delay(1000);
			
		}
	}
}
