using DG.Tweening;
using Prototype.Scripts.Grid;
using Prototype.Scripts.Interfaces;
using Prototype.Scripts.Managers.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.Scripts.Managers
{
    public class GameplayManager : MonoBehaviour , ICompareLists
    {
        //To invoke scoreManager.
        [SerializeField] ScoreManager scoreManager;

        //To play sounds
        [SerializeField]SoundManager soundManager;

        //To select grids width and height.
        [SerializeField]GridManager gridManager;

        //To save blocks on quitting game.
        [SerializeField]DataManager dataManager;

        //The values that blocks holds.
        [SerializeField] List<BlockType> types;

        //List for nodes and blocks.
        List<Node> nodesList;
        List<Block> blocksList;
        List<Block> afterShiftBlocks;
        List<Vector2> occupiedNodesPos;
        List<int> occupiedNodesValue;

        //GameStates.
        private GameState state;
        private int round;
        private GameState previousState;

        //Getting  blocks value.
        BlockType GetBlockTypeByValue(int value) => types.First(t => t.value == value);

        private void Awake()
        {
            DOTween.Init(true, false, LogBehaviour.Default);
            DOTween.useSmoothDeltaTime = true;
            Application.targetFrameRate = 60;
        }


        private void Start()
        {
            if (dataManager.GivingBlocksPos() != null)
            {
                var blocksPos = dataManager.GivingBlocksPos();
                var blockValues = dataManager.GivingBlockValue();

                if (dataManager.NodeNumber() == 16)
                {
                    gridManager.gridWidth = 4;
                    gridManager.gridHeight = 4;
                }
                else if (dataManager.NodeNumber() == 25)
                {
                    gridManager.gridWidth = 5;
                    gridManager.gridHeight = 5;
                }
                else if (dataManager.NodeNumber() == 36)
                {
                    gridManager.gridWidth = 6;
                    gridManager.gridHeight = 6;
                }
                GenerateGrid();
                round = 1;
                for (int i = 0; i < blocksPos.Count; i++)
                {
                    SpawnBlockForUtilization(blocksPos[i], blockValues[i], GetNodeAtPosition(blocksPos[i]));
                }
                ChangeGameState(GameState.WaitingInput);
            }
            else
            {
                ChangeGameState(GameState.GenerateLevel);
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (blocksList == null || blocksList.Count == 0) return;
            var orderedBlocks = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            dataManager.GettingGBlocks(orderedBlocks.Count, orderedBlocks, nodesList.Count);
        }
        private void OnApplicationQuit()
        {
            if (blocksList == null || blocksList.Count == 0) return;
            var orderedBlocks = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            dataManager.GettingGBlocks(orderedBlocks.Count, orderedBlocks, nodesList.Count);
        }

        public GameState GetCurrentGameState()
        {
            return state;
        }

        /// <summary>
        /// Changing game states.
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeGameState(GameState newState)
        {
            if (state != GameState.OnPause) previousState = state;
            state = newState;

            switch (newState)
            {
                case GameState.GenerateLevel:
                    Time.timeScale = 1;
                    GenerateGrid();
                    SpawnBlocks(round++ == 0 ? 2 : 1);
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
                    GameObject winPanel = Instantiate(gridManager.winPanel, FindObjectOfType<UiScoreManager>().gameObject.transform.position,
                        Quaternion.identity, FindObjectOfType<UiScoreManager>().gameObject.transform);
                    winPanel.SetActive(true);
                    soundManager.WinSound(gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    break;
                case GameState.Lose:
                    Time.timeScale = 0;
                    GameObject losePanel = Instantiate(gridManager.losePanel, FindObjectOfType<UiScoreManager>().gameObject.transform.position,
                        Quaternion.identity, FindObjectOfType<UiScoreManager>().gameObject.transform);
                    losePanel.SetActive(true);
                    soundManager.LoseSound(gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    break;
                case GameState.OnPause:
                    //
                    break;
            }
        }

        public void OnPanelOpen()
        {
            ChangeGameState(GameState.OnPause);
        }

        public void OnPanelClose()
        {
            ChangeGameState(previousState);
        }

        /// <summary>
        /// Generating base grid.
        /// </summary>
        private void GenerateGrid()
        {
            round = 0;
            nodesList = new List<Node>();
            afterShiftBlocks = new List<Block>();
            occupiedNodesPos = new List<Vector2>();
            occupiedNodesValue = new List<int>();
            blocksList = new List<Block>();
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                for (int y = 0; y < gridManager.gridHeight; y++)
                {
                    var node = Instantiate(gridManager.nodePrefab, new Vector2(x, y), Quaternion.identity);
                    nodesList.Add(node);
                }
            }
            if (gridManager.gridWidth == 4)
            {
                gridManager.winCondition = 1024;
            }
            else if (gridManager.gridWidth == 5)
            {
                gridManager.winCondition = 2048;
            }
            else
            {
                gridManager.winCondition = 4096;
            }

            var center = new Vector2((float)gridManager.gridWidth / 2 - 0.5f, (float)gridManager.gridHeight / 2 - 0.5f); //We subtract 0.5 is because our nodes are centered on each whole vector.

            var board = Instantiate(gridManager.boardPrefab, center, Quaternion.identity);
            board.size = new Vector2(gridManager.gridWidth, gridManager.gridHeight);

            Camera.main.transform.position = new Vector3(center.x, center.y + 1.5f, -10); // +1.5f to y-axis for better looking
            Camera.main.orthographicSize = gridManager.gridWidth + 1;
        }

        /// <summary>
        /// Spawning blocks generally.
        /// </summary>
        /// <param name="amount"></param>
        private void SpawnBlocks(int amount)
        {
            var freeNodes = nodesList.Where(n => n.occupiedBlock == null).OrderBy(b => Random.value).ToList();
            foreach (var node in freeNodes.Take(amount))
            {
                SpawnBlock(node, Random.value > 0.8f ? 4 : 2);

            }
            if (freeNodes.Count() == 1)
            {
                CheckLoseGame();
                return;
            }
            ChangeGameState(blocksList.Any(b => b.value == gridManager.winCondition) ? GameState.Win : GameState.WaitingInput);
        }

        

        /// <summary>
        /// Spawn blocks specifically for merging. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        private void SpawnBlock(Node node, int value)
        {
            var block = Instantiate(gridManager.blockPrefab, node.Pos, Quaternion.identity);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            blocksList.Add(block);
        }
        /// <summary>
        /// Spawn blocks specifically for undo.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        /// <param name="node"></param>
        void SpawnBlockForUtilization(Vector2 pos,int value,Node node)
        {
            var block = Instantiate(gridManager.blockPrefab, pos, Quaternion.identity);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            blocksList.Add(block);
        }
 
        /// <summary>
        /// Base shifting mechanics.
        /// </summary>
        /// <param name="direction"></param>
        public void Shift(Vector2 direction)
        {
            if (Time.timeScale == 0) return;
            ChangeGameState(GameState.Shifting);
            var occupiedNodes = nodesList.Where(n => n.occupiedBlock != null).ToList();
            occupiedNodesPos.Clear();
            occupiedNodesValue.Clear();
            foreach (var node in occupiedNodes)
            {
                occupiedNodesPos.Add(node.Pos);
                occupiedNodesValue.Add(node.occupiedBlock.value);
            }
            
            soundManager.MoveSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
            var orderedBlocks = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList(); //Before shifting blocks list.
            List<Vector2> orderedBlockPositionsBeforeShift = new List<Vector2>();
            foreach (var block in blocksList)
            {
                orderedBlockPositionsBeforeShift.Add(block.Pos);
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

                sequence.Insert(0, block.transform.DOMove(movePoint, gridManager.travelTime).SetEase(Ease.Flash));
            }
            sequence.Play();
            sequence.OnComplete(() =>
            {
                var mergeBlocks = orderedBlocks.Where(b => b.mergingBlock != null).ToList();
                foreach (var block in mergeBlocks)
                {
                    MergeBlocks(block.mergingBlock, block);
                    scoreManager.HandleScore(block.value);
                    if (block.value > 63) 
                        soundManager.NiceScoreSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                    else
                        soundManager.MergingSound(this.gameObject.GetComponent<AudioSource>() != null ? this.gameObject.GetComponent<AudioSource>() : this.gameObject.AddComponent<AudioSource>());
                }

                //This section is here to prevent spawning block if there is not shift via compare before blocks list and after block list.
                afterShiftBlocks.Clear();
                var orderedBlockAfterShift = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList(); // After shifting block list.
                List<Vector2> orderedBlockPositionsAfterShift = new List<Vector2>();
                foreach (var block in orderedBlockAfterShift)
                {
                    orderedBlockPositionsAfterShift.Add(block.Pos);
                    afterShiftBlocks.Add(block);
                }
                if (!ICompareLists.CompareLists(orderedBlockPositionsBeforeShift,orderedBlockPositionsAfterShift))
                {
                    ChangeGameState(GameState.SpawningBlocks);
                }
                sequence.Kill();
            });
        }

        /// <summary>
        /// Checking after all grid is full if there is still mergeable block.
        /// </summary>
        void CheckLoseGame()
        {
            List<Block> possibleMerges = new List<Block>();

            var orderedBlocks = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            foreach (var block in orderedBlocks)
            {
                var next = block.node;
                block.SetBlock(next);

                var possibleNodeRight = GetNodeAtPosition(next.Pos + Vector2.right);
                var possibleNodeLeft = GetNodeAtPosition(next.Pos + Vector2.left);
                var possibleNodeUp = GetNodeAtPosition(next.Pos + Vector2.up);
                var possibleNodeDown = GetNodeAtPosition(next.Pos + Vector2.down);
                List<Node> possibleNodes = new List<Node>();
                if (possibleNodeRight != null)
                {
                    possibleNodes.Add(possibleNodeRight);
                }
                if (possibleNodeLeft != null)
                {
                    possibleNodes.Add(possibleNodeLeft);
                }
                if (possibleNodeUp != null)
                {
                    possibleNodes.Add(possibleNodeUp);
                }
                if (possibleNodeDown != null)
                {
                    possibleNodes.Add(possibleNodeDown);
                }
                foreach (var possibleNode in possibleNodes)
                {
                    if (possibleNode.occupiedBlock.CanMerge(block.value)) //There is still mergeable blocks on grid.
                    {
                        possibleMerges.Add(block);
                    }
                }

                possibleNodes.Clear();
            }
            if (possibleMerges.Count == 0) // All grid is full and there is not any mergeable block so it is Lose.
            {
                ChangeGameState(GameState.Lose);
            }
            else
            {
                possibleMerges.Clear();
            }
        }

        /// <summary>
        /// Merging blocks that matching.
        /// </summary>
        /// <param name="baseBlock"></param>
        /// <param name="mergingBlock"></param>
        void MergeBlocks(Block baseBlock, Block mergingBlock)
        {
            var newValue = baseBlock.value * 2;

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
            blocksList.Remove(block);
            Destroy(block.gameObject);
        }
        /// <summary>
        /// Getting node at given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Node GetNodeAtPosition(Vector2 pos)
        {
            return nodesList.FirstOrDefault(n => n.Pos == pos);
        }

        IEnumerator UndoFunctionCoroutine()
        {
            var blocksInGrid = blocksList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();  // All the blocks in the grid.
            foreach (var oldBlock in blocksInGrid)
            {
                RemoveBlock(oldBlock);
            }
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < occupiedNodesPos.Count; i++)
            {
                SpawnBlockForUtilization(occupiedNodesPos[i], occupiedNodesValue[i], GetNodeAtPosition(occupiedNodesPos[i]));
            }

            scoreManager.currentScore = scoreManager.previousScore;
            scoreManager.CallScoreChangeEvent();
            

            yield return null; 
        }
        public void UndoFunction()
        {
            if (round == 1) return;
            StartCoroutine(UndoFunctionCoroutine());
        }

        public void MuteUnMute()
        {
            var mAudioSource = gameObject.GetComponent<AudioSource>() != null ? 
                gameObject.GetComponent<AudioSource>() : gameObject.AddComponent<AudioSource>();
            mAudioSource.volume = Mathf.Approximately(0f, mAudioSource.volume) ? 0.3f : 0f;
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
        OnPause
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



