using Bcl.Community.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace git_untrack_common
{
    public static class Utility
    {
        public const string GitUntrackFileName = ".gituntrack";
        private static Options _options;

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

        public static int Process(Options options, ProcessVerb verb)
        {
            _options = options;
            var pathsToProcess = EnumeratePathsToProcess(_options.Paths.Select(str => new PathNode(str, true)).ToList()).ToList();

            if (!_options.Temporary || _options.DryRun)
            {
                if (verb == ProcessVerb.Retrack)
                {
                    SaveRetrackChanges(pathsToProcess);
                }
                else
                {
                    SaveUntrackChanges(pathsToProcess);
                }
            }

            if (_options.Verbose)
                Console.WriteLine($"Processing {pathsToProcess.Count} paths");

            var gitExe = GetGitExe();

            foreach (var pathToProcess in pathsToProcess)
            {
                var toBeTracked = verb == ProcessVerb.Retrack;
                if (!pathToProcess.IsIncluded)
                    toBeTracked = !toBeTracked;

                var args = toBeTracked
                    ? $"update-index --no-assume-unchanged \"{Path.GetFullPath(pathToProcess.Path).RelativeTo(Environment.CurrentDirectory)}\""
                    : $"update-index --assume-unchanged \"{Path.GetFullPath(pathToProcess.Path).RelativeTo(Environment.CurrentDirectory)}\"";
                Console.WriteLine($"git.exe {args}");
                if (!_options.DryRun)
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        Arguments = args,
                        FileName = gitExe,
                        UseShellExecute = false
                    };

                    var proc = System.Diagnostics.Process.Start(startInfo);
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                        return proc.ExitCode;
                }
            }

            return 0;
        }

        private static void SaveUntrackChanges(List<PathNode> pathsToProcess)
        {
            var allExplicitlyUntracked = EnumeratePathsToProcess(
                EnumerableUtility.EmptyArray<PathNode>());
            var pathsToStartUntracking = pathsToProcess.Where(pn => !allExplicitlyUntracked.Any(pn2 =>
                Path.GetFullPath(pn2.Path).Equals(Path.GetFullPath(pn.Path), StringComparison.OrdinalIgnoreCase)))
                .ToList();
            foreach (var pathToStartUntracking in pathsToStartUntracking)
            {
                var gitUntrackFileToAddNewPathTo = Path.Combine(Path.GetDirectoryName(pathToStartUntracking.Path), GitUntrackFileName);
                var lines = File.ReadAllLines(gitUntrackFileToAddNewPathTo)
                    .ConcatItems(Path.GetFileName(pathToStartUntracking.Path))
                    .ToList();
                File.WriteAllLines(gitUntrackFileToAddNewPathTo, lines);
            }
        }

        private static void SaveRetrackChanges(List<PathNode> pathsToProcess)
        {
            var pathsToStopUntrackingGroupedByGitUntrackFile = EnumeratePathsToProcess(
                EnumerableUtility.EmptyArray<PathNode>())
                .Where(
                    pn =>
                        pathsToProcess.Any(
                            pn2 =>
                                Path.GetFullPath(pn2.Path)
                                    .Equals(Path.GetFullPath(pn.Path), StringComparison.OrdinalIgnoreCase)))
                .GroupBy(pn => pn.GitUntrackFile);
            foreach (var group in pathsToStopUntrackingGroupedByGitUntrackFile)
            {
                var groupMembers = group.OrderBy(pn => pn.GitUntrackFileLineNumber).ToList();
                var lines = File.ReadAllLines(group.Key).Where((str, i) => groupMembers.All(pn => pn.GitUntrackFileLineNumber != i)).ToList();
                File.WriteAllLines(group.Key, lines);
            }
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

            if (_options.Clean)
                return result.Select(strs => strs[0])
                    .Where(node => File.Exists(node.Path));
            return result.Select(strs => strs[0])
                .Where(node => File.Exists(node.Path))
                .Where(node => node.IsIncluded);
        }

        /// <summary>
        /// http://www.iandevlin.com/blog/2010/01/csharp/generating-a-relative-path-in-csharp
        /// </summary>
        private static string RelativeTo(this string relTo, string absPath)
        {
            string[] absDirs = absPath.Split('\\');
            string[] relDirs = relTo.Split('\\');

            // Get the shortest of the two paths
            int len = absDirs.Length < relDirs.Length ? absDirs.Length : relDirs.Length;
            // Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            // Find common root
            for (index = 0; index < len; index++)
            {
                if (absDirs[index] == relDirs[index])
                {
                    lastCommonRoot = index;
                }
                else
                {
                    break;
                }
            }

            // If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
            {
                throw new ArgumentException($"Paths do not have a common base; \"{relTo}\" cannot be made relative to \"{absPath}\"");
            }

            // Build up the relative path
            StringBuilder relativePath = new StringBuilder();
            // Add on the ..
            for (index = lastCommonRoot + 1; index < absDirs.Length; index++)
            {
                if (absDirs[index].Length > 0)
                {
                    relativePath.Append("..\\");
                }
            }

            // Add on the folders
            for (index = lastCommonRoot + 1; index < relDirs.Length - 1; index++)
            {
                relativePath.Append(relDirs[index] + "\\");
            }

            relativePath.Append(relDirs[relDirs.Length - 1]);
            return relativePath.ToString();
        }

        private static IEnumerable<PathNode> EnumeratePathToProcess(Stack<PathNode> pathsStack)
        {
            var node = pathsStack.Peek();
            var path = node.Path;

            if (_options.Verbose)
            {
                var nodeChar = node.IsIncluded ? "T" : "F";
                Console.WriteLine($"{nodeChar} {path.RelativeTo(Environment.CurrentDirectory)}");
            }

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
                    .Select((str, i) =>
                    {
                        if (string.IsNullOrWhiteSpace(str))
                            return null;
                        var result = new PathNode(Path.IsPathRooted(str) ? str : $"{Path.GetDirectoryName(path)}\\{str}", path, i);
                        return result;
                    });
            }

            return EnumerableUtility.EmptyArray<PathNode>();
        }
    }
}
