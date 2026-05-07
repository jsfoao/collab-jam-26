using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "IM_", menuName = "Input/Input Mapping")]
    public class InputMappingObject : ScriptableObject
    {
        [SerializeField] private string description = "Description";
        public string Description => description;

        public List<InputActionObject> actions = new List<InputActionObject>();

#if UNITY_EDITOR
        void OnValidate()
        {
            List<InputActionObject> duplicates = new List<InputActionObject>();
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] == null)
                {
                    continue;
                }
                if (duplicates.Contains(actions[i]))
                {
                    continue;
                }
                for (int j = i + 1; j < actions.Count; j++)
                {
                    if (actions[j] == null)
                    {
                        continue;
                    }
                    if (actions[i] == actions[j] && !duplicates.Contains(actions[i]))
                    {
                        duplicates.Add(actions[i]);
                    }
                }
            }

            List<InputActionObject> removed = new List<InputActionObject>();
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i] == null)
                {
                    continue;
                }
                if (duplicates.Contains(actions[i]))
                {
                    if (!removed.Contains(actions[i]))
                    {
                        Debug.LogWarning($"Input Mapping '{name}' already contains action. Duplicates aren't allowed: '{actions[i].name}'", this);
                        removed.Add(actions[i]);
                        actions.RemoveAt(i);
                    }
                }
            }
        }
#endif
    }
}
