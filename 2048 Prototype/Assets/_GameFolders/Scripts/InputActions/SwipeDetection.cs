using Prototype.Scripts.Managers;
using UnityEngine;

namespace Prototype.Scripts.InputActions
{
    public class SwipeDetection : SingletonMB<SwipeDetection>
    {
        /* This script made to detect is swipe started and which direction to swipe.
         */

        SwipeController swipeController;
        //How far start and end point must be
        [SerializeField] float minimumDistance = 0.2f;
        //How long start and end time must be
        [SerializeField] float maximumTime = 1f;
        //How close match direction and standard vectors must be. Higher value means sharper swipe.
        [SerializeField, Range(0f, 1f)] float directionThreshold = 0.5f;
        [SerializeField] private GameplayManager gameplayManager;

        private Vector2 startPosition;
        private float startTime;
        private Vector2 endPosition;
        private float endTime;

        public event System.Action<Vector2> OnUpSwipe;
        public event System.Action<Vector2> OnDownSwipe;
        public event System.Action<Vector2> OnRightSwipe;
        public event System.Action<Vector2> OnLeftUpSwipe;


        private void Awake()
        {
            swipeController = GetComponent<SwipeController>();
        }
        private void OnEnable()
        {
            swipeController.OnStartTouch += SwipeStart;
            swipeController.OnEndTouch += SwipeEnd;
        }
        private void OnDisable()
        {
            swipeController.OnStartTouch -= SwipeStart;
            swipeController.OnEndTouch -= SwipeEnd;
        }
        void SwipeStart(Vector2 position, float time)
        {
            if (gameplayManager.GetCurrentGameState() == GameState.OnPause) return;
            startPosition = position;
            startTime = time;
        }
        void SwipeEnd(Vector2 position, float time)
        {
            if (gameplayManager.GetCurrentGameState() == GameState.OnPause) return;
            endPosition = position;
            endTime = time;
            DetectSwipe();
        }
        void DetectSwipe()
        {
            //Check for swipe lenght and duration is enough to perform.
            if (Vector3.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maximumTime)
            {
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
        }

        //Check which direction to swipe.
        void SwipeDirection(Vector2 direction)
        {
            /*Vector 2 Dot
             * returns 1 if given two direction looking same direction 
             * returns 0 if perpendicular to each other
             * returns -1 if opposite direction
             */

            float vector2DotUp = Vector2.Dot(Vector2.up, direction);
            float vector2DotRight = Vector2.Dot(Vector2.right, direction);

            if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
            {
                OnUpSwipe?.Invoke(Vector2.up);
            }
            else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
            {
                OnRightSwipe?.Invoke(Vector2.right);
            }
            else if (Vector2.Dot(-Vector2.right, direction) > directionThreshold)
            {
                OnLeftUpSwipe?.Invoke(Vector2.left);
            }
            else if (Vector2.Dot(-Vector2.up, direction) > directionThreshold)
            {
                OnDownSwipe?.Invoke(Vector2.down);
            }
        }
    }
}
