using System.Collections.Generic;
using CommandLine;

namespace git_untrack_common
{
    public abstract class Options
    {
        [ValueList(typeof(List<string>))]
        public List<string> Paths { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [Option('d', "dry-run", HelpText = "Don't actually effect anything")]
        public bool DryRun { get; set; }

        public abstract bool Clean { get; set; }
        public abstract bool Temporary { get; set; }
    }
}