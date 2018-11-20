using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace IEvangelist.DotNet.Miglifier
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                return await CommandLineApplication.ExecuteAsync<Miglifier>(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Unexpected error: {ex}");
                Console.ResetColor();

                return 1;
            }
        }
    }
}