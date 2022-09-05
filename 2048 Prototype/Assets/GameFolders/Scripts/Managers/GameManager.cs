using DG.Tweening;
using Prototype.Scripts.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        //Witdh of gird.
        [SerializeField] int width = 4;

        //Height of grid.
        [SerializeField] int height = 4;

        //Node prefab. Will select manually on unity inspector.
        [SerializeField] Node nodePrefab;

        //Block prefab. Will select manually on unity inspector.
        [SerializeField] Block blockPrefab;

        //Board prefabs's spriteRenderer for instantiate.
        [SerializeField] SpriteRenderer boardPrefab;

        //The values that blocks holds.
        [SerializeField] List<BlockType> types;

        //Shifting anim time.
        [SerializeField] float travelTime = 0.2f;

        //The needed value to win.
        [SerializeField] int winCondition = 2048;

        //To instantiate merged block.
        [SerializeField] GameObject mergeEffectPrefab;

        ////To animate merged blocks value.
        ////[SerializeField] FloatingText _floatingTextPrefab;


        ////Will adjust later.........!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //[SerializeField] GameObject _winScreen, _loseScreen, _winScreenText;
        //[SerializeField] AudioClip[] _moveClips;
        //[SerializeField] AudioClip[] _matchClips;
        //[SerializeField] AudioSource _source;


        //List for nodes and blocks.
        List<Node> nodes;
        List<Block> blocks;

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
                    break;
                case GameState.Lose:
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
            blocks = new List<Block>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                    nodes.Add(node);
                }
            }

            var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f); //We subtract 0.5 is because our nodes are centered on each whole vector.

            var board = Instantiate(boardPrefab, center, Quaternion.identity);
            board.size = new Vector2(width, height);

            Camera.main.transform.position = new Vector3(center.x, center.y, -10);

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

            ChangeGameState(blocks.Any(b => b.value == winCondition) ? GameState.Win : GameState.WaitingInput);
        }
        void SpawnBlock(Node node, int value)
        {
            var block = Instantiate(blockPrefab, node.Pos, Quaternion.identity);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            blocks.Add(block);
        }

        /// <summary>
        /// Base shifting mechanics.
        /// </summary>
        /// <param name="dir"></param>
        void Shift(Vector2 dir)
        {
            ChangeGameState(GameState.Shifting);

            var orderedBlocks = blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
            if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

            foreach (var block in orderedBlocks)
            {
                var next = block.node;
                do
                {
                    block.SetBlock(next);

                    var possibleNode = GetNodeAtPosition(next.Pos + dir);
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

                sequence.Insert(0, block.transform.DOMove(movePoint, travelTime).SetEase(Ease.InQuad));
            }

            sequence.OnComplete(() =>
            {
                var mergeBlocks = orderedBlocks.Where(b => b.mergingBlock != null).ToList();
                foreach (var block in mergeBlocks)
                {
                    MergeBlocks(block.mergingBlock, block);
                }
                //if (mergeBlocks.Any()) _source.PlayOneShot(_matchClips[Random.Range(0, _matchClips.Length)], 0.2f);
                ChangeGameState(GameState.SpawningBlocks);
            });


            //_source.PlayOneShot(_moveClips[Random.Range(0, _moveClips.Length)], 0.2f);
        }

        /// <summary>
        /// Merging blocks that matching.
        /// </summary>
        /// <param name="baseBlock"></param>
        /// <param name="mergingBlock"></param>
        void MergeBlocks(Block baseBlock, Block mergingBlock)
        {
            var newValue = baseBlock.value * 2;

            Instantiate(mergeEffectPrefab, baseBlock.Pos, Quaternion.identity);
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



