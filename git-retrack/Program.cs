using System;
using System.Linq;
using System.Reflection;
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
                {
                    var version = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).First();
                    Console.WriteLine($"git-retrack {version.InformationalVersion}");
                }
                Utility.Process(options, ProcessVerb.Retrack);
            }
        }
    }
}
