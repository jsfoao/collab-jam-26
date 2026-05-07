using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract class InputActionContext
    {
        private bool enabled = false;
        public bool Enabled => enabled;
        
        private InputActionObject inputAction;

        private MonoBehaviour owner;
        public MonoBehaviour Owner => owner;

        public InputActionContext(InputActionObject inputAction, MonoBehaviour owner)
        {
            this.inputAction = inputAction;
            this.owner = owner;
        }

        public void Enable()
        {
            if (enabled)
            {
                return;
            }
            enabled = true;

            // Subscribe to input action events
            inputAction.action.started += Start_Internal;
            inputAction.action.performed += Perform_Internal;
            inputAction.action.canceled += Finish_Internal;
        }

        public void Disable()
        {
            if (!enabled)
            {
                return;
            }
            enabled = false;

            // Unsubscribe from input action events
            inputAction.action.started -= Start_Internal;
            inputAction.action.performed -= Perform_Internal;
            inputAction.action.canceled -= Finish_Internal;
        }

        ~InputActionContext()
        {
            Disable();
        }

        private void Start_Internal(InputAction.CallbackContext ctx)
        {
            if (!CanTrigger())
            {
                return;
            }
            Start(ctx);
        }
        private void Perform_Internal(InputAction.CallbackContext ctx)
        {
            if (!CanTrigger())
            {
                return;
            }
            Perform(ctx);
        }
        private void Finish_Internal(InputAction.CallbackContext ctx)
        {
            if (!CanTrigger())
            {
                return;
            }
            Finish(ctx);
        }
        private void Update_Internal()
        {
            if (!CanTrigger())
            {
                return;
            }
            Update();
        }

        private bool CanTrigger()
        {
            if (!enabled)
            {
                return false;
            }
            if (!inputAction.TriggerWhenPaused && InputManager.IsPaused())
            {
                return false;
            }
            return true;
        }
    
        public virtual void Start(InputAction.CallbackContext ctx) {}
        public virtual void Perform(InputAction.CallbackContext ctx) {}
        public virtual void Update() {}
        public virtual void Finish(InputAction.CallbackContext ctx) {}
    }
}
