using System.Runtime.InteropServices;

namespace asm.Asm
{
    public static class BitOperations
    {

        public static uint GetBits(int number, int from, int to)
        {
            if (from > to)
            {
                throw new ArgumentException("From parameter greater than to parameter.");
            }
            var size = sizeof(int) * 8 - 1;

            // right shift with uint to avoid adding 1's
            var mask = ((~0u) << (from + size - to)) >> (size - to);
            Console.WriteLine(Convert.ToString(mask, 2));


            var bits = (number & mask) >> from;

            return (uint)bits;
        }

        public static uint GetRegisterValue(int number)
        {
            return GetBits(number, 0, 2);
        }

        public static uint Get26BitImmediateValue(int number)
        {
            return GetBits(number, 0, 25);
        }

        public static string ToBinary(uint number)
        {
            var str = Convert.ToString(number, 2);
            while (str.Length != sizeof(int) * 8)
            {
                str = '0' + str;
            }

            return str;
        }

        
    }
}
