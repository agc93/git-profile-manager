using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GitProfileManager.Services
{
    public class RepositoryInformation : IDisposable
    {
        public string CommitHash
        {
            get
            {
                return RunCommand("rev-parse HEAD");
            }
        }

        public string BranchName
        {
            get
            {
                return RunCommand("rev-parse --abbrev-ref HEAD");
            }
        }

        public string TrackedBranchName
        {
            get
            {
                return RunCommand("rev-parse --abbrev-ref --symbolic-full-name @{u}");
            }
        }

        public bool HasUnpushedCommits
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("log @{u}..HEAD"));
            }
        }

        public bool HasUncommittedChanges
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("status --porcelain"));
            }
        }

        public IEnumerable<string> Log
        {
            get
            {
                int skip = 0;
                while (true)
                {
                    string entry = RunCommand(String.Format("log --skip={0} -n1", skip++));
                    if (String.IsNullOrWhiteSpace(entry))
                    {
                        yield break;
                    }

                    yield return entry;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _gitProcess.Dispose();
            }
        }

        internal RepositoryInformation(string path, string gitPath)
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = Directory.Exists(gitPath) ? gitPath : "git.exe",
                CreateNoWindow = true,
                WorkingDirectory = (path != null && Directory.Exists(path)) ? path : Directory.GetCurrentDirectory()
            };

            _gitProcess = new Process();
            _gitProcess.StartInfo = processInfo;
        }

        internal bool IsGitRepository
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("log -1"));
            }
        }

        private string RunCommand(string args)
        {
            _gitProcess.StartInfo.Arguments = args;
            _gitProcess.Start();
            string output = _gitProcess.StandardOutput.ReadToEnd().Trim();
            _gitProcess.WaitForExit();
            return output;
        }

        private bool _disposed;
        private readonly Process _gitProcess;
    }
}