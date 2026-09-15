
using Alta.Caves;
using Alta.Console;
using HarmonyLib;
using MelonLoader;
using System.Text;
using System.Threading.Tasks;

[assembly : MelonInfo(typeof(CaveSeedUtil.Main), "Cave Seed Utilities", "1.0.0", "Circl")]

namespace CaveSeedUtil
{
    public class Main : MelonMod
    {
        public static int SetSeed = -1;
        public override void OnInitializeMelon()
        {
            CommandService.collection.ProcessModule(typeof(CaveCommands), typeof(CaveCommands).Assembly);
        }
    }
    [Module("caveseed", "Set seed of cave (or get seed)")]
    public class CaveCommands
    {
        [Command("set", "Wipe caves and regenerate with a set seed")]
        public static async Task<string> WipeAndSetCommand(int seed)
        {
            if (!ServerWipeManager.IsWiping && seed >= 0)
            {
                Main.SetSeed = seed;
                await ServerWipeManager.WipeCaves();
                logBuilder.Clear();
                logBuilder.AppendLine($"Set cave seed to {seed}");
                return logBuilder.ToString();
            }
            else
            {
                logBuilder.Clear();
                logBuilder.AppendLine($"Entered seed was invalid or server is busy");
                return logBuilder.ToString();
            }
        }
        [Command("get", "Get the seed of the current cave")]
        public static string GetCommand()
        {
            logBuilder.Clear();
            logBuilder.AppendLine($"{CaveLayerManager.Instance.seed}");
            return logBuilder.ToString();
        }

        private static StringBuilder logBuilder = new StringBuilder();
    }
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPatch(typeof(CaveLayerManager), nameof(CaveLayerManager.Prepare)), HarmonyPrefix]
        public static void setCaveSeed(CaveLayerManager __instance)
        {
            if (Main.SetSeed >= 0)
            {
                __instance.seed = Main.SetSeed;
                Main.SetSeed = -1;
            }
        }
    }
}
