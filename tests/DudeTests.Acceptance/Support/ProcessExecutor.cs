// Copyright (c) Adam Ralph. (adam@adamralph.com)
namespace DudeTests.Acceptance.Console.Support
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using static System.FormattableString;

    public static class ProcessExecutor
    {
        public static Tuple<int, string, string> Execute(
            string fileName, string arguments, string workingDirectory, int timeout)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                process.Start();
                var processId = process.Id;

                using (var waitForExit = Task.Factory.StartNew(() => process.WaitForExit(timeout)))
                using (var readStandardOutput = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd()))
                using (var readStandardError = Task.Factory.StartNew(() => process.StandardError.ReadToEnd()))
                {
                    if (!waitForExit.Result)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (!process.HasExited)
                            {
                                throw new InvalidOperationException(Invariant($"Failed to kill overdue process {processId}."), ex);
                            }
                        }

                        throw new TimeoutException(Invariant($"The process {processId} took too long to complete and was killed."));
                    }

                    return Tuple.Create(process.ExitCode, readStandardOutput.Result, readStandardError.Result);
                }
            }
        }
    }
}
