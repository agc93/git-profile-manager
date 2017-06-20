using System;
using System.Diagnostics;
using System.IO;

namespace GitProfileManager.Services
{
    public class GitCommandRunner : IDisposable
    {
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _gitProcess.Dispose();
            }
        }

        public GitCommandRunner()
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
                FileName = "git",
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            _gitProcess = new Process();
            _gitProcess.StartInfo = processInfo;
        }

        internal bool IsGitRepository
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("log -1").Item2);
            }
        }

        internal (int, string) RunCommand(string args)
        {
            //args = args.StartsWith("git ") ? args : $"git {args}";
            _gitProcess.StartInfo.Arguments = args;
            _gitProcess.Start();
            string output = _gitProcess.StandardOutput.ReadToEnd().Trim();
            //output = output + Environment.NewLine + _gitProcess.StandardError.ReadToEnd().Trim();
            _gitProcess.WaitForExit();
            return (_gitProcess.ExitCode, output);
        }

        private bool _disposed;
        private readonly Process _gitProcess;
        
    }
}