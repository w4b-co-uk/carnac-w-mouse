using System.Threading.Tasks;

namespace Carnac.Logic.Settings {
    public interface ISettingsProvider {
        T GetSettings<T>() where T : new();
        void SaveSettings<T>(T settings);
        Task SaveSettingsAsync<T>(T settings);
        void ResetToDefaults<T>() where T : new();
        Task ResetToDefaultsAsync<T>() where T : new();
    }
}
