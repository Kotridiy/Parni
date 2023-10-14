using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using Assets.Scripts.Debug;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class MouseController : MonoBehaviour
    {
        [SerializeField] float ClickTimer = 0.2f;
        [SerializeField] GameUnit UnitPrefab;
        [SerializeField] GameObject BorderPrefab;
        [SerializeField] Transform SpawnTranform;

        private float lastLeftClickTime;
        private float lastRightClickTime;
        private GameEntity captured;
        private GameObject border;

        private MemoryDrawer memoryDrawer;

        private static Vector2 MousePos { get => Camera.main.ScreenToWorldPoint(Input.mousePosition); }

        private void OnEnable()
        {
            memoryDrawer = FindObjectOfType<MemoryDrawer>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastLeftClickTime = Time.realtimeSinceStartup;
            }
            if (Input.GetMouseButtonUp(0) && Time.realtimeSinceStartup - lastLeftClickTime < ClickTimer)
            {
                LeftClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                lastRightClickTime = Time.realtimeSinceStartup;
            }
            if (Input.GetMouseButtonUp(1) && Time.realtimeSinceStartup - lastRightClickTime < ClickTimer)
            {
                RightClick();
            }

            if (captured != null)
            {
                SystemInfoText.ChangeInfoText(captured.Info);
            } else
            {
                SystemInfoText.ChangeInfoText("");
            }
        }

        private void LeftClick()
        {
            GraphPoint point = GetClosestGraphPoint();
            Capture(point);
        }

        private void RightClick()
        {
            if (captured == null)
            {
                CreateUnit();
            }
            else
            {
                if (captured is GameUnit)
                {
                    MoveUnit();
                }
            }
        }

        private void Capture(GraphPoint point)
        {
            GameEntity entity = GetNextCaptureEntity(point);
            if (entity != null)
            {
                SystemInfoText.ChangeInfoText(entity.Info);
                captured = entity;
                memoryDrawer.StartDraw(entity.Memory);
            }
            else 
            { 
                captured = null;
                memoryDrawer.StopDraw();
            }

            if (captured)
            {
                if (!border)
                {
                    border = Instantiate(BorderPrefab);
                }
                border.transform.SetParent(captured is GameUnit unit ? unit.SpriteController.transform : captured.transform);
                border.transform.position = new Vector3(border.transform.parent.position.x, border.transform.parent.position.y, border.transform.parent.position.z - 1);
            }
            else
            {
                if (border)
                {
                    Destroy(border);
                }
            }
        }

        private void CreateUnit()
        {
            if (UnitPrefab != null)
            {
                GraphPoint point = GetClosestGraphPoint();
                var unit = Instantiate(UnitPrefab, SpawnTranform);
                unit.Initialization(point, TeamManager.GetOrCreate("Green", Race.Pepl));
            }
        }

        private void MoveUnit()
        {
            var ans = (captured as GameUnit).Brain.TrySayCommand(new BrainTask(BrainTaskType.Movement, GetClosestGraphPoint(), null),
                (o, e) => UnityEngine.Debug.Log("End command"));
            if (!ans)
            {
                UnityEngine.Debug.Log("Command failed");
            }
        }

        private GraphPoint GetClosestGraphPoint()
        {
            var mousePos = MousePos;
            return HexGraph.Graph.FindClosestPoint(mousePos.x, mousePos.y);
        }

        private GameUnit GetClosestUnit(IEnumerable<GameUnit> units)
        {
            if (units == null || !units.Any()) return null;
            var mousePos = MousePos;
            return units.Aggregate((closest, next) =>
                Vector2.Distance(mousePos, closest.Point.Pos) < Vector2.Distance(mousePos, next.Point.Pos) ? closest : next);
        }

        // 1. Ближайший к курсору
        // 2. Если здание было выбрано, то первый юнит
        // 3. Если юнит был выбран, то следующий юнит
        // 4. Если выбранный юнит - последний, то снова здание
        // 5. Если в прошлом условии здания нет, то снова первый юнит
        // 6. Если ничего не выбрано, то здание
        // 7. Если ничего не выбрано и здания нет, то первый юнит
        // 8. Если прошлый выбор повторяется с единственным зданием или единственным юнитом, то ничего
        // 9. Если в точке ничего нет, то ничего 
        private GameEntity GetNextCaptureEntity(GraphPoint point)
        {
            GameEntity entity = null;
            if (point.GamePlace == null && !point.GetUnits().Any()) return null;

            var mousePos = MousePos;
            var unitsOnRoad = point.GetUnits().Where(u => u.OnRoad != null);
            entity = GetClosestUnit(unitsOnRoad);
            if (entity != null && (!point.GetUnits().Where(u => u.OnRoad == null).Any() || 
                Vector2.Distance(mousePos, (Vector2)entity.transform.position) < Vector2.Distance(mousePos, point.Pos)))
            {
                return captured != entity ? entity : null;
            }

            if (captured != null)
            {
                if (captured is GamePlace)
                {
                    return captured != point.GamePlace ? (GameEntity)point.GamePlace : (point.GetUnits().Any() ? point.GetUnits().First() : null);
                }
                else
                {
                    var unitsOnPoint = point.GetUnits().Where(u => u.OnRoad == null).ToList() ?? new List<GameUnit>();
                    var index = unitsOnPoint.IndexOf(captured as GameUnit);
                    if (index == unitsOnPoint.Count - 1)
                    {
                        return point.GamePlace != null ? (GameEntity)point.GamePlace : (unitsOnPoint.Count == 1 ? null : unitsOnPoint.First());
                    }
                    else
                    {
                        return unitsOnPoint[index + 1];
                    }
                }
            }
            else
            {
                return point.GamePlace != null ? (GameEntity)point.GamePlace : point.GetUnits().Where(u => u.OnRoad == null).First();
            }
        }
    }
}