namespace GitProfileManager.Services
{
    public interface IGitConfigService {
        bool SetValue(string key, string value, bool global = false);
        bool UnsetValue(string key, string value, bool global = false);
    }
}