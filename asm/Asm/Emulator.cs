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

        public EmulatorState State { get; set; } = EmulatorState.Idle;



        /// <summary>
        /// External devices
        /// </summary>

        public Memory Memory { get; set; } = new Memory();

		public IO Io { get; set; } = new IO();



		/// <summary>
		/// Registers
		/// </summary>

        public int[] Registers { get; set; } = new int[8];

        public uint LinkRegister { get; set; } = 0;

        public uint ProgramCounter { get; set; }

        public uint StackPointer { get; set; } = Memory.Size;


        public bool Zero { get; set; }
		public bool Carry { get; set; }
		public bool Overflow { get; set; }
		public bool Negative { get; set; }



        public Emulator(InstructionDecoder instructionDecoder, InstructionEncoder instructionEncoder, CodeProcessor codeProcessor, Action? seedEmulator = null)
		{
			this.instructionDecoder = instructionDecoder;

			this.instructionDecoder.Emulator = this;

			this.instructionEncoder = instructionEncoder;


			this.codeProcessor = codeProcessor;


			// seed memory stuff like that
            seedEmulator?.Invoke();
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

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Jump(uint address)
        {
            ErrorHandler.VerifyAddress(address);

            LinkRegister = ProgramCounter+1;

            ProgramCounter = address;


        }

        public void PushToStack(uint value)
        {
            ErrorHandler.ValidatePush(StackPointer);

            StackPointer++;

			Memory.Write(StackPointer,value);

        }

        public void PopStack(uint destinationRegister)
        {
            ErrorHandler.ValidatePop(StackPointer);
            StackPointer--;

            Registers[destinationRegister] = (int)Memory.Read(StackPointer);
        }

        public void Return()
        {
            ProgramCounter = LinkRegister;

            LinkRegister = 0;

        }

        public void Branch(int get26BitImmediateValue)
        {
        }
    }
}
