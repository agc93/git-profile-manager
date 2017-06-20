using System;

namespace GitProfileManager.Services
{
    public class GitCommandService : IGitConfigService
    {
        private GitCommandRunner _runner;

        public GitCommandService(GitCommandRunner runner) {
            _runner = runner;
        }

        public bool SetValue(string key, string value, bool global = false)
        {
            if (!value.StartsWith("\"")) {
                value = $"\"{value}\"";
            }
            var output = _runner.RunCommand($"config {(global ? "--global" : string.Empty)} {key} {value}");
            return output.Item1 == 0 && output.Item2 == string.Empty ;
        }

        public bool UnsetValue(string key, string value, bool global = false)
        {
            if (!value.StartsWith("\"")) {
                value = $"\"{value}\"";
            }
            var output = _runner.RunCommand($"config {(global ? "--global" : string.Empty)} --unset {key} {value}");
            return output.Item1 == 0 && output.Item2 == string.Empty ;
        }
    }
}