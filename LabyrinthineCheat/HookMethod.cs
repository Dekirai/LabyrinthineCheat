using HarmonyLib;
using Il2CppCharacterCustomization;
using MelonLoader;

namespace LabyrinthineCheat
{
    public class HookMethod
    {
        [HarmonyPatch(typeof(CustomizationSave), "UnlockItem")]
        public static class HookUnlockItem
        {
            [HarmonyPostfix]
            public static void Postfix(ushort itemID, bool unlockOnContractFinish)
            {
                MelonLogger.Msg($"UnlockItem Hook: Item {itemID} unlocked. OnContractFinish: {unlockOnContractFinish}");
            }
        }
    }
}
