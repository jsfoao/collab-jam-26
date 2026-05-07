using System;
using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "InputAssetsRegistry", menuName = "Input/Input Assets Registry")]
    public class InputAssetsRegistry : ScriptableObject
    {
        public static InputAssetsRegistry Instance { get; private set; }

        [Serializable]
        public struct Entry
        {
            public string guid;
            public UnityEngine.Object asset;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        private Dictionary<string, UnityEngine.Object> lookup;

        private void OnEnable()
        {
            if (Instance == null || Instance == this)
            {
                Instance = this;
            }
            RebuildLookup();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildLookup();
        }

        public void SetEntries(List<Entry> newEntries)
        {
            entries = newEntries ?? new List<Entry>();
            RebuildLookup();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public bool TryGet<T>(string guid, out T asset) where T : UnityEngine.Object
        {
            asset = null;
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            if (lookup == null)
            {
                RebuildLookup();
            }

            if (!lookup.TryGetValue(guid, out UnityEngine.Object obj))
            {
                return false;
            }

            asset = obj as T;
            return asset != null;
        }

        private void RebuildLookup()
        {
            lookup = new Dictionary<string, UnityEngine.Object>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Count; i++)
            {
                Entry entry = entries[i];
                if (string.IsNullOrEmpty(entry.guid) || entry.asset == null)
                {
                    continue;
                }

                lookup[entry.guid] = entry.asset;
            }
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}