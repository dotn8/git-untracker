using System;
using System.Linq;
using System.Reflection;
using CommandLine;
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
                {
                    var version = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).First();
                    Console.WriteLine($"git-untrack {version.InformationalVersion}");
                }
                Utility.Process(options, ProcessVerb.Untrack);
            }
        }
    }
}