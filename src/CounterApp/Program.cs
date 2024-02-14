using Counter.Core;

namespace CounterApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Counter <inputFile> <outputFile>");
                return;
            }

            if (!args.All(a => a.EndsWith(".txt")))
            {
                Console.WriteLine("Only .txt files are supported");
                return;
            }

            if (args[0] == args[1])
            {
                Console.WriteLine("Input file and Output file can not be same");
                return;
            }

            try
            {
                var processor = new WordFrequencyProcessor(new FileProvider(), new ParallelWordCounter());

                await processor.ProcessFileAsync(args[0], args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
