namespace Shared.Asm;

public enum EmulatorState
{
    Idle,
    Ready,
    Loading,
    Running
}

public class Emulator
{
    private readonly CodeProcessor _codeProcessor;
    private readonly InstructionDecoder _instructionDecoder;

    private readonly InstructionEncoder _instructionEncoder;

    private readonly ErrorHandler _errorHandler;


    public Emulator(InstructionDecoder instructionDecoder, InstructionEncoder instructionEncoder,
        CodeProcessor codeProcessor, ErrorHandler errorHandler, Action? seedEmulator = null)
    {
        _instructionDecoder = instructionDecoder;

        _instructionDecoder.Emulator = this;

        _instructionEncoder = instructionEncoder;


        _codeProcessor = codeProcessor;
        _errorHandler = errorHandler;


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
        var instructions = _codeProcessor.ProcessInstructions(code);

        ushort index = 0;

        Memory.Reset();

        instructions.ForEach(instruction =>
        {
            var instructionBinary = _instructionEncoder.EncodeInstruction(instruction);
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
            var decodedInstruction = _instructionDecoder.DecodeInstruction(instruction);

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
            var decodedInstruction = _instructionDecoder.DecodeInstruction(instruction);

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
        _errorHandler.VerifyAddress(address);

        LinkRegister = ProgramCounter + 1;

        ProgramCounter = address - 1;
    }

    public void PushToStack(uint value)
    {
        _errorHandler.ValidatePush(StackPointer);

        StackPointer++;

        Memory.Write(StackPointer, value);
    }

    public void PopStack(uint destinationRegister)
    {
        _errorHandler.ValidatePop(StackPointer);
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
        _errorHandler.VerifyAddress((uint)(ProgramCounter + relativeAddress));

        ProgramCounter = (uint)(ProgramCounter + relativeAddress) - 1;
    }
}