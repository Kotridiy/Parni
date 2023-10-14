using Assets.Scripts.Entity;
using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityRand = UnityEngine.Random;

namespace Assets.Scripts.Core
{
    public class UnitSpriteController : MonoBehaviour
    {
        const float BASE_OFFSET_X = 1f;
        const float BASE_OFFSET_Y = 1.5f;

        [SerializeField] GameUnit unit;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer dotRenderer;
        public SpriteRenderer SpriteRenderer { get => spriteRenderer; }
        public SpriteRenderer DotRenderer { get => dotRenderer; }

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (unit == null) throw new Exception("No attached entity");
                if (spriteRenderer == null) throw new Exception("No attached sprite");

                StartCoroutine(DotUpdate());
            }
        }

        private void Update()
        {
            SpritesUpdate();
        }

        private IEnumerator DotUpdate()
        {
            while (true)
            {
                if (unit.OnRoad != null || !(unit.Point.GamePlace is Castle))
                {
                    yield return StartCoroutine(DotMoving(Vector3.zero, 0.2f));
                }
                else
                {
                    float maxAmplitude = math.sqrt(unit.Point.GetUnits().Count() / 10f);
                    var nextPosition = new Vector3(UnityRand.Range(-maxAmplitude, maxAmplitude), UnityRand.Range(-maxAmplitude, maxAmplitude), 0);
                    float time = 1;
                    yield return StartCoroutine(DotMoving(nextPosition, time));
                    yield return new WaitForSeconds(time / 2);
                }
            }
        }

        private IEnumerator DotMoving(Vector3 destination, float time)
        {
            float timeLeft = 0;
            var start = dotRenderer.transform.localPosition;

            while (timeLeft < time)
            {
                dotRenderer.transform.localPosition = Vector3.Lerp(start, destination, timeLeft / time);
                timeLeft += Time.deltaTime;
                yield return null;
            }
        }

        private void SpritesUpdate()
        {
            spriteRenderer.enabled = unit.OnRoad != null || !(unit.Point.GamePlace is Castle);
            if (unit.OnRoad == null)
            {
                if (unit.Point.GamePlace is Castle)
                {
                    dotRenderer.color = new Color(dotRenderer.color.r, dotRenderer.color.g, dotRenderer.color.b, 1);
                }
                else
                {
                    var units = unit.Point.GetUnits().Where(u => u.OnRoad == null).ToList();
                    transform.localPosition = GetAlignedPositionOffset(units.Count, units.IndexOf(unit));
                    dotRenderer.color = new Color(dotRenderer.color.r, dotRenderer.color.g, dotRenderer.color.b, 0);
                }
            }
            else
            {
                var closerPoint = Vector2.Distance(unit.OnRoad.First.Pos, (Vector2)unit.transform.position) < 
                    Vector2.Distance(unit.OnRoad.Second.Pos, (Vector2)unit.transform.position) ?
                    unit.OnRoad.First : unit.OnRoad.Second;
                if (closerPoint.GamePlace is Castle)
                {
                    var distanceRatio = math.min(Vector2.Distance(closerPoint.Pos, (Vector2)unit.transform.position) / HexGraph.Graph.CellSize * 2, 1);
                    dotRenderer.color = new Color(dotRenderer.color.r, dotRenderer.color.g, dotRenderer.color.b, 1 - distanceRatio);
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, distanceRatio);
                }
                else
                {
                    transform.localPosition = GetAlignedPositionOffset(1, 0);
                    dotRenderer.color = new Color(dotRenderer.color.r, dotRenderer.color.g, dotRenderer.color.b, 0);
                }
            }
        }

        public static Vector3 GetAlignedPositionOffset(int count, int index)
        {
            if (count < 1 || index < 0 || count < index) throw new ArgumentException($"Incorrect {nameof(count)}({count}) or {nameof(index)}({index})");

            if (count == 1) return new Vector2(0, 0);

            if (count == 2) return new Vector2((index - 0.5f) * BASE_OFFSET_X, 0);

            int height = (count <= 6) ? 2 : 3;

            int y = index * height / count;
            int width = (count - y + (height - 1)) / height;
            int x = index % width;
            return new Vector3((x - (width - 1) * 0.5f) * BASE_OFFSET_X, (-y + (height - 1) * 0.5f) * BASE_OFFSET_Y, -y);
        }
    }
}