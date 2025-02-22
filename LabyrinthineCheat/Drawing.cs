using LabyrinthineCheat;
using System;
using UnityEngine;

namespace Labyrinthine.Utilities
{
    internal class Drawing
    {
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
        private static Camera _gameCamera;

        public static void DrawString(Vector2 position, string label, Color color, int fontSize, bool centered = true)
        {
            GUI.color = color;
            Drawing.StringStyle.fontSize = fontSize;
            Drawing.StringStyle.normal.textColor = color;
            GUIContent content = new GUIContent(label);
            Vector2 vector = Drawing.StringStyle.CalcSize(content);
            GUI.Label(new Rect(centered ? (position - vector / 2f) : position, vector), content, Drawing.StringStyle);
        }

        public static void TextWithDistance(Transform target, string text, Camera? relativeTo = null)
        {
            var camera = relativeTo ?? GameCamera;

            Vector3 position = target.position;
            Vector3 vector = camera.WorldToScreenPoint(position);

            if ((vector.x < 0f || vector.x > (float)Screen.width || vector.y < 0f || vector.y > (float)Screen.height || vector.z > 0f))
            {
                float distanceToPlayer = (float)Math.Round((double)Vector3.Distance(Laby.PlayerControl.transform.position, target.position), 1);

                if (vector.z >= 0f && distanceToPlayer < 1000f)
                    DrawString(new Vector2(vector.x, (float)Screen.height - vector.y), text + " [" + distanceToPlayer.ToString() + "m]", Color.green, 12, true);
            }
        }

        public static void TextWithDistanceMonster(Transform target, string text, Camera? relativeTo = null)
        {
            var camera = relativeTo ?? GameCamera;

            Vector3 position = target.position;
            Vector3 vector = camera.WorldToScreenPoint(position);

            if ((vector.x < 0f || vector.x > (float)Screen.width || vector.y < 0f || vector.y > (float)Screen.height || vector.z > 0f))
            {
                float distanceToMonster = (float)Math.Round((double)Vector3.Distance(Laby.PlayerControl.transform.position, target.position), 1);

                if (vector.z >= 0f && distanceToMonster < 100f)
                    DrawString(new Vector2(vector.x, (float)Screen.height - vector.y), text + " [" + distanceToMonster.ToString() + "m]", Color.red, 12, true);
                else if (vector.z >= 0f && distanceToMonster < 1000f)
                    DrawString(new Vector2(vector.x, (float)Screen.height - vector.y), text + " [" + distanceToMonster.ToString() + "m]", Color.green, 12, true);
            }
        }
    }
}