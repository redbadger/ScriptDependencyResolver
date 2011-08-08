namespace RedBadger.Web.ScriptDependencyResolver.Extensions
{
    using System;
    using System.IO;

    public static class StringExtensions
    {
        public static string AppendPath(this string referencePath, string path)
        {
            path = path.Replace(@"\", "/");
            if (path.StartsWith("~"))
            {
                path = path.Substring(1);
            }

            if (path.StartsWith("/"))
            {
                path = string.Concat(".", path);
            }

            return Path.GetFullPath(Path.Combine(referencePath, path));
        }

        public static void ThrowIfNullOrWhiteSpace(this string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("{0} cannot be null or white space.", name);
            }
        }
    }
}