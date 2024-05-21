using UnityEngine;

namespace Prototype.Scripts.Grid
{
    public class Node : MonoBehaviour
    {
        public Vector2 Pos => transform.position;

        public Block occupiedBlock;
    }
}
