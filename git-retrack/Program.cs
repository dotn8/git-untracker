using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using git_untrack_common;

namespace git_retrack
{
    public class Options
    {
        [ValueList(typeof(List<string>))]
        public List<string> Paths { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("git-untrack");
            return usage.ToString();
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Utility.Process(options.Paths, ProcessVerb.Retrack, options.Verbose);
            }
            else
            {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }
    }
}
