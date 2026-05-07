using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Input
{
    public static class InputDebugGuiRenderer
    {
        public static List<InputManager> FindAllInputManagers()
        {
            InputManager[] managers = Object.FindObjectsByType<InputManager>();
            return new List<InputManager>(managers);
        }

        public static void Draw(ref InputManager inputManager)
        {
            List<InputManager> allInputManagers = FindAllInputManagers();
            if (allInputManagers.Count == 0)
            {
                GUILayout.Label("No InputManager found in scene.");
                return;
            }

            DrawInputManagerSelector(ref inputManager, allInputManagers);

            if (inputManager == null)
            {
                GUILayout.Label("InputManager instance not found.");
                return;
            }

            DrawHeader(inputManager);

            GUI.color = Color.white;
            foreach (InputMappingObject inputMapping in inputManager.ActiveMappings)
            {
                if (inputMapping == null)
                {
                    continue;
                }

                GUILayout.BeginVertical("box");
                GUI.color = Color.yellow;
                GUILayout.Label(inputMapping.name, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUI.color = Color.white;

                foreach (InputActionObject inputActionObject in inputMapping.actions)
                {
                    if (inputActionObject == null)
                    {
                        continue;
                    }

                    GUILayout.BeginVertical("box");
                    DrawActionState(inputActionObject);
                    DrawActionValue(inputActionObject.action);
                    DrawBindings(inputActionObject.action);
                    DrawContexts(inputManager, inputActionObject);
                    GUILayout.EndVertical();

                    GUILayout.Space(10);
                }

                GUILayout.EndVertical();

                GUILayout.Space(20);
            }

            GUI.color = Color.white;
        }

        private static void DrawHeader(InputManager inputManager)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Input Debug", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUILayout.Label("Manager: " + inputManager.name);
            GUILayout.EndVertical();
            GUILayout.Space(8f);
        }

        private static void DrawInputManagerSelector(ref InputManager selectedInputManager, List<InputManager> allInputManagers)
        {
            if (selectedInputManager == null || !allInputManagers.Contains(selectedInputManager))
            {
                selectedInputManager = allInputManagers[0];
            }

            GUILayout.BeginVertical("box");
            GUILayout.Label("Input Manager", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

#if UNITY_EDITOR
            string[] options = new string[allInputManagers.Count];
            int selectedIndex = 0;
            for (int i = 0; i < allInputManagers.Count; i++)
            {
                InputManager manager = allInputManagers[i];
                options[i] = manager != null ? $"{manager.name} ({manager.GetEntityId()})" : "None";
                if (manager == selectedInputManager)
                {
                    selectedIndex = i;
                }
            }

            int popupIndex = EditorGUILayout.Popup("Active", selectedIndex, options);
            selectedInputManager = allInputManagers[popupIndex];
#else
            GUILayout.Label(selectedInputManager != null ? selectedInputManager.name : "None");
#endif

            GUILayout.EndVertical();
            GUILayout.Space(8f);
        }

        private static void DrawActionState(InputActionObject inputActionObject)
        {
            if (inputActionObject.action == null)
            {
                GUI.color = Color.red;
                GUILayout.Label(inputActionObject.name + " (Missing InputAction)", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUI.color = Color.white;
                return;
            }

            if (!inputActionObject.action.enabled)
            {
                GUI.color = Color.red;
            }
            else if (inputActionObject.action.IsPressed())
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }

            GUILayout.Label(inputActionObject.name, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            GUI.color = Color.white;
        }

        private static void DrawActionValue(InputAction action)
        {
            if (action == null)
            {
                GUILayout.Label("Value: None");
                return;
            }

            object value;
            try
            {
                value = action.ReadValueAsObject();
            }
            catch
            {
                value = null;
            }

            GUILayout.Label(value != null ? "Value: " + value : "Value: None");
        }

        private static void DrawBindings(InputAction action)
        {
            if (action == null)
            {
                return;
            }

            foreach (InputBinding binding in action.bindings)
            {
                GUI.color = Color.white;
                GUILayout.Label(binding.ToString());
            }
        }

        private static void DrawContexts(InputManager inputManager, InputActionObject inputActionObject)
        {
            if (!inputManager.TryGetActiveContexts(inputActionObject, out var contexts) || contexts.Count == 0)
            {
                GUI.color = Color.gray;
                GUILayout.Label("No active contexts");
                GUI.color = Color.white;
                return;
            }

            foreach (InputActionContext context in contexts)
            {
                if (context == null)
                {
                    continue;
                }

                GUI.color = Color.gray;
                GUILayout.Label(context.GetType().Name + " (" + context.Owner.name + ")");
                GUI.color = Color.white;
            }
        }
    }
}
