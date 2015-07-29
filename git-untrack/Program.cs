using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bcl.Community.Extensions;
using CommandLine;

namespace git_untrack
{
    public interface IDoOptions
    {
        IList<string> Paths { get; }
    }

    [Verb("redo", HelpText = "Reapplies the state of .gituntrack")]
    public class RedoOptions : IDoOptions
    {
        [Value(0)]
        public IList<string> Paths { get; set; }
    }

    [Verb("undo", HelpText = "Reinstates tracking for all files listed in .gituntrack")]
    public class UndoOptions : IDoOptions
    {
        [Value(0)]
        public IList<string> Paths { get; set; }
    }

    public enum ProcessVerb
    {
        Undo,Redo
    }

    public class PathNode
    {
        public PathNode(string path, bool isIncluded)
        {
            Path = path;
            IsIncluded = isIncluded;
        }

        public string Path { get; }
        public bool IsIncluded { get; }
    }

    internal class Program
    {
        public const string GitUntrackFileName = ".gituntrack";

        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<RedoOptions, UndoOptions>(args)
                .Return(
                    (RedoOptions opts) => Process(opts, ProcessVerb.Redo),
                    (UndoOptions opts) => Process(opts, ProcessVerb.Undo),
                    errs => 1);
        }

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

        private static int Process(IDoOptions opts, ProcessVerb verb)
        {
            var pathsToProcess = EnumeratePathsToProcess(opts.Paths.ToList()).ToList();

            foreach (var pathToProcess in pathsToProcess)
            {
                switch (verb)
                {
                    case ProcessVerb.Redo:
                        {
                            var proc = System.Diagnostics.Process.Start("git.exe", $"update-index --assume-unchanged \"{pathToProcess}\"");
                            proc.WaitForExit();
                            return proc.ExitCode;
                        }
                    case ProcessVerb.Undo:
                        {
                            var proc = System.Diagnostics.Process.Start("git.exe", $"update-index --no-assume-unchanged \"{pathToProcess}\"");
                            proc.WaitForExit();
                            return proc.ExitCode;
                        }
                }
            }

            return 0;
        }

        private static IEnumerable<string> EnumeratePathsToProcess(IReadOnlyList<string> paths)
        {
            if (paths.Count == 0)
            {
                return Environment.CurrentDirectory.EnumerateTreeRecursively(EnumeratePathToProcess, EnumerableExtensions.TreeTraversalOrder.PostOrder)
                    .Select(strs => strs[strs.Count - 1])
                    .Where(File.Exists)
                    .Where(str => Path.GetFileName(str) != GitUntrackFileName)
                    .Distinct();
            }
            return paths
                .SelectMany(path =>
                    path.EnumerateTreeRecursively(EnumeratePathToProcess,
                        EnumerableExtensions.TreeTraversalOrder.PostOrder))
                .Select(strs => strs[strs.Count - 1])
                .Where(File.Exists)
                .Where(str => Path.GetFileName(str) != GitUntrackFileName)
                .Distinct();
        }

        private static IEnumerable<string> EnumeratePathToProcess(Stack<string> pathsStack)
        {
            var path = pathsStack.Peek();

            if (path == "-")
            {
                return ReadLinesFromInput()
                    .Where(str => !string.IsNullOrWhiteSpace(str));
            }

            if (Directory.Exists(path))
            {
                return Directory.EnumerateFileSystemEntries(path);
            }

            if (!File.Exists(path))
                return EnumerableUtility.EmptyArray<string>();

            if (Path.GetFileName(path) == GitUntrackFileName)
            {
                return File.ReadLines(path)
                    .Where(str => !string.IsNullOrWhiteSpace(str));
            }

            return EnumerableUtility.EmptyArray<string>();
        }
    }
}