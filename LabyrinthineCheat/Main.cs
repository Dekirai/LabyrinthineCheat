using Il2CppRandomGeneration.Contracts;
using MelonLoader;
using UnityEngine;
using Il2CppCharacterCustomization;
using HarmonyLib;
using Il2CppValkoGames.Labyrinthine.Saves;
using Il2CppValkoGames.Labyrinthine.Store;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2Cpp;
using System.Collections;
using System.Linq;
using System.Threading;

namespace LabyrinthineCheat
{
    public class Laby : MelonMod
    {
        private bool showMenu = false;
        private Rect windowRect = new Rect(50, 50, 300, 400);
        private Rect windowMonsterTeleportRect = new Rect(350, 50, 300, 400);
        private Rect windowPlayerTeleportRect = new Rect(650, 50, 300, 400);
        public static bool CanRunCoRoutine = true;
        public static bool CoRoutineIsRunning = true;
        private static object coRoutine;

        public static bool ESPEnabled = false;
        public static bool isAIEnabled = true;

        public static float FlashlightMultiplier = 1f;
        public static float DefaultFlashlightIntensity = 47.7f;
        public static float DefaultPointFlashlightIntensity = 30f;

        public static int? CurrentSceneIndex;
        public static string? CurrentSceneName;

        public static Camera GameCamera { get; set; }
        public static GameManager GameManager { get; set; }
        public static PlayerControl PlayerControl { get; set; }
        public static AIController[] AIControllers { get; set; }
        public static KeyPuzzle KeyPuzzle { get; set; }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

            CurrentSceneIndex = buildIndex;
            CurrentSceneName = sceneName;

            if (buildIndex >= 4)
            {
                CanRunCoRoutine = false;
                new Thread(() =>
                {
                    while (CoRoutineIsRunning)
                    {
                        coRoutine = MelonCoroutines.Start(CollectGameObjects());
                        Thread.Sleep(5000);
                    }
                }).Start();
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                showMenu = !showMenu;
            }
        }

        public override void OnGUI()
        {
            if (CurrentSceneIndex == null || CurrentSceneIndex < 2)
                return;

            if (showMenu)
            {
                GUI.Window(0, windowRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawMenu), "Labyrinthine Menu");
                if (CurrentSceneIndex != 2 && CurrentSceneIndex != 8)
                {
                    GUI.Window(1, windowMonsterTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawMonsterTeleportMenu), "Monster Teleport Menu");
                    GUI.Window(2, windowPlayerTeleportRect, Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<GUI.WindowFunction>(DrawPlayerTeleportMenu), "Player Teleport Menu");
                }
            }

            if (ESPEnabled)
            {
                ESP.Render();
            }
        }

        private void DrawMenu(int windowID)
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.cyan }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);

            GUILayout.Label("                ~ Made by Dekirai ~", titleStyle);
            GUILayout.Space(10);

            if (CurrentSceneIndex == 2 || CurrentSceneIndex == 8)
            {
                if (GUILayout.Button("Unlock all cosmetics", buttonStyle))
                    Hacks.UnlockAllCosmetics();

                if (GUILayout.Button("Unlock all monster types", buttonStyle))
                    Hacks.UnlockAllMonsterTypes();

                if (GUILayout.Button("Unlock all maze types", buttonStyle))
                    Hacks.UnlockAllMazeTypes();

                if (GUILayout.Button("Have all items x1000", buttonStyle))
                    Hacks.SetAllItemsCount();
            }
            else
            {
                if (GUILayout.Button("Teleport to Spawnpoint", buttonStyle))
                    Hacks.TeleportToSpawn();

                if (GUILayout.Button("Complete Case", buttonStyle))
                    Hacks.CompleteAllObjectivesInCase();

                if (GUILayout.Button(isAIEnabled ? "Disable Monsters" : "Enable Monsters", buttonStyle))
                    Hacks.ToggleMonsters();

                if (GUILayout.Button("Toggle ESP", buttonStyle))
                    Hacks.ToggleESP();

                GUILayout.Space(5);
                GUILayout.Label("Flashlight power: " + FlashlightMultiplier.ToString("F2"), titleStyle);
                FlashlightMultiplier = GUILayout.HorizontalSlider(FlashlightMultiplier, 1f, 500f, sliderStyle, GUI.skin.horizontalSliderThumb);
                if (GUILayout.Button("Set Flashlight power", buttonStyle))
                    Hacks.SetFlashlightPower(FlashlightMultiplier);
            }

            GUI.DragWindow();
        }

        private void DrawMonsterTeleportMenu(int windowID)
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.cyan }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUILayout.Space(10);

            GUILayout.Label("Monster list", titleStyle);

            foreach (var ai in AIControllers)
            {
                var normalizedMonsterName = ai.MonsterType.ToString().Replace("_", " ");

                if (GUILayout.Button(normalizedMonsterName, buttonStyle))
                {
                    var transform = ai.transform;
                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y + 4f, // to fix some stuck/fall into the ground
                        transform.position.z
                    );

                    PlayerControl.playerNetworkSync.MoveToTransform(ai.transform);
                }
            }

            GUI.DragWindow();
        }

        private void DrawPlayerTeleportMenu(int windowID)
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.cyan }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUILayout.Space(10);

            GUILayout.Label("Player list", titleStyle);

            foreach (var player in GameManager.Players)
            {

                if (GUILayout.Button(player.playerName, buttonStyle))
                {
                    var transform = player.transform;
                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y + 4f, // to fix some stuck/fall into the ground
                        transform.position.z
                    );

                    PlayerControl.playerNetworkSync.MoveToTransform(player.transform);
                }
            }

            GUI.DragWindow();
        }

        [HarmonyPatch(typeof(CustomizationSave), "UnlockItem")]
        public static class HookUnlockItem
        {
            [HarmonyPostfix]
            public static void Postfix(ushort itemID, bool unlockOnContractFinish)
            {
                MelonLogger.Msg($"UnlockItem Hook: Item {itemID} unlocked. OnContractFinish: {unlockOnContractFinish}");
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            ESPEnabled = false;
            isAIEnabled = true;
        }

        private IEnumerator CollectGameObjects()
        {
            GameManager = GameObject.FindObjectOfType<GameManager>();
            yield return new WaitForSeconds(0.15f);

            PlayerControl = GameObject.FindObjectOfType<PlayerControl>();
            yield return new WaitForSeconds(0.15f);

            AIControllers = GameObject.FindObjectsOfType<AIController>().ToArray();
            yield return new WaitForSeconds(0.15f);

            KeyPuzzle = GameObject.FindObjectOfType<KeyPuzzle>();
            yield return new WaitForSeconds(0.15f);
        }
    }
}
