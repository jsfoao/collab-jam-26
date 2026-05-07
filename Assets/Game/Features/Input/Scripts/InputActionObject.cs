using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "IA_", menuName = "Input/Input Action")]
    public class InputActionObject : ScriptableObject
    {
        [SerializeField] private string displayName = "Action";
        public string DisplayName => displayName;

        [SerializeField] private string description = "Description";
        public string Description => description;

        [SerializeField] private bool triggerWhenPaused = false;
        public bool TriggerWhenPaused => triggerWhenPaused;

        [SerializeField] public InputAction action;

        public void Enable()
        {
            action.Enable();
        }

        public void Disable()
        {
            action.Disable();
        }
    }   
}
