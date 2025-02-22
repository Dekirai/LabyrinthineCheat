using UnityEngine;
using Labyrinthine.Utilities;

namespace LabyrinthineCheat
{
    public static class ESP
    {
        public static Camera GameCamera = Camera.main;

        public static void Render()
        {
            RenderMonsters();
            RenderPlayers();
        }

        private static void RenderPlayers()
        {
            foreach (var player in Laby.GameManager.Players)
            {
                Drawing.TextWithDistance(player.transform, player.playerName);
            }
        }

        private static void RenderMonsters()
        {
            foreach (var ai in Laby.AIControllers)
            {
                Drawing.TextWithDistance(ai.transform, ai.monsterType.ToString());
            }
        }
    }
}