using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Saves;
using MelonLoader;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LabyrinthineCheat
{
    public class Menu
    {
        private Rect windowRect = new Rect(50, 50, 300, 400);
        private Rect windowMonsterTeleportRect = new Rect(350, 50, 300, 400);
        private Rect windowPlayerTeleportRect = new Rect(650, 50, 300, 400);

        private string xCoords = "0";
        private string zCoords = "0";
        private string yCoords = "0";

        private string currencyInput = "100";

        private string experienceInput = "1000";

        private bool initializeStampValues = true;
        private string rareStampInput = "0";
        private string hardcoreStampInput = "0";

        private GUIStyle? titleStyle;
        private GUIStyle? buttonStyle;
        private GUIStyle? sliderStyle;

        public void StartMenu()
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.cyan }
            };

            sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);

            windowRect = GUILayout.Window(0, windowRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawMenu), "Labyrinthine Menu", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            if (Laby.PlayerInCase)
            {
                windowMonsterTeleportRect = GUILayout.Window(1, windowMonsterTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawMonsterTeleportMenu), "Monster Teleport Menu", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                windowPlayerTeleportRect = GUILayout.Window(2, windowPlayerTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawPlayerTeleportMenu), "Player Teleport Menu", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
        }

        private void DrawMenu(int windowID)
        {
            GUILayout.Label(" ~ Made by Dekirai & GravityMaster ~", titleStyle);
            GUILayout.Space(10);

            if (Laby.CurrentSceneIndex == 2 || Laby.CurrentSceneIndex == 8)
            {
                if (GUILayout.Button("Unlock all cosmetics", buttonStyle))
                    Hacks.UnlockAllCosmetics();

                if (GUILayout.Button("Unlock all monster types", buttonStyle))
                    Hacks.UnlockAllMonsterTypes();

                if (GUILayout.Button("Unlock all maze types", buttonStyle))
                    Hacks.UnlockAllMazeTypes();

                if (GUILayout.Button("Have all items x1000", buttonStyle))
                    Hacks.SetAllItemsCount();

                GUILayout.BeginHorizontal();

                currencyInput = NumberInput(currencyInput);

                if (GUILayout.Button("Give Currency", buttonStyle))
                    Hacks.AddOrRemoveCurrency(int.Parse(currencyInput));

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                experienceInput = NumberInput(experienceInput);

                if (GUILayout.Button("Give Experience", buttonStyle))
                    Hacks.AddOrRemoveExperience(int.Parse(experienceInput));
                GUILayout.EndHorizontal();

                if(initializeStampValues)
                {
                    hardcoreStampInput = EquipmentSave.GetContractTypeTokenCount(ContractType.Hardcore).ToString();
                    rareStampInput = EquipmentSave.GetContractTypeTokenCount(ContractType.Rare).ToString();
                    initializeStampValues = false;
                }

                GUILayout.BeginHorizontal();

                rareStampInput = NumberInput(rareStampInput);

                if (GUILayout.Button("Set Rare stamps", buttonStyle))
                    Hacks.SetRareStamps(int.Parse(rareStampInput));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                hardcoreStampInput = NumberInput(hardcoreStampInput);

                if (GUILayout.Button("Set Hardcore stamps", buttonStyle))
                    Hacks.SetHardcoreStamps(int.Parse(hardcoreStampInput));
                GUILayout.EndHorizontal();
            }
            else if (Laby.PlayerInCase)
            {
                if (GUILayout.Button("Complete Case", buttonStyle))
                    Hacks.CompleteAllObjectivesInCase();

                if (GUILayout.Button("Self Revive", buttonStyle))
                    Hacks.SelfRevive();

                if (GUILayout.Button(Laby.isAIEnabled ? "Disable Monsters" : "Enable Monsters", buttonStyle))
                    Hacks.ToggleMonsters();

                if (GUILayout.Button("Toggle ESP", buttonStyle))
                    Hacks.ToggleESP();

                GUILayout.Space(5);

                GUILayout.Label("Flashlight power: " + Laby.FlashlightMultiplier.ToString("F2"), titleStyle);

                float previousValue = Laby.FlashlightMultiplier;

                Laby.FlashlightMultiplier = GUILayout.HorizontalSlider(Laby.FlashlightMultiplier, 1f, 500f, sliderStyle, GUI.skin.horizontalSliderThumb);

                if (Mathf.Abs(previousValue - Laby.FlashlightMultiplier) > Mathf.Epsilon)
                {
                    Hacks.SetFlashlightPower(Laby.FlashlightMultiplier);
                }
            }
            else
            {
                GUILayout.Label("Loading...");
            }

            GUI.DragWindow();
        }

        private void DrawMonsterTeleportMenu(int windowID)
        {
            GUILayout.Label("Monster list", titleStyle);

            foreach (var ai in Laby.AIControllers)
            {
                var normalizedMonsterName = ai.MonsterType.ToString().Replace("_", " ");

                if (GUILayout.Button(normalizedMonsterName, buttonStyle))
                {
                    var transform = ai.transform;
                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y + 2f,
                        transform.position.z
                    );

                    Laby.PlayerControl.playerNetworkSync.MoveToTransform(ai.transform);
                }
            }

            GUI.DragWindow();
        }

        private void DrawPlayerTeleportMenu(int windowID)
        {
            Vector3 playerPosition = Laby.PlayerControl.playerNetworkSync.transform.position;

            GUILayout.Label($"Current Coords | X: {Mathf.RoundToInt(playerPosition.x)}, Y: {Mathf.RoundToInt(playerPosition.y)}, Z: {Mathf.RoundToInt(playerPosition.z)}");
            GUILayout.BeginHorizontal();

            xCoords = NumberInput(xCoords);
            yCoords = NumberInput(yCoords);
            zCoords = NumberInput(zCoords);

            GUILayout.EndHorizontal();
            if (GUILayout.Button($"Teleport To Coords", buttonStyle))
            {
                try
                {
                    Laby.PlayerControl.playerNetworkSync
                        .MoveToPosition(new Vector3(int.Parse(xCoords), int.Parse(yCoords), int.Parse(zCoords)));
                }
                catch
                {
                    MelonLogger.Error("Couldn't teleport you to those coords");
                }
            }

            if (GUILayout.Button("Teleport to Spawnpoint", buttonStyle))
                Hacks.TeleportToSpawn();

            GUILayout.Space(10);

            GUILayout.Label("Player list", titleStyle);

            foreach (var player in Laby.GameManager.Players)
            {
                if (GUILayout.Button(player.playerName, buttonStyle))
                {
                    var transform = player.transform;
                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y,
                        transform.position.z
                    );

                    Laby.PlayerControl.playerNetworkSync.MoveToTransform(player.transform);
                }
            }

            GUILayout.Label("Safezone list", titleStyle);
            if (Laby.Safezones != null && Laby.Safezones.Count > 0)
            {
                if (GUILayout.Button($"Teleport to Random Safezone", buttonStyle))
                {
                    int randomIndex = UnityEngine.Random.Range(0, Laby.Safezones.Count);
                    Laby.PlayerControl.playerNetworkSync.MoveToPosition(Laby.Safezones[randomIndex]);
                }

                int index = 1;
                foreach (var item in Laby.Safezones)
                {
                    if (GUILayout.Button($"Safezone {index}", buttonStyle))
                    {
                        Laby.PlayerControl.playerNetworkSync.MoveToPosition(item);
                    }
                    index++;
                }
            }
            else
            {
                GUILayout.Label($"Searching for safehouses...");
            }

            GUI.DragWindow();
        }

        private string NumberInput(string input) => Regex.Replace(GUILayout.TextField(input, 25), @"(?!^-)[^0-9]", "");
    }
}
