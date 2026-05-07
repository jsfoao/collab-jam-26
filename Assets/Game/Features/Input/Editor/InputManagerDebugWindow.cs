using UnityEditor;
using UnityEngine;

namespace Input
{
    public class InputManagerDebugWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private InputManager selectedInputManager;

        [MenuItem("Tools/Input/Debug")]
        public static void ShowWindow()
        {
            InputManagerDebugWindow window = GetWindow<InputManagerDebugWindow>("Input");
            window.minSize = new Vector2(360f, 300f);
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to debug InputManager instances.", MessageType.Info);
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            InputDebugGuiRenderer.Draw(ref selectedInputManager);
            GUILayout.EndScrollView();
            GUI.color = Color.white;
        }
    }
}
