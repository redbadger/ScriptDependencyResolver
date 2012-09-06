namespace RedBadger.Web.ScriptDependencyResolver.Shell
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Yahoo.Yui.Compressor;

    internal class Program
    {
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
            Environment.Exit(1);
        }

        private static void Main(string[] args)
        {
            SetUnhandledExceptionHandler();
            if (args.Length != 3 && args.Length != 4)
            {
                PrintUsage();
                return;
            }

            string appRoot = args[0];
            string scriptsPath = args[1];
            string outputFile = args[2];

            bool minifyOutput = args.Length > 3 && args[3] == "-min";

            WriteOutput(appRoot, scriptsPath, outputFile, minifyOutput);
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: {0} ApplicationRoot ScriptPath Output", Assembly.GetExecutingAssembly().GetName().Name);
        }

        private static void SetUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        private static void WriteOutput(string appRoot, string scriptsPath, string outputFile, bool minifyOutput)
        {
            var stringBuilder = new StringBuilder();

            foreach (string file in new Resolver(appRoot, scriptsPath, "*.js").Resolve())
            {
                Console.WriteLine("Appending {0}", file);
                stringBuilder.AppendLine(
                    "// ~" + file.ToLower().Replace(scriptsPath.ToLower(), string.Empty).Replace('\\', '/'));
                stringBuilder.AppendLine(File.ReadAllText(file));
            }

            using (Stream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                var writer = new StreamWriter(output);
                writer.WriteLine(minifyOutput ? new JavaScriptCompressor().Compress(stringBuilder.ToString()) : stringBuilder.ToString());
                writer.Flush();
            }

            Console.WriteLine("Written to {0}.", outputFile);
        }
    }
}