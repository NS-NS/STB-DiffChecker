using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;

namespace DiffCheckerLib.WPF
{
    /// <summary>
    /// ユーザー設定（最後に開いたフォルダなど）の保存クラス
    /// %APPDATA%\STB-DiffChecker\settings.json に保存する
    /// 旧バージョンのレジストリ保存(HKCU\NS-NS\STB-DiffChecker)からは初回読込時に移行する
    /// </summary>
    public static class UserSettings
    {
        private const string LegacyRegistryKey = @"NS-NS\STB-DiffChecker";
        private const string LegacyRegistryValueName = "Path";

        private static string SettingsDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "STB-DiffChecker");

        private static string SettingsPath => Path.Combine(SettingsDirectory, "settings.json");

        private sealed class Settings
        {
            public string LastDirectory { get; set; }
        }

        /// <summary>
        /// 最後に使用したフォルダを取得する（未設定時はデスクトップ）
        /// </summary>
        public static string GetLastDirectory()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsPath));
                    if (!string.IsNullOrEmpty(settings?.LastDirectory) && Directory.Exists(settings.LastDirectory))
                    {
                        return settings.LastDirectory;
                    }
                }
                else
                {
                    // 旧レジストリ保存からの移行
                    string legacy = ReadLegacyRegistry();
                    if (!string.IsNullOrEmpty(legacy) && Directory.Exists(legacy))
                    {
                        return legacy;
                    }
                }
            }
            catch (Exception)
            {
                // 設定が読めない場合は既定値へフォールバック
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        /// <summary>
        /// 最後に使用したフォルダを保存する
        /// </summary>
        public static void SetLastDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                _ = Directory.CreateDirectory(SettingsDirectory);
                string json = JsonSerializer.Serialize(new Settings { LastDirectory = path }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception)
            {
                // 保存失敗は動作に影響しないため無視
            }
        }

        private static string ReadLegacyRegistry()
        {
            try
            {
                using RegistryKey rk = Registry.CurrentUser.OpenSubKey(LegacyRegistryKey, false);
                return rk?.GetValue(LegacyRegistryValueName) as string;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
