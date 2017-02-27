namespace UIO
{
    public class File
    {
        public static bool Exists(string path)
        {
            return Path.Exists(path, Path.HeadType.File);
        }

        public static void Delete(string uioPath)
        {
            Path.Delete(uioPath, false);
        }
    }
}
