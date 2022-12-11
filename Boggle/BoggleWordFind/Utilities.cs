using System.Diagnostics;

namespace BoggleWordFind
{
    internal class Utilities
    {
        /// <summary>Times it.</summary>
        /// <param name="action">The action.</param>
        internal static void TimeIt(Action action)
        {
            TimeIt<int>(
                () =>
                {
                    action();
                    return 0;
                });
        }

        /// <summary>Times it.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        internal static T TimeIt<T>(Func<T> func)
        {
            T result = default(T);

            var stopwatch = Stopwatch.StartNew();
            result = func();
            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed}\r\n");

            return result;
        }

        /// <summary>Gets the random character.</summary>
        /// <param name="chars">The chars.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>char[][]</returns>
        internal static char[][] CreateContentsArray(string chars, int width, int height)
        {
            char[][] charArray = new char[height][];
            var index = 0;

            for (int r = 0; r < height; r++)
            {
                charArray[r] = new char[width];
                for (int c = 0; c < width; c++)
                {
                    charArray[r][c] = chars[index];
                    index++;
                }
            }

            return charArray;
        }
    }
}
