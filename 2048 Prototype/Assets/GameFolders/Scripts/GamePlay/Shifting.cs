using Prototype.Scripts.InputActions;
using Prototype.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Scripts.GamePlay
{
    public class Shifting : MonoBehaviour
    {
        //Getting inputs from InputSystem.
        InputsControl inputAction;
        SwipeDetection swipeDetection;
        GameplayManager gameplayManager;

        private void Awake()
        {
            inputAction = new InputsControl();
            swipeDetection = SwipeDetection.Instance;
            gameplayManager = GetComponent<GameplayManager>();
        }
        private void OnEnable()
        {
            inputAction.Enable();
            inputAction.Keyboard.Keyboard.started += GetKeyboardInput;
            swipeDetection.OnUpSwipe += GetMobileInput;
            swipeDetection.OnDownSwipe += GetMobileInput;
            swipeDetection.OnRightSwipe += GetMobileInput;
            swipeDetection.OnLeftUpSwipe += GetMobileInput;

        }
        private void OnDisable()
        {
            inputAction.Disable();
            inputAction.Keyboard.Keyboard.started -= GetKeyboardInput;
            swipeDetection.OnUpSwipe -= GetMobileInput;
            swipeDetection.OnDownSwipe -= GetMobileInput;
            swipeDetection.OnRightSwipe -= GetMobileInput;
            swipeDetection.OnLeftUpSwipe -= GetMobileInput;
        }
        void GetKeyboardInput(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            gameplayManager.Shift(direction);
        }
        void GetMobileInput(Vector2 direction)
        {
            gameplayManager.Shift(direction);
        }
    }
}
