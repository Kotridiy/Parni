using Assets.Scripts.Core;
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
            GameUnit unit = point.GetUnit();
            if (point.GamePlace != null)
            {
                SystemInfoText.ChangeInfoText(point.GamePlace.Info);
                captured = point.GamePlace;
                memoryDrawer.DrawMemory(point.GamePlace.Memory);
            }
            else if (unit != null)
            {
                SystemInfoText.ChangeInfoText(unit.Info);
                captured = unit;
                memoryDrawer.DrawMemory(unit.Memory);
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
                border.transform.SetParent(captured.transform);
                border.transform.position = new Vector3(captured.transform.position.x, captured.transform.position.y, captured.transform.position.z - 1);
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
            (captured as GameUnit).Move(GetClosestGraphPoint());
        }

        private GraphPoint GetClosestGraphPoint()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return HexGraph.Graph.FindClosestPoint(mousePos.x, mousePos.y);
        }
    }
}