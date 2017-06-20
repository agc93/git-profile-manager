using System.Collections.Generic;

namespace GitProfileManager.Services
{
    public interface IGitProfileStore
    {
        IEnumerable<string> GetProfiles();
        Dictionary<string, string> ReadProfile(string profileName);
        bool WriteProfile(string profileName, Dictionary<string, string> configurations);
        bool DeleteProfile(string profileName);
    }
}