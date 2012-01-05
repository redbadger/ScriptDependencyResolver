namespace RedBadger.Web.ScriptDependencyResolver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using QuickGraph;
    using QuickGraph.Algorithms;

    using RedBadger.Web.ScriptDependencyResolver.Extensions;

    public class Resolver : IResolver
    {
        private static readonly Regex DependencyRegex = new Regex(
            @"///\s*<reference\s*path\s*=\s*""(?<path>.*)""\s*/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string applicationRoot;
        private readonly string scriptFilePattern;
        private readonly string scriptsDirectory;

        public Resolver(string applicationRoot, string scriptsDirectory, string scriptFilePattern)
        {
            applicationRoot.ThrowIfNullOrWhiteSpace("applicationRoot");
            scriptsDirectory.ThrowIfNullOrWhiteSpace("scriptsDirectory");
            scriptFilePattern.ThrowIfNullOrWhiteSpace("scriptFilePattern");

            this.applicationRoot = applicationRoot;
            this.scriptsDirectory = scriptsDirectory;
            this.scriptFilePattern = scriptFilePattern;
        }

        /// <summary>
        /// Returns a list of javascript files, ordered according to their dependencies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Resolve()
        {
            return this.Resolve(file => true);
        }

        /// <summary>
        /// Returns a list of javascript files, ordered according to their dependencies
        /// </summary>
        /// <param name="filterPredicate">A predicate which can be used to filter the input files</param>
        /// <returns></returns>
        public IEnumerable<string> Resolve(Func<string, bool> filterPredicate)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(
                this.applicationRoot.AppendPath(this.scriptsDirectory), 
                this.scriptFilePattern,
                SearchOption.AllDirectories);

            FileInfo[] fileInfos =
                files.Where(filterPredicate).Select(file => new FileInfo(file.ToLowerInvariant())).ToArray();

            var edges = new HashSet<Edge<string>>();
            var scripts = new List<string>();

            foreach (FileInfo source in fileInfos)
            {
                string script = File.ReadAllText(source.FullName);
                scripts.Add(source.FullName);

                if (source.Directory != null)
                {
                    string directory = source.Directory.FullName;
                    Debug.Assert(directory != null, "directory != null");

                    Match match = DependencyRegex.Match(script);                    
                    while (match.Success)
                    {
                        string path = match.Groups["path"].Value;
                        string rootPath = path.StartsWith("~") || path.StartsWith("/")
                                              ? this.applicationRoot
                                              : directory;
                        var target = new FileInfo(rootPath.AppendPath(path));
                        if (!target.Exists)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    "The script file \"{0}\" has a reference (\"{1}\") that points to a non-existant file",
                                    source.FullName,
                                    path));
                        }

                        edges.Add(new Edge<string>(source.FullName, target.FullName.ToLowerInvariant()));
                        
                        match = match.NextMatch();
                        
                    }
                }
            }

            IEnumerable<string> sortedEdges;
            try
            {
                sortedEdges = edges.ToAdjacencyGraph<string, Edge<string>>(false).TopologicalSort();
            }
            catch (NonAcyclicGraphException exception)
            {
                throw new InvalidOperationException("A script file has a circular dependency", exception);
            }
            
            var sortedScripts = sortedEdges.Reverse().Where(scripts.Contains).ToList();
            var unsorted = scripts.Where(s => !sortedScripts.Contains(s)); 
            return sortedScripts.Concat(unsorted);
        }
    }
}