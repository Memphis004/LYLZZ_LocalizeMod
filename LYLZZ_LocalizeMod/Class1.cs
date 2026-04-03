using MelonLoader;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;


[assembly: MelonInfo(typeof(LYLZZ_LocalizeMod.Core), "LongYinLiZhiZhuanTH", "1.0.0", "????", null)]
[assembly: MelonGame(null, null)]


namespace LYLZZ_LocalizeMod;


public class Core : MelonMod
{
    public static string ModPath = null!;
    public static string GamePath = null!;

    public static Action<string> LogError { get; set; } = null!;
    public static Action<string> LogInfo { get; set; } = null!;

    public override void OnInitializeMelon()
    {

        LogError = (string log) => { LoggerInstance.Error(log); Debug.LogError(log); };
        LogInfo = (string log) => { LoggerInstance.Msg(log); Debug.Log(log); };

        ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        GamePath = new DirectoryInfo(Application.dataPath).Parent!.FullName;
        Core.LogInfo("Initialized. Hello World");

        try
        {
            var harmony = new HarmonyLib.Harmony("com.????.LongYinLiZhiZhuanMod");
            harmony.PatchAll(typeof(LocalizationPatches));
            Core.LogInfo($"Harmony patches applied: {harmony.GetPatchedMethods().Count()} methods");
        }
        catch (Exception ex)
        {
            LogError(ex.ToString());
        }
    }
}
