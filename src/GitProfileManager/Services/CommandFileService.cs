using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pair = System.Collections.Generic.KeyValuePair<string, string>;

namespace GitProfileManager.Services
{
    public class CommandFileService : ICommandFileService
    {
        public Dictionary<string, string> ReadFromFile(FileInfo filePath)
        {
            filePath.Refresh();
            if (!filePath.Exists) throw new FileNotFoundException("Could not find command file", filePath.FullName);
            var lines = File.ReadAllLines(filePath.FullName);
            var confs = lines
                .ToConfig()
                .ToDictionary(
                    k => k.Split(' ')[0],
                    v => string.Join(" ", v.Split(' ').Skip(1)).Trim());
            return confs;
        }

        public bool WriteToFile(Dictionary<string, string> configurations, FileInfo path, bool includeCommand = false)
        {
            string RenderConfig(Pair config)
            {
                return string.Format(
                    "{0} {1}",
                    config.Key,
                    (
                        (config.Value.StartsWith("\"") || config.Value.StartsWith("'"))
                        && (config.Value.EndsWith("\"") || config.Value.EndsWith("'"))
                    )
                        ? config.Value
                        : $"\"{config.Value}\""
                );
            }
            var text = configurations.Select(RenderConfig);
            // var text = configurations.Select(p => $"{p.Key} {((p.Value.StartsWith("\"") && p.Value.EndsWith("\"")) ? p.Value : "\"" + p.Value + "\"")}");
            if (includeCommand)
            {
                text = text.Select(t => $"git config {t}");
            }
            File.WriteAllLines(path.FullName, text);
            path.Refresh();
            return path.Exists;
        }
    }
}