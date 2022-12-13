namespace BoggleWordFind.UnitTests
{
    internal class Utilities
    {
        /// <summary>Generate a random char</summary>
        /// <param name="chars">The chars.</param>
        /// <param name="seed"></param>
        /// <returns>char</returns>
        internal static char GetRandomChar(string chars, int? seed = null)
        {
            Random rnd = seed == null ? new Random() : new Random((int)seed);
            var index = rnd.Next(0, chars.Length);
            return chars[index];
        }

        /// <summary>Generate a random char array</summary>
        /// <param name="chars">The chars.</param>
        /// <param name="size"></param>
        /// <param name="seed"></param>
        /// <returns>char[]</returns>
        internal static char[] GenerateRandomCharArray(string chars, int size, int? seed = null)
        {
            char[] charArray = new char[size];
            Random rnd = seed == null ? new Random() : new Random((int)seed);
            int index = 0;

            for (int i = 0; i < size; i++)
            {
                index = rnd.Next(0, chars.Length);
                charArray[i] = chars[index];
            }

            return charArray;
        }
    }
}
