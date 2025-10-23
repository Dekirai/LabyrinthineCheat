using MelonLoader;
using UnityEngine;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2Cpp;
using System.Collections;

namespace LabyrinthineCheat
{
    public class Laby : MelonMod
    {
        public static bool PlayerInCase = false;
        public static bool ESPEnabled = false;
        public static bool isAIEnabled = true;

        public static float FlashlightMultiplier = 1f;
        public static float DefaultFlashlightIntensity = 47.7f;
        public static float DefaultPointFlashlightIntensity = 30f;

        public static float JumpHeight = 1.8f;
        public static float MovementSpeed = 4.4f;

        public static int? CurrentSceneIndex;
        public static string? CurrentSceneName;

        public static List<Vector3> Safezones = new List<Vector3>();

        public static Camera? GameCamera { get; set; }
        public static GameManager? GameManager { get; set; }
        public static PlayerControl? PlayerControl { get; set; }
        public static AIController[] AIControllers { get; set; } = Array.Empty<AIController>();

        private bool showMenu = false;
        private Menu menu = new Menu();

        public override void OnInitializeMelon()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MelonLogger.Error($"Unhandled Exception: {e.ExceptionObject}");
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                MelonLogger.Error($"Unobserved Task Exception: {e.Exception}");
                e.SetObserved();
            };
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            CurrentSceneIndex = buildIndex;
            CurrentSceneName = sceneName;

            if (buildIndex >= 4 && buildIndex != 8)
            {
                MelonCoroutines.Start(CollectCaseGameObjectsAndData());
            }
        }
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MovementSpeed = 4.4f;
            JumpHeight = 1.8f;
            if (PlayerInCase)
            {
                Hacks.SetMovementSpeed(MovementSpeed);
                Hacks.SetJumpHeight(JumpHeight);
            }
            ESPEnabled = false;
            PlayerInCase = false;
            isAIEnabled = true;
            FlashlightMultiplier = 1f;
            AIControllers = null;
            GameManager = null;
            PlayerControl = null;
            GameCamera = null;
            Safezones.Clear();
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
                menu.StartMenu();
            }

            if (ESPEnabled)
            {
                ESP.Render();
            }
        }

        private IEnumerator CollectCaseGameObjectsAndData()
        {
            while (GameManager == null)
            {
                GameManager = GameObject.FindObjectOfType<GameManager>();
                yield return new WaitForSeconds(0.5f);
            }

            while (PlayerControl == null)
            {
                PlayerControl = GameObject.FindObjectOfType<PlayerControl>();
                yield return new WaitForSeconds(0.5f);
            }

            MelonLogger.Msg("All required game objects collected!");

            PlayerInCase = true;

            Safezones.AddRange(Hacks.GetAllSafezones());
            MelonLogger.Msg($"Found {Safezones?.Count} safe zones!");

            GetInfoCosmeticInCase();

            while (AIControllers == null || AIControllers.Length == 0)
            {
                AIControllers = GameObject.FindObjectsOfType<AIController>();
                yield return new WaitForSeconds(0.5f);
            }

            MelonLogger.Msg("All optional game objects collected!");
        }

        private void GetInfoCosmeticInCase()
        {
            var pickup = Hacks.GetPickupCosmeticInCase();

            if (pickup != null)
            {
                var pickupName = pickup.name
                    .Replace("CPickup - ", "")
                    .Replace("Customization Pickup - ", "")
                    .Replace("(Clone)", "")
                    .Replace("_", " ")
                    .Trim();

                MelonLogger.Msg($"Cosmetic in this case if any: {pickupName} with itemID {pickup.itemID}");
            }
        }
    }
}
