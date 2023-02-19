using System.Runtime.InteropServices;

namespace asm.Asm
{
    public static class BitOperations
    {

        public static int GetBits(int number, int from, int to)
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

            return (int)bits;
        }

        
    }
}
