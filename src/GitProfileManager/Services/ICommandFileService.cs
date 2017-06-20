using System.Collections.Generic;
using System.IO;

namespace GitProfileManager.Services
{
    public interface ICommandFileService {
        Dictionary<string, string> ReadFromFile(FileInfo filePath);
        bool WriteToFile(Dictionary<string, string> configurations, FileInfo path, bool includeCommand = false);
    }
}