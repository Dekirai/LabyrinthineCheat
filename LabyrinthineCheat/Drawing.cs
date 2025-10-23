using LabyrinthineCheat;
using System;
using UnityEngine;

namespace Labyrinthine.Utilities
{
    internal class Drawing
    {
        private static Camera _gameCamera;
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);
        public static Camera GameCamera
        {
            get
            {
                if (_gameCamera == null || !_gameCamera.isActiveAndEnabled)
                {
                    _gameCamera = Camera.main; // Reassign if lost
                }
                return _gameCamera;
            }
        }

        public static void DrawString(Vector2 position, string label, Color color, int fontSize, bool centered = true)
        {
            GUI.color = color;
            Drawing.StringStyle.fontSize = fontSize;
            Drawing.StringStyle.normal.textColor = color;
            GUIContent content = new GUIContent(label);
            Vector2 vector = Drawing.StringStyle.CalcSize(content);
            GUI.Label(new Rect(centered ? (position - vector / 2f) : position, vector), content, Drawing.StringStyle);
        }

        public static void TextWithDistance(Vector3 target, string text, Color color, Camera? relativeTo = null)
        {
            Camera camera = relativeTo ?? GameCamera;

            if (camera == null || Laby.PlayerControl == null || Laby.PlayerControl.transform == null)
                return;

            Vector3 position = camera.WorldToScreenPoint(target);

            // Only draw if within visible screen
            if (position.z > 0 && position.x >= 0 && position.x <= Screen.width && position.y >= 0 && position.y <= Screen.height)
            {
                float distanceToPlayer = Vector3.Distance(Laby.PlayerControl.transform.position, target);

                if(distanceToPlayer < 1000f)
                    DrawString(new Vector2(position.x, Screen.height - position.y), text + " [" + Math.Round(distanceToPlayer, 1) + "m]", color, 12, true);
            }
        }

        public static void TextWithDistanceMonster(Vector3 target, string text, Camera? relativeTo = null)
        {
            var camera = relativeTo ?? GameCamera;

            if (camera == null || Laby.PlayerControl == null || Laby.PlayerControl.transform == null)
                return;

            Vector3 vector = camera.WorldToScreenPoint(target);

            if (vector.z > 0 && vector.x >= 0 && vector.x <= Screen.width && vector.y >= 0 && vector.y <= Screen.height)
            {
                double distanceToMonster = Math.Round(Vector3.Distance(Laby.PlayerControl.transform.position, target), 1);

                if(distanceToMonster < 1000f)
                {
                    Color color = distanceToMonster < 100f ? Color.red : Color.green;
                    DrawString(new Vector2(vector.x, Screen.height - vector.y), text + " [" + distanceToMonster + "m]", color, 12, true);
                }
            }
        }
    }
}