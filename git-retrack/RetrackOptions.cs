using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using git_untrack_common;

namespace git_retrack
{
    public class RetrackOptions : Options
    {
        [Option('c', "clean", HelpText = "Retrack everything")]
        public override bool Clean { get; set; }

        [Option('t', "temporary", HelpText = "Does not remove the specified files from any .gituntrack files. This means that the specified files can only be guaranteed to be tracked until the next git untrack or git retrack command.")]
        public override bool Temporary { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var version = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).First();

            var help = new HelpText
            {
                Heading = new HeadingInfo("git-retrack", version.InformationalVersion),
                Copyright = new CopyrightInfo("Apocalyptic Octopus", 2015),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("MIT License");
            help.AddPreOptionsLine("Usage: git retrack");
            help.AddOptions(this);
            return help;
        }
    }
}