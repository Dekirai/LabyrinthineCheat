using System;
using System.Linq;
using UnityEngine;
using Il2CppCharacterCustomization;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppValkoGames.Labyrinthine.Saves;
using Il2CppRandomGeneration.Contracts;
using MelonLoader;
using Il2Cpp;
using Il2CppValkoGames.Labyrinthine.Cases.Objectives;
using Il2CppObjectives;

namespace LabyrinthineCheat
{
    public static class Hacks
    {
        public static void UnlockAllCosmetics()
        {
            CustomizationSave save = CustomizationSave.Load();
            for (ushort i = 0; i < 1500; i++)
            {
                save.UnlockItem(i, false);
            }
            MelonLogger.Msg("All cosmetics unlocked.");
        }

        public static void UnlockAllMonsterTypes()
        {
            foreach (MonsterType monsterType in Enum.GetValues(typeof(MonsterType)))
            {
                EquipmentSave.UnlockMonsterType(monsterType);
                EquipmentSave.Save();
            }
            MelonLogger.Msg("All monster types unlocked.");
        }

        public static void UnlockAllMazeTypes()
        {
            foreach (MazeType mazeType in Enum.GetValues(typeof(MazeType)))
            {
                EquipmentSave.UnlockMazeType(mazeType);
                EquipmentSave.Save();
            }
            MelonLogger.Msg("All maze types unlocked.");
        }

        public static void CompleteAllObjectivesInCase()
        {
            if (Laby.CurrentSceneName.Contains("Zone"))
                return;

            var objectives = GameObject.FindObjectOfType<ObjectiveManager>();

            if (objectives != null)
            {
                try
                {
                    PickupCosmeticInCase();
                    foreach (var item in objectives.Objectives)
                    {
                        MelonLogger.Msg($"Found {item.Key} and {item.Value}");
                        objectives.SetObjectiveProgressL(item.Key, 100);
                        objectives.SetObjectiveProgressSync(item.Key, 100);
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Failed to set progress for an objective: {ex}");
                }
            }
            else
            {
                MelonLogger.Msg("Found No Objectives");
            }
        }

        public static void PickupCosmeticInCase()
        {
            GameObject.FindObjectOfType<CustomizationPickup>()?.Pickup();
        }

        public static void SetAllItemsCount()
        {
            for (short i = 0; i < 51; i++)
            {
                EquipmentSave.SetItemCount(i, 1000);
                EquipmentSave.Save();
            }
            MelonLogger.Msg("Successfully set all items to x1000.");
        }

        public static void ToggleMonsters()
        {
            Laby.isAIEnabled = !Laby.isAIEnabled;

            if (Laby.isAIEnabled)
            {
                MelonLogger.Msg("Monsters have been enabled.");
                foreach (MonsterType monsterType in Enum.GetValues(typeof(MonsterType)))
                {
                    AIManager.StartAI(monsterType);
                }
            }
            else
            {
                MelonLogger.Msg("Monsters have been disabled.");
                foreach (MonsterType monsterType in Enum.GetValues(typeof(MonsterType)))
                {
                    AIManager.StopAI(monsterType);
                }
            }
        }

        public static void TeleportToSpawn()
        {
            MelonLogger.Msg("Moved the player to Spawnpoint");
            Laby.PlayerControl.playerNetworkSync.MoveToSpawnpoint();
        }

        public static List<Vector3> GetAllSafezones()
        {
            List<Vector3> safezones = new List<Vector3>();

            MelonLogger.Msg("Finding safezones...");
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if (obj.name.ToLower().Contains("lightzone"))
                {
                    MelonLogger.Warning($"Found Safezone: {obj.name}");
                    MelonLogger.Msg($" - Position: {obj.transform.position}");

                    Collider collider = obj.GetComponent<Collider>();
                    if (collider != null)
                    {
                        Vector3 center = collider.bounds.center;
                        MelonLogger.Msg($" - Collider Center: {center}");

                        safezones.Add(new Vector3(center.x, center.y + 4f, center.z));
                    }
                    else
                    {
                        MelonLogger.Warning("No collider found for this object! Can't teleport");
                    }

                }
            }

            return safezones;
        }

        public static void ToggleESP()
        {
            Laby.ESPEnabled = !Laby.ESPEnabled;
            MelonLogger.Msg("ESP toggled: " + (Laby.ESPEnabled ? "enabled" : "disabled"));
        }

        public static void SetFlashlightPower(float multiplier)
        {
            if (Laby.PlayerControl?.playerNetworkSync?.flashlight?.flashlightLight == null)
                return;

            Laby.PlayerControl.playerNetworkSync.flashlight.flashlightLight.intensity = Laby.DefaultFlashlightIntensity * multiplier;

            var spotLight = Laby.PlayerControl.playerNetworkSync.AllLights
                .Where(l => l.Item1.name == "Point Light")
                .Select(l => l.Item1)
                .FirstOrDefault();

            if (spotLight != null)
            {
                spotLight.intensity = Laby.DefaultPointFlashlightIntensity * multiplier;
            }
            MelonLogger.Msg("Flashlight power set to: " + multiplier);
        }
    }
}
