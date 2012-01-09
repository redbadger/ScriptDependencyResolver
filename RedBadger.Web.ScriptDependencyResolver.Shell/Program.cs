using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RedBadger.Web.ScriptDependencyResolver;
using System.Text;

namespace RedBadger.Web.ScriptDependencyResolver.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUnhandledExceptionHandler();
            if (args.Length != 3 && args.Length != 4)
            {
                PrintUsage();
                return;
            }

            var appRoot = args[0];
            var scriptsPath = args[1];
            var outputFile = args[2];

            WriteOutput(appRoot, scriptsPath, outputFile);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: {0} ApplicationRoot ScriptPath Output", Assembly.GetExecutingAssembly().GetName().Name);
        }

        private static void SetUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
            Environment.Exit(1);
        }

        private static void WriteOutput(string appRoot, string scriptsPath, string outputFile)
        {            
            using (var writer = new StreamWriter(outputFile))
            {
                foreach (string file in new Resolver(appRoot, scriptsPath, "*.js").Resolve())
                {
                    Console.WriteLine("Appending {0}", file);
                    writer.WriteLine("// " + file.ToLower().Replace(scriptsPath.ToLower(), ""));
                    writer.WriteLine(File.ReadAllText(file));
                }                
            }
            Console.WriteLine("Written to {0}.", outputFile);
        }
    }
}
