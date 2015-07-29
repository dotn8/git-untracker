using System;
using System.Collections.Generic;
using CommandLine;
using git_untrack_common;

namespace git_retrack
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new RetrackOptions();
            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Verbose)
                    Console.WriteLine("git-retrack 0.0.1-alpha");
                Utility.Process(options, ProcessVerb.Retrack);
            }
            else
            {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }
    }
}
