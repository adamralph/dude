// Copyright (c) Adam Ralph. (adam@adamralph.com)
namespace Dude.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
    using Microsoft.CodeAnalysis.Scripting.Hosting;

    internal static class Program
    {
        private static readonly string DefaultResponseFilename = "dude.rsp";

        private static readonly string DefaultResponseFileContents =
@"/r:System
/r:System.Core
/r:Microsoft.CSharp
/u:System
/u:System.IO
/u:System.Collections.Generic
/u:System.Console
/u:System.Diagnostics
/u:System.Dynamic
/u:System.Linq
/u:System.Linq.Expressions
/u:System.Text
/u:System.Threading.Tasks
";

        internal static int Main(string[] args)
        {
            try
            {
                // create default response file if not present and not specified
                var responseFile = Path.Combine(AppContext.BaseDirectory, DefaultResponseFilename);
                if (!args.Any(arg => arg.StartsWith("@", StringComparison.Ordinal)) && !File.Exists(responseFile))
                {
                    File.WriteAllText(responseFile, DefaultResponseFileContents);
                }

                // get assembly refs
                var codeAnalysis = typeof(Accessibility).Assembly;
                var codeAnalysisScripting = typeof(CommandLineScriptGlobals).Assembly;
                var codeAnalysisCSharpScripting = typeof(CSharpObjectFormatter).Assembly;

                // create compiler
                var compilerType = codeAnalysisCSharpScripting
                    .GetType("Microsoft.CodeAnalysis.CSharp.Scripting.Hosting.CSharpInteractiveCompiler", true);

                var baseDirectory = Directory.GetCurrentDirectory();

                var sdkDirectoryOpt = codeAnalysis.GetType("Roslyn.Utilities.CorLightup", true)
                    .GetNestedType("Desktop", BindingFlags.NonPublic)
                    .GetMethod("TryGetRuntimeDirectory", BindingFlags.NonPublic | BindingFlags.Static)
                    .Invoke(null, null);

                var analyzerLoader = Activator.CreateInstance(codeAnalysisScripting
                        .GetType("Microsoft.CodeAnalysis.Scripting.Hosting.NotImplementedAnalyzerLoader", true));

                var compiler = Activator.CreateInstance(
                    compilerType,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { responseFile, baseDirectory, sdkDirectoryOpt, args, analyzerLoader },
                    null);

                // create command line runner
                var commandLineRunnerType = codeAnalysisScripting
                    .GetType("Microsoft.CodeAnalysis.Scripting.Hosting.CommandLineRunner", true);

                var console = codeAnalysisScripting.GetType("Microsoft.CodeAnalysis.Scripting.Hosting.ConsoleIO", true)
                    .GetField("Default").GetValue(null);

                var scriptCompiler = codeAnalysisCSharpScripting
                    .GetType("Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScriptCompiler", true)
                    .GetField("Instance").GetValue(null);

                var objectFormatter = CSharpObjectFormatter.Instance;

                var commandLineRunner = Activator.CreateInstance(
                    commandLineRunnerType,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { console, compiler, scriptCompiler, objectFormatter },
                    null);

                // return value from command line runner -> run interactive
                return (int)commandLineRunnerType
                    .GetMethod("RunInteractive", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(commandLineRunner, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}
