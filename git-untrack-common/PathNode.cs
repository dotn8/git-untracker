namespace git_untrack_common
{
    internal class PathNode
    {
        public PathNode(string path, bool isIncluded)
        {
            Path = path;
            IsIncluded = isIncluded && System.IO.Path.GetFileName(path) != Utility.GitUntrackFileName;
        }

        public string Path { get; }
        public bool IsIncluded { get; }
    }
}