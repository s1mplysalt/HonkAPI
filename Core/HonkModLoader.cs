using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using BepInEx.Logging;

namespace HonkAPI.Core
{
    public class HonkModInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string FilePath { get; set; }
        public bool IsLoaded { get; set; }
    }

    public static class HonkModLoader
    {
        private static readonly List<IHonkMod> loadedMods = new List<IHonkMod>();
        private static readonly List<HonkModInfo> modInfos = new List<HonkModInfo>();
        private static ManualLogSource logger;

        public static IReadOnlyList<IHonkMod> LoadedMods
        {
            get { return loadedMods.AsReadOnly(); }
        }

        public static IReadOnlyList<HonkModInfo> ModInfos
        {
            get { return modInfos.AsReadOnly(); }
        }

        public static void Initialize(ManualLogSource log)
        {
            logger = log;
            loadedMods.Clear();
            modInfos.Clear();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string pluginsPath = Path.Combine(baseDir, Path.Combine("BepInEx", "plugins"));
            string modsPath = Path.Combine(baseDir, "mods");

            if (!Directory.Exists(modsPath))
            {
                try
                {
                    Directory.CreateDirectory(modsPath);
                }
                catch (Exception)
                {
                }
            }

            List<string> searchPaths = new List<string>();
            if (Directory.Exists(pluginsPath))
            {
                searchPaths.Add(pluginsPath);
            }
            if (Directory.Exists(modsPath))
            {
                searchPaths.Add(modsPath);
            }

            HashSet<string> loadedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string searchPath in searchPaths)
            {
                string[] dlls = Directory.GetFiles(searchPath, "*.dll", SearchOption.AllDirectories);
                foreach (string dll in dlls)
                {
                    string fileName = Path.GetFileName(dll);
                    if (fileName.Equals("HonkAPI.dll", StringComparison.OrdinalIgnoreCase)) continue;
                    if (loadedFiles.Contains(fileName)) continue;

                    loadedFiles.Add(fileName);
                    LoadModFromAssembly(dll);
                }
            }

            logger.LogInfo(string.Format("HonkModLoader: {0} mod(s) successfully loaded from plugins and mods directories.", loadedMods.Count));
        }

        private static void LoadModFromAssembly(string assemblyPath)
        {
            try
            {
                byte[] raw = File.ReadAllBytes(assemblyPath);
                Assembly asm = Assembly.Load(raw);
                Type modInterface = typeof(IHonkMod);

                bool foundMod = false;
                foreach (Type t in asm.GetTypes())
                {
                    if (modInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    {
                        try
                        {
                            IHonkMod mod = (IHonkMod)Activator.CreateInstance(t);
                            HonkContext ctx = new HonkContext(mod.ModId, logger);
                            mod.OnInitialize(ctx);
                            loadedMods.Add(mod);
                            modInfos.Add(new HonkModInfo
                            {
                                Id = mod.ModId,
                                Name = mod.ModName,
                                Version = mod.Version,
                                FilePath = assemblyPath,
                                IsLoaded = true
                            });
                            foundMod = true;
                            logger.LogInfo(string.Format("Mod activated: [{0}] {1} v{2}", mod.ModId, mod.ModName, mod.Version));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(string.Format("Failed to initialize mod class {0}: {1}", t.FullName, ex.Message));
                        }
                    }
                }

                if (!foundMod)
                {
                    modInfos.Add(new HonkModInfo
                    {
                        Id = Path.GetFileNameWithoutExtension(assemblyPath),
                        Name = Path.GetFileNameWithoutExtension(assemblyPath),
                        Version = asm.GetName().Version.ToString(),
                        FilePath = assemblyPath,
                        IsLoaded = true
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error loading assembly {0}: {1}", Path.GetFileName(assemblyPath), ex.Message));
            }
        }
    }
}
