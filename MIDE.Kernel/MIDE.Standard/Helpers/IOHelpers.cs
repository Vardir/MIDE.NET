namespace MIDE.Helpers
{
    public static class IOHelpers
    {
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            var normalizedPath = path.Replace('/', '\\');
            int lastSlashIndex = normalizedPath.LastIndexOf('\\');
            if (lastSlashIndex <= 0) return path;

            return path.Substring(lastSlashIndex + 1);
        }
    }
}