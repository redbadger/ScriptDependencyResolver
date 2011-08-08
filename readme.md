Script Dependency Resolver
===
Resolves dependencies between JavaScript files based on declared reference paths
---
 * Git core.autocrlf and core.safecrlf should be false

Example usage:
---

	private const string WebRoot = @"..\..\..\WebProject";

    private static void Main(string[] args)
    {
        using (Stream output = new FileStream(WebRoot.AppendPath("Tests.js"), FileMode.Create, FileAccess.Write))
        {
            var writer = new StreamWriter(output);

            foreach (string file in
                new ScriptDependencyResolver().Resolve(WebRoot, "~/Scripts", true))
            {
                writer.WriteLine(file);
            }

            writer.Flush();
        }
    }
