namespace asm.Asm;

public enum EmulatorState
{
    Idle,
    Ready,
    Loading,
    Running
}

public class Emulator
{
    private readonly CodeProcessor codeProcessor;
    private readonly InstructionDecoder instructionDecoder;

    private readonly InstructionEncoder instructionEncoder;


    public Emulator(InstructionDecoder instructionDecoder, InstructionEncoder instructionEncoder,
        CodeProcessor codeProcessor, Action? seedEmulator = null)
    {
        this.instructionDecoder = instructionDecoder;

        this.instructionDecoder.Emulator = this;

        this.instructionEncoder = instructionEncoder;


        this.codeProcessor = codeProcessor;


        seedEmulator?.Invoke();
    }

    public EmulatorState State { get; set; } = EmulatorState.Idle;


    /// <summary>
    ///     External devices
    /// </summary>

    public Memory Memory { get; set; } = new();

    public IO Io { get; set; } = new();


    /// <summary>
    ///     Registers
    /// </summary>

    public int[] Registers { get; set; } = new int[8];

    public uint LinkRegister { get; set; }

    public uint ProgramCounter { get; set; }

    public uint StackPointer { get; set; } = Memory.Size;


    public bool Zero { get; set; }
    public bool Carry { get; set; }
    public bool Overflow { get; set; }
    public bool Negative { get; set; }

    public bool Stopped { get; set; }

    public int Delay { get; set; } = 1;

    public string LastInstruction { get; set; } = "NOP";

    public void LoadEmulator(string code)
    {
        var instructions = codeProcessor.ProcessInstructions(code);

        ushort index = 0;

        Memory.Reset();

        instructions.ForEach(instruction =>
        {
            var instructionBinary = instructionEncoder.EncodeInstruction(instruction);
            Memory.Write(index++, instructionBinary);
        });


        State = EmulatorState.Ready;
    }

    public async Task Run()
    {
        ProgramCounter = 0;
        Stopped = false;
        while (!Stopped)
        {
            var instruction = Memory.Read(ProgramCounter);
            var decodedInstruction = instructionDecoder.DecodeInstruction(instruction);

            decodedInstruction.Fetch?.Invoke();
            decodedInstruction.Execute();

            await Task.Delay(Delay);
            ProgramCounter++;
        }


        State = EmulatorState.Idle;
    }

    public void Reset()
    {
        ProgramCounter = 0;
        Stopped = false;

        LastInstruction = "NOP";

        for (var i = 0; i < Registers.Length; i++) Registers[i] = 0;

        Zero = false;
        Carry = false;
        Overflow = false;
        LinkRegister = 0;

        StackPointer = Memory.Size;

        State = EmulatorState.Running;
    }

    public async Task<bool> RunNext()
    {
        if (!Stopped)
        {
            var instruction = Memory.Read(ProgramCounter);
            var decodedInstruction = instructionDecoder.DecodeInstruction(instruction);

            decodedInstruction.Fetch?.Invoke();
            decodedInstruction.Execute();

            LastInstruction = decodedInstruction.Type;

            ProgramCounter++;

            await Task.Delay(Delay);
            return true;
        }

        State = EmulatorState.Ready;

        return false;
    }


    public void Stop()
    {
        Stopped = true;

        State = EmulatorState.Ready;

        LastInstruction = "HLT";
    }


    public void Jump(uint address)
    {
        ErrorHandler.VerifyAddress(address);

        LinkRegister = ProgramCounter + 1;

        ProgramCounter = address - 1;
    }

    public void PushToStack(uint value)
    {
        ErrorHandler.ValidatePush(StackPointer);

        StackPointer++;

        Memory.Write(StackPointer, value);
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

    public void Branch(int relativeAddress)
    {
        ErrorHandler.VerifyAddress((uint)(ProgramCounter + relativeAddress));

        ProgramCounter = (uint)(ProgramCounter + relativeAddress) - 1;
    }
}