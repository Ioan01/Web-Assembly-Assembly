namespace asm.Asm
{
	
	
	
	public class InstructionEncoder
    {
        private static readonly Dictionary<string, uint> Opcodes = new Dictionary<string, uint>
        {
            {"INP",0},
            {"OUT",1},
            {"LDR",2},
            {"STR",5},
            {"HLT",8},
            {"JMS",9},
            {"PSH",10},
            {"POP",12},
            {"RET",13},
            {"CMP",14},
            {"BRA",16},
            {"BEQ",17},
            {"BMI",18},
            {"BPL",19},
            {"BGT",20},
            {"BLT",22},
            {"ADD",23},
            {"SUB",25},
            {"MUL",27},
            {"DIV",29},
            {"MOD",31},
            {"AND",33},
            {"OR",35},
            {"XOR",37},
            {"RSHIFT",39},
            {"LSHIFT",41},
            {"MOV",43},
        };

		public uint EncodeInstruction(IntermediaryInstruction instruction)
        {
            var variant = 0u;
            var _26Bits = 0u;

            var arguments = instruction.Arguments;

            switch (instruction.Alias)
            {
                case "INP":
                case "OUT":
                    _26Bits = EncodeIO(instruction, ref variant);
                    break;
                case "LDR":
                case "STR":
                    _26Bits = EncodeLoadAndStore(instruction, ref variant);
                    break;
                case "RET":
                case "HLT":
                    break;
                case "PSH":
                    _26Bits = EncodePsh(instruction, ref variant);
                    break;
                case "POP":
                    _26Bits = BitOperations.GetRegisterValue(instruction.Arguments[0].Value);
                    break;
                case "CMP":
                    _26Bits = EncodeCmp(instruction, ref variant);
                    break;
                case "BRA":
                case "JMS":
                case "BEQ":
                case "BRZ":
                case "BMI":
                case "BPL":
                case "BLT":
                    _26Bits = BitOperations.Get26BitImmediateValue(instruction.Arguments[0].Value);
                    break;
                case "ADD":
                case "SUB":
                case "MUL":
                case "DIV":
                case "MOD":
                case "AND":
                case "OR":
                case "XOR":
                case "RSHIFT":
                case "LSHIFT":
                case "MOV":
                    _26Bits = EncodeALU(instruction, ref variant);
                    break;
            }

        var binary = ((Opcodes[instruction.Alias] + variant) << 26) | _26Bits;

        Console.WriteLine(instruction.Alias + instruction.Arguments.Aggregate(" : ",(s1,s2)=>s1 + ' ' +s2));
        Console.WriteLine(BitOperations.ToBinary(binary));

        return binary;
        }

        private uint EncodeALU(IntermediaryInstruction instruction, ref uint variant)
        {
            if (instruction.Arguments[1].ValueType == Type.Register)
            {
                variant = 1;
                return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
                       (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);
            }

            return  BitOperations.GetRegisterValue(instruction.Arguments[1].Value) |
                          (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3);
        }

        private uint EncodeCmp(IntermediaryInstruction instruction, ref uint variant)
        {
            if (instruction.Arguments[1].ValueType == Type.Register)
            {
                variant = 1;
                return (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3) |
                       BitOperations.GetRegisterValue(instruction.Arguments[1].Value);
            }

            return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
                   (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);
        }

        private uint EncodePsh(IntermediaryInstruction instruction, ref uint variant)
        {
            if (instruction.Arguments[0].ValueType == Type.Register)
            {
                return BitOperations.GetRegisterValue(instruction.Arguments[0].Value);
            }

            variant = 1;
            return BitOperations.Get26BitImmediateValue(instruction.Arguments[0].Value);
        }

        private uint EncodeIO(IntermediaryInstruction instruction, ref uint variant)
        {
            return (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 8)
                   | BitOperations.GetBits(instruction.Arguments[1].Value,0,7);
        }


        private uint EncodeLoadAndStore(IntermediaryInstruction instruction, ref uint variant)
        {
            // LDR/STR Rx,Raddress, variant 1
            if (instruction.Arguments[1].ValueType == Type.Register)
            {
                variant = 1;
                return BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 3 
                       | BitOperations.GetRegisterValue(instruction.Arguments[1].Value);
            }

            if (instruction.Arguments[2].ValueType == Type.RegisterOffset)
            {
                variant = 2;

                // offset register 
                return BitOperations.GetRegisterValue(instruction.Arguments[2].Value) |
                       (BitOperations.GetBits(instruction.Arguments[1].Value, 0, 19) << 3) |
                       BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23;

            }
            // LDR Rx,immediate

            return BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
                   (BitOperations.GetRegisterValue(instruction.Arguments[0].Value) << 23);

        }


    }
}
