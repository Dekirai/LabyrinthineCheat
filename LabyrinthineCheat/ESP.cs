using Labyrinthine.Utilities;
using UnityEngine;

namespace LabyrinthineCheat
{
    public static class ESP
    {
        public static void Render()
        {
            if (Laby.PlayerInCase && Laby.ESPEnabled)
            {
                RenderMonsters();
                RenderPlayers();
                RenderSafezones();
            }
        }

        private static void RenderSafezones()
        {
            int index = 1;
            foreach (var safezone in Laby.Safezones)
            {
                Drawing.TextWithDistance(safezone, $"Safezone {index}", Color.magenta);
                index++;
            }
        }

        private static void RenderPlayers()
        {
            foreach (var player in Laby.GameManager.Players)
            {
                if (player != null && player.transform != null)
                    Drawing.TextWithDistance(player.transform.position, player.ClientData.Name, Color.cyan);
            }
        }

        private static void RenderMonsters()
        {
            foreach (var ai in Laby.AIControllers)
            {
                if(ai != null && ai.transform != null)
                    Drawing.TextWithDistanceMonster(ai.transform.position, ai.monsterType.ToString().Replace("_", " "));    
            }
        }
    }
}