using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace HonkAPI.Core
{
    public class HonkSaveAPI
    {
        private readonly string modId;
        private readonly string saveFilePath;
        private readonly Dictionary<string, string> cache = new Dictionary<string, string>();

        public HonkSaveAPI(string modId)
        {
            this.modId = modId;
            string localLow = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string parent = Path.GetDirectoryName(localLow);
            string baseFolder = Path.Combine(parent, "LocalLow");
            string modDataDir = Path.Combine(baseFolder, Path.Combine("House House", Path.Combine("Untitled Goose Game", "HonkModsData")));
            
            if (!Directory.Exists(modDataDir))
            {
                Directory.CreateDirectory(modDataDir);
            }
            
            saveFilePath = Path.Combine(modDataDir, modId + ".data");
            LoadFromDisk();
        }

        private void LoadFromDisk()
        {
            cache.Clear();
            if (!File.Exists(saveFilePath)) return;

            string[] lines = File.ReadAllLines(saveFilePath);
            foreach (string line in lines)
            {
                int sep = line.IndexOf('=');
                if (sep > 0)
                {
                    string k = line.Substring(0, sep).Trim();
                    string v = line.Substring(sep + 1).Trim();
                    cache[k] = v;
                }
            }
        }

        private void SaveToDisk()
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, string> kvp in cache)
            {
                lines.Add(kvp.Key + "=" + kvp.Value);
            }
            File.WriteAllLines(saveFilePath, lines.ToArray());
        }

        public void SetString(string key, string value)
        {
            cache[key] = value;
            SaveToDisk();
        }

        public string GetString(string key, string defaultValue)
        {
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }

        public int GetInt(string key, int defaultValue)
        {
            string val = GetString(key, null);
            int res;
            if (val != null && int.TryParse(val, out res))
            {
                return res;
            }
            return defaultValue;
        }

        public void SetFloat(string key, float value)
        {
            SetString(key, value.ToString());
        }

        public float GetFloat(string key, float defaultValue)
        {
            string val = GetString(key, null);
            float res;
            if (val != null && float.TryParse(val, out res))
            {
                return res;
            }
            return defaultValue;
        }

        public void SetBool(string key, bool value)
        {
            SetString(key, value ? "true" : "false");
        }

        public bool GetBool(string key, bool defaultValue)
        {
            string val = GetString(key, null);
            if (val != null)
            {
                return val.ToLower() == "true";
            }
            return defaultValue;
        }
    }
}
