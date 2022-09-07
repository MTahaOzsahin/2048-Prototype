using DG.Tweening;
using Prototype.Scripts.Grid;
using Prototype.Scripts.InputActions;
using Prototype.Scripts.Managers.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Scripts.Managers
{
    public class GameplayManager : MonoBehaviour
    {
        //To invoke scoreManager.
        [SerializeField] ScoreManager scoreManager;

        //To play sounds
        [SerializeField]SoundManager soundManager;

        //To select grids width and height.
        [SerializeField]GridManager gridManager;

        //Simple win and lose panels.
        [SerializeField] GameObject winScreen, loseScreen;

        //The values that blocks holds.
        [SerializeField] List<BlockType> types;

        //List for nodes and blocks.
        List<Node> nodes;
        List<Block> blocks;

        List<Block> beforeShiftBlocks;
        List<Block> afterShiftBlocks;

        //GameStates.
        GameState state;
        int round;

        //Getting  blocks value.
        BlockType GetBlockTypeByValue(int value) => types.First(t => t.value == value);

       
        private void Start()
        {
            ChangeGameState(GameState.GenerateLevel);
        }

        /// <summary>
        /// Changing game states.
        /// </summary>
        /// <param name="newState"></param>
        void ChangeGameState(GameState newState)
        {
            state = newState;

            switch (newState)
            {
                case GameState.GenerateLevel:
                    Time.timeScale = 1;
                    GenerateGrid();
                    break;
                case GameState.SpawningBlocks:

                    SpawnBlocks(round++ == 0 ? 2 : 1);
                    break;
                case GameState.WaitingInput:
                    break;
                case GameState.Shifting:
                    break;
                case GameState.Win:
                    Time.timeScale = 0;
                    winScreen.SetActive(true);
                    soundManager.WinSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    break;
                case GameState.Lose:
                    Time.timeScale = 0;
                    loseScreen.SetActive(true);
                    soundManager.LoseSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    break;
                case GameState.Pause:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Generating base grid.
        /// </summary>
        void GenerateGrid()
        {
            round = 0;
            nodes = new List<Node>();
            beforeShiftBlocks = new List<Block>();
            afterShiftBlocks = new List<Block>();
            blocks = new List<Block>();
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                for (int y = 0; y < gridManager.gridHeight; y++)
                {
                    var node = Instantiate(gridManager.nodePrefab, new Vector2(x, y), Quaternion.identity);
                    nodes.Add(node);
                }
            }

            var center = new Vector2((float)gridManager.gridWidth / 2 - 0.5f, (float)gridManager.gridHeight / 2 - 0.5f); //We subtract 0.5 is because our nodes are centered on each whole vector.

            var board = Instantiate(gridManager.boardPrefab, center, Quaternion.identity);
            board.size = new Vector2(gridManager.gridWidth, gridManager.gridHeight);

            Camera.main.transform.position = new Vector3(center.x, center.y + 1.5f, -10); // +1.5f to y-axis for better looking
            Camera.main.orthographicSize = gridManager.gridWidth;

            ChangeGameState(GameState.SpawningBlocks);
        }

        /// <summary>
        /// Spawning blocks generally.
        /// </summary>
        /// <param name="amount"></param>
        void SpawnBlocks(int amount)
        {

            var freeNodes = nodes.Where(n => n.occupiedBlock == null).OrderBy(b => Random.value).ToList();

            foreach (var node in freeNodes.Take(amount))
            {
                SpawnBlock(node, Random.value > 0.8f ? 4 : 2);
            }

            if (freeNodes.Count() == 1)
            {
                ChangeGameState(GameState.Lose);
                return;
            }

            ChangeGameState(blocks.Any(b => b.value == gridManager.winCondition) ? GameState.Win : GameState.WaitingInput);
        }
        void SpawnBlock(Node node, int value)
        {
            var block = Instantiate(gridManager.blockPrefab, node.Pos, Quaternion.identity);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            blocks.Add(block);
        }
        public void UndoFunction()
        {
            Debug.Log(beforeShiftBlocks.Count);
            Debug.Log(afterShiftBlocks.Count);
            foreach (var block in afterShiftBlocks)
            {
                if (!beforeShiftBlocks.Contains(block))
                {
                    RemoveBlock(block);
                }
            }
        }

        /// <summary>
        /// Base shifting mechanics.
        /// </summary>
        /// <param name="direction"></param>
        public void Shift(Vector2 direction)
        {
            if (Time.timeScale == 0) return;

            ChangeGameState(GameState.Shifting);
            soundManager.MoveSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
            beforeShiftBlocks.Clear();
            var orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList(); //Before shifting blocks list.
            List<Vector2> orderedBlockPositions = new List<Vector2>();
            foreach (var block in orderedBlocks)
            {
                orderedBlockPositions.Add(block.Pos);
                beforeShiftBlocks.Add(block);
            }

            if (direction == Vector2.right || direction == Vector2.up) orderedBlocks.Reverse();

            foreach (var block in orderedBlocks)
            {
                var next = block.node;
                do
                {
                    block.SetBlock(next);

                    var possibleNode = GetNodeAtPosition(next.Pos + direction);
                    if (possibleNode != null)
                    {
                        // We know a node is present
                        // If it's possible to merge, set merge
                        if (possibleNode.occupiedBlock != null && possibleNode.occupiedBlock.CanMerge(block.value))
                        {
                            block.MergeBlock(possibleNode.occupiedBlock);
                        }
                        // Otherwise, can we move to this spot?
                        else if (possibleNode.occupiedBlock == null) next = possibleNode;
                        // None hit? End do while loop
                    }


                } while (next != block.node);
            }


            var sequence = DOTween.Sequence();

            foreach (var block in orderedBlocks)
            {
                var movePoint = block.mergingBlock != null ? block.mergingBlock.node.Pos : block.node.Pos;

                sequence.Insert(0, block.transform.DOMove(movePoint, gridManager.travelTime).SetEase(Ease.InQuad));
            }

            sequence.OnComplete(() =>
            {
                var mergeBlocks = orderedBlocks.Where(b => b.mergingBlock != null).ToList();
                foreach (var block in mergeBlocks)
                {
                    MergeBlocks(block.mergingBlock, block);
                    scoreManager.HandleScore(block.value);
                    if (block.value > 63)
                    {
                        soundManager.NiceScoreSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    }
                    else
                    soundManager.MergingSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                }

                ///This section is here to prevent spawning block if there is not shift via compare before blocks list and after block list.
                afterShiftBlocks.Clear();
                var orderedBlockAfterShift = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList(); // After shifting block list.
                List<Vector2> orderedBlockPositionsAfterShift = new List<Vector2>();
                foreach (var block in orderedBlockAfterShift)
                {
                    orderedBlockPositionsAfterShift.Add(block.Pos);
                    afterShiftBlocks.Add(block);
                }
                if (!CompareLists(orderedBlockPositions,orderedBlockPositionsAfterShift))
                {
                    ChangeGameState(GameState.SpawningBlocks);
                }
            });
        }

        /// <summary>
        /// Merging blocks that matching.
        /// </summary>
        /// <param name="baseBlock"></param>
        /// <param name="mergingBlock"></param>
        void MergeBlocks(Block baseBlock, Block mergingBlock)
        {
            var newValue = baseBlock.value * 2;

            //Instantiate(floatingTextPrefab, baseBlock.Pos, Quaternion.identity).Init(newValue);

            SpawnBlock(baseBlock.node, newValue);

            RemoveBlock(baseBlock);
            RemoveBlock(mergingBlock);
        }

        /// <summary>
        /// Removing matched block to prevent match again.
        /// </summary>
        /// <param name="block"></param>
        void RemoveBlock(Block block)
        {
            blocks.Remove(block);
            Destroy(block.gameObject);
        }

        Node GetNodeAtPosition(Vector2 pos)
        {
            return nodes.FirstOrDefault(n => n.Pos == pos);
        }

        public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
        {
            if (aListA == null || aListB == null || aListA.Count != aListB.Count)
                return false;
            if (aListA.Count == 0)
                return true;
            Dictionary<T, int> lookUp = new Dictionary<T, int>();
            // create index for the first list
            for (int i = 0; i < aListA.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListA[i], out count))
                {
                    lookUp.Add(aListA[i], 1);
                    continue;
                }
                lookUp[aListA[i]] = count + 1;
            }
            for (int i = 0; i < aListB.Count; i++)
            {
                int count = 0;
                if (!lookUp.TryGetValue(aListB[i], out count))
                {
                    // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                    return false;
                }
                count--;
                if (count <= 0)
                    lookUp.Remove(aListB[i]);
                else
                    lookUp[aListB[i]] = count;
            }
            // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
            return lookUp.Count == 0;

            /// <summary>
            /// Example how to use.
            /// </summary>
            //List<int> A = new List<int>(new int[] { 1, 5, 6, 7, 3, 1 });
            //List<int> B = new List<int>(new int[] { 6, 3, 5, 1, 1, 7 });
            //if (CompareLists(A, B))
            //    Debug.Log("Equal");
            //else
            //    Debug.Log("Not equal");
        }
    }

    /// <summary>
    /// Game States to control game.
    /// </summary>
    public enum GameState
    {
        GenerateLevel,
        SpawningBlocks,
        WaitingInput,
        Shifting,
        Win,
        Lose,
        Pause
    }
    /// <summary>
    /// Blocks types which hold their values and colors.
    /// </summary>
    [System.Serializable]
    public struct BlockType
    {
        //Blocks value.
        public int value;

        //Blocks color.
        public Color color;
    }
}



