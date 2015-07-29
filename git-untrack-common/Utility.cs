using Bcl.Community.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace git_untrack_common
{
    public class Options
    {
        [ValueList(typeof(List<string>))]
        public List<string> Paths { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [Option("dry-run", HelpText = "Don't actually effect anything")]
        public bool DryRun { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("git-untrack");
            return usage.ToString();
        }
    }


    public enum ProcessVerb
    {
        Retrack, Untrack
    }

    internal class PathNode
    {
        public PathNode(string path, bool isIncluded)
        {
            Path = path;
            IsIncluded = isIncluded;
        }

        public string Path { get; }
        public bool IsIncluded { get; }
    }

    public class Utility
    {
        public const string GitUntrackFileName = ".gituntrack";

        private static IEnumerable<string> ReadLinesFromInput()
        {
            while (true)
            {

                var line = Console.ReadLine();
                if (line == null)
                    break;
                yield return line;
            }
        }

        private static string GetGitExe()
        {
            var enviromentPath = Environment.GetEnvironmentVariable("PATH");
            var paths = enviromentPath.Split(';');
            var exePath = paths
                   .Select(x => Path.Combine(x, "git.exe"))
                   .FirstOrDefault(File.Exists);
            return exePath;
        }

        private static Options _options;

        public static int Process(Options options, ProcessVerb verb)
        {
            _options = options;
            var pathsToProcess = EnumeratePathsToProcess(_options.Paths.Select(str => new PathNode(str, true)).ToList()).ToList();

            if (_options.Verbose)
                Console.WriteLine($"Processing {pathsToProcess.Count} paths");

            var gitExe = GetGitExe();

            foreach (var pathToProcess in pathsToProcess)
            {
                var args = verb == ProcessVerb.Untrack
                    ? $"update-index --assume-unchanged \"{pathToProcess.Path}\""
                    : $"update-index --no-assume-unchanged \"{pathToProcess.Path}\"";
                Console.WriteLine($"git.exe {args}");
                if (!_options.DryRun)
                {
                    var proc = System.Diagnostics.Process.Start(gitExe, args);
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                        return proc.ExitCode;
                }
            }

            return 0;
        }

        private static IEnumerable<PathNode> EnumeratePathsToProcess(IReadOnlyList<PathNode> paths)
        {
            IEnumerable<List<PathNode>> result;
            if (paths.Count == 0)
            {
                result = new PathNode(Environment.CurrentDirectory, false)
                    .EnumerateTreeRecursively(EnumeratePathToProcess, EnumerableExtensions.TreeTraversalOrder.PostOrder);
            }
            else
            {
                result = paths.SelectMany(path =>
                    path.EnumerateTreeRecursively(EnumeratePathToProcess, EnumerableExtensions.TreeTraversalOrder.PostOrder));
            }

            return result.Select(strs => strs[0])
                .Where(node => File.Exists(node.Path))
                .Where(node => Path.GetFileName(node.Path) != GitUntrackFileName)
                .Where(node => node.IsIncluded);
        }

        private static IEnumerable<PathNode> EnumeratePathToProcess(Stack<PathNode> pathsStack)
        {
            var node = pathsStack.Peek();
            var path = node.Path;

            var nodeChar = node.IsIncluded ? "T" : "F";

            if (_options.Verbose)
                Console.WriteLine($"{nodeChar} {path}");

            if (path == "-")
            {
                return ReadLinesFromInput()
                    .Where(str => !string.IsNullOrWhiteSpace(str))
                    .Select(str => Path.IsPathRooted(str) ? str : $"{Environment.CurrentDirectory}\\{str}")
                    .Select(str => new PathNode(str, true));
            }

            if (Directory.Exists(path))
            {
                return Directory.EnumerateFileSystemEntries(path)
                    .Where(str => Path.GetFileName(str) != ".git")
                    .Select(str => new PathNode(str, node.IsIncluded));
            }

            if (!File.Exists(path))
                return EnumerableUtility.EmptyArray<PathNode>();

            if (Path.GetFileName(path) == GitUntrackFileName)
            {
                return File.ReadLines(path)
                    .Where(str => !string.IsNullOrWhiteSpace(str))
                    .Select(str => Path.IsPathRooted(str) ? str : $"{Path.GetDirectoryName(path)}\\{str}")
                    .Select(str => new PathNode(str, true));
            }

            return EnumerableUtility.EmptyArray<PathNode>();
        }
    }
}
