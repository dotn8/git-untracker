using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using git_untrack_common;

namespace git_untrack
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new UntrackOptions();
            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Verbose)
                    Console.WriteLine("git-untrack 0.0.1-alpha");
                Utility.Process(options, ProcessVerb.Untrack);
            }
            else
            {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }
    }
}