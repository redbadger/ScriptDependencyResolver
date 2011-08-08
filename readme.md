Script Dependency Resolver
===
Resolves dependencies between JavaScript files based on declared reference paths
---
 * Git core.autocrlf and core.safecrlf should be false
 * The Resolve() method has an overload that takes a predicate which can be used to filter the included script filenames

Example usages:
---

    private const string WebRoot = @"..\..\..\WebProject";

    private static void Main(string[] args)
    {
        using (Stream output = new FileStream(WebRoot.AppendPath("Tests.js"), FileMode.Create, FileAccess.Write))
        {
            var writer = new StreamWriter(output);

            foreach (string script in new Resolver(WebRoot, "~/Scripts", "*.js").Resolve())
            {
                writer.WriteLine(script);
            }

            writer.Flush();
        }
    }

or as an OpenRasta :-) Script Handler:

    public class ScriptHandler : IScriptHandler
    {
        private static readonly string RootDirectory = HttpContext.Current.Server.MapPath("~");

        public OperationResult Get()
        {
            return this.Get(false);
        }

        public OperationResult Get(bool shouldIncludeTests)
        {
            Stream output = new MemoryStream();
            var writer = new StreamWriter(output);

            foreach (string script in
                new Resolver(RootDirectory, "~/Scripts", "*.js").Resolve(
                    file => shouldIncludeTests || !file.EndsWith("tests.js", true, CultureInfo.InvariantCulture)))
            {
                writer.WriteLine(script);
            }

            writer.Flush();
            return
                new OperationResult.OK(
                    new InMemoryFile(output) { ContentType = new MediaType("application/x-javascript") });
        }
    }

