using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace GitProfileManager.Services
{
    public class FileProfileStore : IGitProfileStore
    {
        private const string _fileName = ".gitprofiles";
        public Dictionary<string, string> ReadProfile(string profileName)
        {
            var file = GetProfileFile();
            var d = GetProfiles(file);
            return d.ContainsKey(profileName) ? d[profileName] : null;            
        }

        public bool WriteProfile(string profileName, Dictionary<string, string> configurations)
        {
            var file = GetProfileFile();
            var d = GetProfiles(file);
            d[profileName] = configurations;
            SaveProfiles(file, d);
            return file.Length > 0;
        }

        public bool DeleteProfile(string profileName)
        {
            var file = GetProfileFile();
            var d = GetProfiles(file);
            if (d.ContainsKey(profileName)) {
                d.Remove(profileName);
            }
            SaveProfiles(file, d);
            return true;
        }

        public IEnumerable<string> GetProfiles()
        {
            var file = GetProfileFile();
            var d = GetProfiles(file);
            return d.Keys;
        }

        private static void SaveProfiles(FileInfo file, Dictionary<string, Dictionary<string, string>> d)
        {
            var ser = new SerializerBuilder().Build();
            var yaml = ser.Serialize(d);
            File.WriteAllText(file.FullName, yaml);
            file.Refresh();
        }

        private static Dictionary<string, Dictionary<string, string>> GetProfiles(FileInfo file)
        {
            var deser = new DeserializerBuilder().Build();
            var content = File.ReadAllText(file.FullName);
            var d = deser.Deserialize<Dictionary<string, Dictionary<string, string>>>(content);
            return d ?? new Dictionary<string, Dictionary<string, string>>();
        }

        private FileInfo GetProfileFile() {
            var home = System.Environment.GetEnvironmentVariable("HOME");
            var homeDir = new DirectoryInfo(home);
            if (!homeDir.Exists) throw new DirectoryNotFoundException($"Could not locate home directory (tried {homeDir.FullName})");
            var file = new FileInfo(Path.Combine(homeDir.FullName, _fileName));
            if (!file.Exists) {
                using (var s = file.Create()) {
                    s.Flush();
                    s.Dispose();
                }
            }
            file.Refresh();
            return file;
        }
    }
}