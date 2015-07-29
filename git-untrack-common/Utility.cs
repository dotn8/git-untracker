using Bcl.Community.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace git_untrack_common
{
    public enum ProcessVerb
    {
        Retrack, Untrack
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

        public static int Process(List<string> paths, ProcessVerb verb, bool verbose)
        {
            var pathsToProcess = EnumeratePathsToProcess(paths).ToList();

            var gitExe = GetGitExe();

            foreach (var pathToProcess in pathsToProcess)
            {
                if (verbose)
                    Console.WriteLine(pathToProcess);
                var args = verb == ProcessVerb.Untrack
                    ? $"update-index --assume-unchanged \"{pathToProcess}\""
                    : $"update-index --no-assume-unchanged \"{pathToProcess}\"";
                var proc = System.Diagnostics.Process.Start(gitExe, args);
                proc.WaitForExit();
                return proc.ExitCode;
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
