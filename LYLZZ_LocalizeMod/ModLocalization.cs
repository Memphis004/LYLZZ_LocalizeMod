using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace LYLZZ_LocalizeMod
{

    public static class ModLocalization
    {

        private static System.Collections.Generic.Dictionary<string, string> _translations;

        public static System.Collections.Generic.Dictionary<string, string> Translations
        {
            get
            {
                if (_translations == null)
                {
                    _translations = new System.Collections.Generic.Dictionary<string, string>();
                    LoadTranslations();
                }
                return _translations;
            }
        }

        private static void LoadTranslations()
        {
            string csvPath = Path.Combine(Core.ModPath, "thai_translation.csv");
            
            if (File.Exists(csvPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(csvPath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        int commaIndex = line.IndexOf(',');
                        if (commaIndex > 0)
                        {
                            string key = line.Substring(0, commaIndex).Trim();
                            string value = line.Substring(commaIndex + 1).Trim();
                            
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            {
                                Translations[key] = value;
                            }
                        }
                    }
                    Core.LogInfo($"Loaded {Translations.Count} Thai translations");
                }
                catch (Exception ex)
                {
                    Core.LogError($"Failed to load translations: {ex.Message}");
                }
            }

        }


        public static string Translate(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;

            if (Translations.TryGetValue(key, out string thaiText))
            {
                return thaiText;
            }


            return key;
        }
    }

    public static class LocalizationPatches
    {

        [HarmonyPatch(typeof(LTLocalization), "GetText", new Type[] { typeof(string), typeof(bool) })]
        [HarmonyPostfix]
        private static void LTLocalization_GetText_Postfix(string key, bool justReplace, ref string __result)
        {
            __result = ModLocalization.Translate(__result);
        }


        [HarmonyPatch(typeof(Localization), "Get", new Type[] { typeof(string), typeof(bool) })]
        [HarmonyPostfix]
        private static void Localization_Get_Postfix(string key, bool warnIfMissing, ref string __result)
        {
            __result = ModLocalization.Translate(__result);
        }

        [HarmonyPatch(typeof(Localization), "Localize", new Type[] { typeof(string) })]
        [HarmonyPostfix]
        private static void Localization_Localize_Postfix(string key, ref string __result)
        {
            __result = ModLocalization.Translate(__result);
        }

        [HarmonyPatch(typeof(Localization), "Set", new Type[] { typeof(string), typeof(string), typeof(string) })]
        [HarmonyPrefix]
        private static bool Localization_Set_Prefix(ref string language, string key, string text)
        {

            return true; 
        }
    }
}
