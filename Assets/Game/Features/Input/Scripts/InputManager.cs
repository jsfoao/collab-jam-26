using System;
using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputMappingObject[] defaultMappings;
        
        private List<InputMappingObject> activeMappings = new List<InputMappingObject>();
        private Dictionary<InputActionObject, List<InputActionContext>> activeContexts = new Dictionary<InputActionObject, List<InputActionContext>>();

        public IReadOnlyList<InputMappingObject> ActiveMappings => activeMappings;

        [Header("Debug")]
        public bool debugLog = false;

        public bool TryGetActiveContexts(InputActionObject inputAction, out IReadOnlyList<InputActionContext> contexts)
        {
            if (activeContexts.TryGetValue(inputAction, out List<InputActionContext> contextList))
            {
                contexts = contextList;
                return true;
            }

            contexts = Array.Empty<InputActionContext>();
            return false;
        }

        void Start()
        {
            Init();
        }

        public virtual void Init()
        {   
            if (defaultMappings != null)
            {
                foreach (InputMappingObject inputMapping in defaultMappings)
                {
                    AddInputMapping(inputMapping);
                }
            }
        }

        void Update()
        {
            // Update active contexts for triggered actions
            foreach (InputMappingObject inputMapping in activeMappings)
            {
                foreach (InputActionObject inputAction in inputMapping.actions)
                {
                    if (activeContexts.ContainsKey(inputAction))
                    {
                        if (!inputAction.TriggerWhenPaused && IsPaused())
                        {
                            continue;
                        }
                        if (!inputAction.action.IsPressed())
                        {
                            continue;
                        }
                        foreach (InputActionContext context in activeContexts[inputAction])
                        {
                            if (context != null && context.Enabled)
                            {
                                context.Update();
                            }
                        }
                    }
                }
            }
        }

        public bool AddInputMapping(InputMappingObject inputMapping)
        {
            if (!inputMapping)
            {
                return false;
            }
            if (activeMappings.Contains(inputMapping))
            {
                return false;
            }
            
            activeMappings.Add(inputMapping);
            EnableInputMapping(inputMapping);

            if (debugLog)
            {
                Debug.Log("Input Manager: Added input mapping " + inputMapping);
            }
            return true;
        }

        public bool RemoveInputMapping(InputMappingObject inputMapping)
        {
            if (!inputMapping)
            {
                return false;
            }
            if (!activeMappings.Contains(inputMapping))
            {
                return false;
            }
            activeMappings.Remove(inputMapping);
            DisableInputMapping(inputMapping);
            
            if (debugLog)
            {
                Debug.Log("Input Manager: Removed input mapping " + inputMapping);
            }
            return true;
        }

        private void EnableInputMapping(InputMappingObject inputMapping)
        {
            foreach (InputActionObject inputAction in inputMapping.actions)
            {
                // Enable contexts for this action
                List<InputActionContext> contexts;
                if (activeContexts.TryGetValue(inputAction, out contexts))
                {
                    foreach (InputActionContext context in contexts)
                    {
                        context.Enable();
                    }
                }

                // Enable input action
                inputAction.action.Enable();
            }
        }

        private void DisableInputMapping(InputMappingObject inputMapping)
        {
            foreach (InputActionObject inputAction in inputMapping.actions)
            {
                // Disable contexts for this action
                if (activeContexts.TryGetValue(inputAction, out List<InputActionContext> contexts))
                {
                    foreach (InputActionContext context in contexts)
                    {
                        context.Disable();
                    }
                }

                // Disable input action
                inputAction.action.Disable();
            }
        }

        public T AddContext<T>(InputActionObject inputAction, MonoBehaviour owner) where T : InputActionContext
        {
            // Create context
            object[] args = new object[] { inputAction, this, owner };
            T context = (T)Activator.CreateInstance(typeof(T), args);
            
            // Add context to active contexts
            if (!activeContexts.ContainsKey(inputAction))
            {
                activeContexts[inputAction] = new List<InputActionContext>();
            }

            // Enable context
            activeContexts[inputAction].Add(context);
            if (inputAction.action.enabled)
            {
                context.Enable();
            }
            return context;
        }

        public void RemoveContext(InputActionObject inputAction, InputActionContext context)
        {
            if (activeContexts.ContainsKey(inputAction))
            {
                activeContexts[inputAction].Remove(context);
            }
        }

        public static bool IsPaused()
        {
            return Time.timeScale == 0f;
        }
    }
}