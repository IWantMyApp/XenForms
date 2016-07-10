namespace XenForms.Core.Toolbox.Shell
{
    public interface ISettingsStore
    {
        bool Set(string key, string value);
        bool Set(string key, bool value);

        string GetString(string key);
        bool? GetBool(string key);
    }
}