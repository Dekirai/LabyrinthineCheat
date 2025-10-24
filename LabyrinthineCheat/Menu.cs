using Il2Cpp;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Saves;
using MelonLoader;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LabyrinthineCheat
{
    public class Menu : MelonMod
    {
        private Rect windowRect = new Rect(50, 50, 300, 400);
        private Rect windowMonsterTeleportRect = new Rect(350, 50, 300, 400);
        private Rect windowPlayerTeleportRect = new Rect(650, 50, 300, 400);

        private string xCoords = "0";
        private string zCoords = "0";
        private string yCoords = "0";

        private Vector3 lastPlayerPosition = Vector3.zero;

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

            if (Laby.PlayerInCase && Laby.PlayerControl != null)
            {
                windowMonsterTeleportRect = GUILayout.Window(1, windowMonsterTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawMonsterTeleportMenu), "Monster Teleport Menu", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                windowPlayerTeleportRect = GUILayout.Window(2, windowPlayerTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawPlayerTeleportMenu), "Player Teleport Menu", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
        }

        private void DrawMenu(int windowID)
        {
            GUILayout.Label("~ Made by Dekirai & GravityMaster ~", titleStyle);
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

                float previousFlashlightValue = Laby.FlashlightMultiplier;

                Laby.FlashlightMultiplier = GUILayout.HorizontalSlider(Laby.FlashlightMultiplier, 1f, 500f, sliderStyle, GUI.skin.horizontalSliderThumb);

                if (Mathf.Abs(previousFlashlightValue - Laby.FlashlightMultiplier) > Mathf.Epsilon)
                {
                    Hacks.SetFlashlightPower(Laby.FlashlightMultiplier);
                }

                GUILayout.Label("Movement Speed: " + Laby.MovementSpeed.ToString("F2"), titleStyle);

                float previousMovementSpeedValue = Laby.MovementSpeed;

                Laby.MovementSpeed = GUILayout.HorizontalSlider(Laby.MovementSpeed, 4.4f, 50f, sliderStyle, GUI.skin.horizontalSliderThumb);

                if (Mathf.Abs(previousMovementSpeedValue - Laby.MovementSpeed) > Mathf.Epsilon)
                {
                    Hacks.SetMovementSpeed(Laby.MovementSpeed);
                }

                GUILayout.Label("Jump Height: " + Laby.JumpHeight.ToString("F2"), titleStyle);

                float previousJumpHeightValue = Laby.JumpHeight;

                Laby.JumpHeight = GUILayout.HorizontalSlider(Laby.JumpHeight, 1.8f, 50f, sliderStyle, GUI.skin.horizontalSliderThumb);

                if (Mathf.Abs(previousJumpHeightValue - Laby.JumpHeight) > Mathf.Epsilon)
                {
                    Hacks.SetJumpHeight(Laby.JumpHeight);
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
                    MelonCoroutines.Start(StartInvincibleAfterTeleport());
                    Laby.PlayerControl.playerNetworkSync.MoveToPosition(new Vector3(ai.transform.position.x, ai.transform.position.y + 2f, ai.transform.position.z));
                }
            }

            GUI.DragWindow();
        }

        private void DrawPlayerTeleportMenu(int windowID)
        {
            Vector3 playerPosition = Laby.PlayerControl.playerNetworkSync.transform.position;

            if (playerPosition != lastPlayerPosition)
            {
                xCoords = Mathf.RoundToInt(playerPosition.x).ToString();
                yCoords = Mathf.RoundToInt(playerPosition.y).ToString();
                zCoords = Mathf.RoundToInt(playerPosition.z).ToString();
                lastPlayerPosition = playerPosition;
            }

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
                    MelonCoroutines.Start(StartInvincibleAfterTeleport());
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
                if (GUILayout.Button(player.ClientData.Name, buttonStyle))
                {
                    MelonCoroutines.Start(StartInvincibleAfterTeleport());
                    Laby.PlayerControl.playerNetworkSync.MoveToPosition(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z));
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

        private IEnumerator StartInvincibleAfterTeleport()
        {
            MelonLogger.Msg($"You're invincible for {5f} seconds before monsters can hunt you");
            Laby.PlayerControl.playerNetworkSync.SafezoneType = SafezoneType.Lightzone;
            yield return new WaitForSeconds(5f);

            Laby.PlayerControl.playerNetworkSync.SafezoneType = SafezoneType.None;
        }
    }
}
