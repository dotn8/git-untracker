namespace git_untrack_common
{
    internal class PathNode
    {
        public PathNode(string path, bool isIncluded)
        {
            Path = path;
            IsIncluded = isIncluded && System.IO.Path.GetFileName(path) != Utility.GitUntrackFileName;
        }

        public PathNode(string path, string gitUntrackFile, int gitUntrackFileLineNumber)
        {
            Path = path;
            IsIncluded = true;
            GitUntrackFile = gitUntrackFile;
            GitUntrackFileLineNumber = gitUntrackFileLineNumber;
        }

        public string Path { get; }
        public bool IsIncluded { get; }
        public string GitUntrackFile { get; }
        public int GitUntrackFileLineNumber { get; }
    }
}