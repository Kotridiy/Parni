using UnityEngine;

namespace Assets.Scripts
{
    //Сгенерировано
    public class CameraController : MonoBehaviour
    {
        public float panSpeed = 10f; // Скорость перемещения камеры
        public float zoomSpeed = 5f; // Скорость масштабирования камеры
        public float zoomMin = 10f; // Порог увелечения

        private bool isPanning = false;
        private Vector3 lastMousePosition;
        private float zoomMax;
        private Rect cameraBorderRect;

        private Camera _camera;

        private void OnDrawGizmos()
        {
            _camera = GetComponent<Camera>();

            var rect = GetCameraViewRect();

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(rect.min, new Vector3(rect.min.x, rect.max.y, 0));
            Gizmos.DrawLine(new Vector3(rect.min.x, rect.max.y, 0), rect.max);
            Gizmos.DrawLine(rect.max, new Vector3(rect.max.x, rect.min.y, 0));
            Gizmos.DrawLine(new Vector3(rect.max.x, rect.min.y, 0), rect.min);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rect.center, 1f);
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            zoomMax = _camera.orthographicSize;
            cameraBorderRect = GetCameraViewRect();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPanning = true;
                lastMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isPanning = false;
            }

            if (isPanning)
            {
                Vector3 mouseDelta = lastMousePosition - _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.Translate(mouseDelta);

                var rect = GetCameraViewRect();

                Vector3 borderClamp = new Vector3(
                    -(rect.min.x < cameraBorderRect.min.x ? rect.min.x - cameraBorderRect.min.x : (rect.max.x > cameraBorderRect.max.x ? rect.max.x - cameraBorderRect.max.x : 0)),
                    -(rect.min.y < cameraBorderRect.min.y ? rect.min.y - cameraBorderRect.min.y : (rect.max.y > cameraBorderRect.max.y ? rect.max.y - cameraBorderRect.max.y : 0))
                );
                transform.Translate(borderClamp);
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            float zoomAmount = scrollDelta * zoomSpeed;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - zoomAmount, zoomMin, zoomMax);

            lastMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private Rect GetCameraViewRect()
        {
            Vector3 bottomLeft = _camera.ScreenToWorldPoint(new Vector3(0f, 0f, 0));
            Vector3 topRight = _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight, 0));
            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }
    }
}