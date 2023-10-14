using Assets.Scripts.Core;
using Assets.Scripts.Entity;
using UnityEngine;


namespace Assets.Scripts
{
    public class GridDrawer : MonoBehaviour
    {
        [SerializeField] GameObject roadPrefab;

        [Header("Size")]
        [SerializeField] int gridSizeX = 10; // Размер сетки по горизонтали
        [SerializeField] int gridSizeY = 10; // Размер сетки по вертикали
        [SerializeField] float cellSize = 1f; // Размер одной ячейки сетки

        [Header("Sprites")]
        [SerializeField] Sprite footpathSprite;
        [SerializeField] Sprite walkwaySprite;
        [SerializeField] Sprite roadSprite;
        [SerializeField] Sprite highwaySprite;

        [Header("Hierarchy")]
        [SerializeField] GameObject roadBase;
        [SerializeField] GameObject spawnBase;

        private HexGraph graph;
        private Vector3 prefabScale;

        public HexGraph Graph { get => graph; }
        public float CellSize { get => cellSize; }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            graph = new HexGraph(gridSizeX, gridSizeY, cellSize);
            Gizmos.color = Color.magenta;
            DrawGraph(graph, transform.position, true);
        }

        private void Awake()
        {
            graph = new HexGraph(gridSizeX, gridSizeY, cellSize);
        }

        private void Start()
        {
            DrawGraph(graph, transform.position, false);
            StartSpawn();
        }

        private void DrawGraph(HexGraph graph, Vector2 offset, bool isGizmos)
        {
            GraphEdge edge;

            for (int i = 0; i <= graph.SizeN; i++)
            {
                for (int j = 0; j <= graph.SizeM; j++)
                {
                    edge = graph.GetGraphEdge(i, j, i + 1, j);
                    if (edge != null)
                    {
                        DrawEdge(edge, offset, isGizmos);
                    }

                    edge = graph.GetGraphEdge(i, j, i, j + 1);
                    if (edge != null)
                    {
                        DrawEdge(edge, offset, isGizmos);
                    }
                }
            }
        }

        private void DrawEdge(GraphEdge edge, Vector2 offset, bool isGizmos)
        {
            var startPos = new Vector3(edge.First.PosX + offset.x, edge.First.PosY + offset.y, 0);
            var endPos = new Vector3(edge.Second.PosX + offset.x, edge.Second.PosY + offset.y, 0);
            Sprite sprite;

            if (isGizmos)
            {
                Gizmos.DrawLine(startPos, endPos);
                return;
            }

            if (roadPrefab == null || startPos == null || endPos == null)
            {
                return;
            }
            if (prefabScale == Vector3.zero)
            {
                prefabScale = new Vector3(GraphPoint.R * 2 * cellSize, roadPrefab.transform.localScale.y * cellSize, roadPrefab.transform.localScale.z);
            }

            switch (edge.Level)
            {
                case EdgeLevel.None:
                    return;
                case EdgeLevel.Foorpath:
                    sprite = footpathSprite;
                    break;
                case EdgeLevel.Walkway:
                    sprite = walkwaySprite;
                    break;
                case EdgeLevel.Road:
                    sprite = roadSprite;
                    break;
                case EdgeLevel.Highway:
                    sprite = highwaySprite;
                    break;
                default:
                    return;
            }

            GameObject road = Instantiate(roadPrefab, roadBase.transform);
            Vector3 direction = (endPos - startPos).normalized;
            road.transform.position = startPos + direction * cellSize * GraphPoint.R;
            road.transform.localPosition = new Vector3(road.transform.localPosition.x, road.transform.localPosition.y, 0);
            road.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            road.transform.localScale = prefabScale;
            road.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        private void StartSpawn()
        {
            var spawners = Resources.LoadAll<Spawner>("Spawners");
            foreach (Spawner spawner in spawners)
            {
                var newEntity = Instantiate(spawner.entity, spawnBase.transform);
                var team = spawner.entity is Dimn ? TeamManager.GetOrCreate("Red", Race.Dimn) : TeamManager.GetOrCreate("Green", Race.Pepl);
                newEntity.Initialization(graph.GetGraphPoint(spawner.spawnPosition.x, spawner.spawnPosition.y), team);
            }
        }
    }
}