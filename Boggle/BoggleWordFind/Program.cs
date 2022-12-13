using System.CommandLine;
using static BoggleWordFind.Utilities;

namespace BoggleWordFind
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            int returnCode = 0;

            var heightOption = new Option<int>(
                name: "--height",
                description: "Enter height of boggle board height.",
                getDefaultValue: () => 3);

            var widthOption = new Option<int>(
                name: "--width",
                description: "Enter width of boggle board.",
                getDefaultValue: () => 3);

            var contentsOption = new Option<string>(
                name: "--contents",
                description: "Enter board contents, string should be the minimum length of board size.  Contents will be split based on width/height")
            {
                IsRequired = true,
            };

            var dictionaryFileOption = new Option<string>(
                name: "--dictionaryTextFile",
                description: "Text file that contains words to use for search. Each word must be on a seperate line",
                getDefaultValue: () => "Dictionary\\words.txt")
            {
                IsHidden = true,
            };

            var rootCommand = new RootCommand("Boggle WordFind");
            rootCommand.AddOption(widthOption);
            rootCommand.AddOption(heightOption);
            rootCommand.AddOption(contentsOption);
            rootCommand.AddOption(dictionaryFileOption);

            rootCommand.SetHandler(
                (widthOptionValue,
                heightOptionValue,
                contentsOptionValue,
                dictionaryFileOptionValue) =>
            {
                returnCode = SetupBoard(
                    widthOptionValue,
                    heightOptionValue,
                    contentsOptionValue,
                    dictionaryFileOptionValue);
            },
            heightOption,
            widthOption,
            contentsOption,
            dictionaryFileOption);

            await rootCommand.InvokeAsync(args);

            return returnCode;
        }

        /// <summary>
        /// Setups the board.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="dictionaryFile">The dictionary file.</param>
        /// <returns></returns>
        private static int SetupBoard(int width, int height, string contents, string dictionaryFile)
        {
            if (contents.Length != (width * height))
            {
                Console.WriteLine($"Contents size of {contents.Length} != {(height * width)}.");
                return -1;
            }

            if (!File.Exists(dictionaryFile))
            {
                Console.WriteLine($"{dictionaryFile} not found.");
                return -1;
            }

            try
            {
                char[][] contentsArray = CreateContentsArray(contents, width, height);
                List<string> words = File.ReadAllLines(dictionaryFile).ToList();
                var wordsFound = new List<string>();
                var boggle = new BoggleTestable();

                Console.WriteLine("SetValidWordSource");
                TimeIt(() =>
                {
                    boggle.SetValidWordSource(words);
                });

                Console.WriteLine("SetBoggleBoardState");
                TimeIt(() =>
                {
                    boggle.SetBoggleBoardState(width, height, contentsArray);
                });

                Console.WriteLine("GetFoundWords");
                TimeIt<List<string>>(() =>
                {
                    return wordsFound = boggle.GetFoundWords().ToList();
                });

                Console.WriteLine($"Words Found: {wordsFound.Count}");
                foreach (var word in wordsFound)
                {
                    Console.Write(word);
                    if (word != wordsFound.Last())
                    {
                        Console.Write(", ");
                    }
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            return 0;
        }
    }
}