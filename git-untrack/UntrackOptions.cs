using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using git_untrack_common;

namespace git_untrack
{
    public class UntrackOptions : Options
    {
        [Option('c', "clean", HelpText = "Untrack everything specified and retrack everything else")]
        public override bool Clean { get; set; }

        [Option('t', "temporary", HelpText = "Does not add the specified files to any .gituntrack files, even if it's missing. This means that the specified files can only be guaranteed to be untracked until the next git untrack or git retrack command.")]
        public override bool Temporary { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var version = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute)).First();

            var help = new HelpText
            {
                Heading = new HeadingInfo("git-untrack", version.InformationalVersion),
                Copyright = new CopyrightInfo("Apocalyptic Octopus", 2015),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("MIT License");
            help.AddPreOptionsLine("Usage: git untrack");
            help.AddOptions(this);
            return help;
        }
    }
}