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

        [HelpOption]
        public string GetUsage()
        {
            var version = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).First();

            var help = new HelpText
            {
                Heading = new HeadingInfo("git-retrack", version.InformationalVersion),
                Copyright = new CopyrightInfo("Apocalyptic Octopus", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("MIT License");
            help.AddPreOptionsLine("Usage: git retrack");
            help.AddOptions(this);
            return help;
        }
    }
}