// Copyright (c) Adam Ralph. (adam@adamralph.com)
namespace DudeTests.Acceptance.Console.Support
{
    using System;
    using System.IO;
    using LiteGuard;
    using static System.FormattableString;

    public static class DudeConsole
    {
        private static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

        public static string Execute(string scriptFile, ScenarioDirectory directory)
        {
            Guard.AgainstNullArgument("directory", directory);

#if DEBUG
            var config = "Debug";
#else
            var config = "Release";
#endif

            var exe = Path.GetFullPath(
                Path.Combine("..", "..", "..", "..", "src", "Dude.Console", "bin", config, "dude.exe"));

            var result = ProcessExecutor.Execute(
                IsMono ? "mono" : exe,
                IsMono ? string.Concat(exe, " ", scriptFile) : scriptFile,
                directory.Name,
                30000);

            File.WriteAllText(Path.GetFileName(directory.Name) + "stdout.log", result.Item2);
            File.WriteAllText(Path.GetFileName(directory.Name) + "stderr.log", result.Item3);

            if (result.Item1 != 0)
            {
                var message = Invariant(
                    $"dude.exe exited with code {result.Item1}.{Environment.NewLine}stdout was: {result.Item2}{Environment.NewLine}stderr was: {result.Item3}");

                throw new InvalidOperationException(message);
            }

            return result.Item2;
        }
    }
}
