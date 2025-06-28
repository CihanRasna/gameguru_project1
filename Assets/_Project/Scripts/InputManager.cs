using UnityEngine;
using Zenject;

namespace _Project.Scripts
{
    public class InputManager : ITickable
    {
        private Camera _camera;
        private GridManager _gridManager;

        [Inject]
        public void Construct(Camera camera, GridManager gridManager)
        {
            _camera = camera;
            _gridManager = gridManager;
        }

        public void Tick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            Vector2 screenPos = Input.mousePosition;

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
                screenPos = Input.GetTouch(0).position;
#endif

            var ray = _camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit) && hit.collider.TryGetComponent(out GridCell cell))
            {
                _gridManager.OnCellClicked(cell);
            }
        }
    }
}