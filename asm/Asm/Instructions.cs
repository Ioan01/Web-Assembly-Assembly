using Microsoft.AspNetCore.Components.Forms;

namespace asm.Asm
{
    public static class Instructions
    {
        public const string Input = "INP";
        public const string Output = "OUT";
        public const string LoadRegister = "LDR";
        public const string StoreRegister = "STR";
        public const string Halt = "HLT";
        public const string JumpToSubroutine = "JMS";
        public const string PushToStack = "PSH";
        public const string PopFromStack = "POP";
        public const string ReturnFromSubroutine = "RET";
        public const string Compare = "CMP";
        public const string BranchAlways = "BRA";
        public const string BranchIfEqual = "BEQ";
        public const string BranchIfZero = "BRZ";
        public const string BranchIfMinus = "BMI";
        public const string BranchIfPlus = "BPL";
        public const string BranchIfGreaterThan = "BGT";
        public const string BranchIfLessThan = "BLT";
        public const string Add = "ADD";
        public const string Subtract = "SUB";
        public const string Multiply = "MUL";
        public const string Divide = "DIV";
        public const string Modulo = "MOD";
        public const string BitwiseAnd = "AND";
        public const string BitwiseOr = "OR";
        public const string BitwiseExclusiveOr = "XOR";
        public const string RightShift = "RSHIFT";
        public const string LeftShift = "LSHIFT";
        public const string MoveToRegister = "MOV";

        public static readonly Dictionary<string, uint> Opcodes = new Dictionary<string, uint>
        {
            {Input,0},
            {Output,1},
            {LoadRegister,2},
            {StoreRegister,5},
            {Halt,8},
            {JumpToSubroutine,9},
            {PushToStack,10},
            {PopFromStack,12},
            {ReturnFromSubroutine,13},
            {Compare,14},
            {BranchAlways,16},
            {BranchIfEqual,17},
            {BranchIfMinus,18},
            {BranchIfPlus,19},
            {BranchIfGreaterThan,20},
            {BranchIfLessThan,22},
            {Add,23},
            {Subtract,25},
            {Multiply,27},
            {Divide,29},
            {Modulo,31},
            {BitwiseAnd,33},
            {BitwiseOr,35},
            {BitwiseExclusiveOr,37},
            {RightShift,39},
            {LeftShift,41},
            {MoveToRegister,43},
        };

        public static readonly Dictionary<uint, string> OpcodesReverse =
            Opcodes.ToDictionary(pair => pair.Value, pair => pair.Key);



    }
}
