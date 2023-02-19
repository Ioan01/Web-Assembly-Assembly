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
                    _26Bits = EncodePSH(instruction, ref variant);
                    break;
                case "POP":
                    _26Bits = (uint)(BitOperations.GetBits(instruction.Arguments[0].Value,0,2));
                    break;
                case "CMP":
                    _26Bits = EncodeCMP(instruction, ref variant);
                    break;
                case "BRA":
                case "JMS":
                case "BEQ":
                case "BRZ":
                case "BMI":
                case "BPL":
                case "BLT":
                    _26Bits = (uint)(BitOperations.GetBits(instruction.Arguments[0].Value,0,25));
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
                    break;
            }

        var binary = ((Opcodes[instruction.Alias] + variant) << 26) | _26Bits;
        Console.WriteLine(Convert.ToString(binary,2));

        return binary;
        }

        private uint EncodeIO(IntermediaryInstruction instruction, ref uint variant)
        {
            return (((uint)BitOperations.GetBits(instruction.Arguments[0].Value,0,2)) << 8)
                   | ((uint)BitOperations.GetBits(instruction.Arguments[1].Value,0,7));
        }


        private uint EncodeLoadAndStore(IntermediaryInstruction instruction, ref uint variant)
        {
            // LDR/STR Rx,Raddress, variant 1
            if (instruction.Arguments[1].ValueType == Type.Register)
            {
                variant = 1;
                return ((uint)((BitOperations.GetBits(instruction.Arguments[0].Value,0,2)) << 3)) 
                       | ((uint)(BitOperations.GetBits(instruction.Arguments[1].Value,0,2)));
            }
            else if (instruction.Arguments[2].ValueType == Type.RegisterOffset)
            {
                variant = 2;

                // offset register 
                return BitOperations.GetBits(instruction.Arguments[2].Value, 0, 2) |
                       (BitOperations.GetBits(instruction.Arguments[1].Value, 0, 19) << 3) |
                       (BitOperations.GetBits(instruction.Arguments[0].Value, 0, 2) << 23);

            }
            // LDR Rx,immediate
            else
            {
                return (uint)BitOperations.GetBits(instruction.Arguments[1].Value, 0, 22) |
                       ((uint)BitOperations.GetBits(instruction.Arguments[0].Value,0,2) << 23);
            }
            
        }


    }
}
