using UnityEngine;
using Labyrinthine.Utilities;
using MelonLoader;

namespace LabyrinthineCheat
{
    public static class ESP
    {
        public static Camera GameCamera = Camera.main;

        public static void Render()
        {
            if (Laby.PlayerInCase)
            {
                RenderMonsters();
                RenderPlayers();
            }
        }

        private static void RenderPlayers()
        {
            foreach (var player in Laby.GameManager.Players)
            {
                if (player != null && player.transform != null)
                    Drawing.TextWithDistance(player.transform, player.playerName);
            }
        }

        private static void RenderMonsters()
        {
            foreach (var ai in Laby.AIControllers)
            {
                if(ai != null && ai.transform != null)
                    Drawing.TextWithDistance(ai.transform, ai.monsterType.ToString().Replace("_", " "));    
            }
        }
    }
}