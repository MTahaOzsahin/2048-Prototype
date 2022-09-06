using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Scripts.InputActions
{
    [DefaultExecutionOrder(-1)]
    public class SwipeController : MonoBehaviour
    {
        InputsControl inputAction;

        public delegate void StartTouch(Vector2 position, float time);
        public event StartTouch OnStartTouch;
        public delegate void EndTouch(Vector2 position, float time);
        public event EndTouch OnEndTouch;

        private void Awake()
        {
            inputAction = new InputsControl();
        }
        private void OnEnable()
        {
            inputAction.Enable();
        }
        private void OnDisable()
        {
            inputAction.Disable();
        }
        private void Start()
        {
            inputAction.Mobile.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
            inputAction.Mobile.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
        }

        //To get start and end position as vector2.
        void StartTouchPrimary(InputAction.CallbackContext context)
        {
            if (OnStartTouch != null) OnStartTouch(inputAction.Mobile.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
        void EndTouchPrimary(InputAction.CallbackContext context)
        {
            if (OnEndTouch != null) OnEndTouch(inputAction.Mobile.PrimaryPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }
}