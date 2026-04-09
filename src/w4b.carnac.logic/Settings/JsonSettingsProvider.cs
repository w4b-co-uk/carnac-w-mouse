using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Carnac.Logic.Settings {
    public class JsonSettingsProvider : ISettingsProvider {
        private static readonly JsonSerializerOptions JsonOptions = new() {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly string settingsDirectory;
        private readonly ConcurrentDictionary<Type, object> cache = new();

        public JsonSettingsProvider(string settingsDirectory) {
            this.settingsDirectory = settingsDirectory;
            Directory.CreateDirectory(settingsDirectory);
        }

        public T GetSettings<T>() where T : new() {
            return (T)cache.GetOrAdd(typeof(T), _ => LoadOrCreate<T>());
        }

        public void SaveSettings<T>(T settings) {
            string path = GetSettingsPath<T>();
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);
            cache[typeof(T)] = settings;
        }

        public async Task SaveSettingsAsync<T>(T settings) {
            string path = GetSettingsPath<T>();
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
            cache[typeof(T)] = settings;
        }

        public void ResetToDefaults<T>() where T : new() {
            string path = GetSettingsPath<T>();
            if (File.Exists(path)) File.Delete(path);
            T defaults = CreateWithDefaults<T>();
            cache[typeof(T)] = defaults;
        }

        public async Task ResetToDefaultsAsync<T>() where T : new() {
            string path = GetSettingsPath<T>();
            if (File.Exists(path)) File.Delete(path);
            T defaults = CreateWithDefaults<T>();
            string json = JsonSerializer.Serialize(defaults, JsonOptions);
            await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
            cache[typeof(T)] = defaults;
        }

        private T LoadOrCreate<T>() where T : new() {
            string path = GetSettingsPath<T>();
            if (File.Exists(path)) {
                try {
                    string json = File.ReadAllText(path);
                    T loaded = JsonSerializer.Deserialize<T>(json, JsonOptions);
                    if (loaded != null) {
                        ApplyMissingDefaults(loaded);
                        return loaded;
                    }
                } catch (JsonException) {
                    // Corrupted file — fall through to defaults
                }
            }

            T settings = CreateWithDefaults<T>();
            SaveSettings(settings);
            return settings;
        }

        private static T CreateWithDefaults<T>() where T : new() {
            T instance = new();
            ApplyMissingDefaults(instance);
            return instance;
        }

        private static void ApplyMissingDefaults<T>(T instance) {
            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (!prop.CanWrite) continue;
                DefaultValueAttribute attr = prop.GetCustomAttribute<DefaultValueAttribute>();
                if (attr == null) continue;

                object current = prop.GetValue(instance);
                object defaultForType = prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null;

                // Apply [DefaultValue] only if the property still holds its CLR default (0, false, null, etc.)
                if (Equals(current, defaultForType) && attr.Value != null) {
                    object converted = Convert.ChangeType(attr.Value, prop.PropertyType);
                    prop.SetValue(instance, converted);
                }
            }
        }

        private string GetSettingsPath<T>() {
            return Path.Combine(settingsDirectory, $"{typeof(T).Name}.json");
        }
    }
}
