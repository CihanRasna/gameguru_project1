using UnityEngine;
using Zenject;

namespace _Project.Scripts
{
    public class StartGrid : MonoBehaviour
    {
        [Inject] private GridManager _gridManager;
        [SerializeField] private GridCell cellPrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private int gridSize = 6;
        [SerializeField] private float cellSizeOffset = 1f;
        [SerializeField] private float cellSingleScaleUpTime = 0.1f;
        [SerializeField] private float initializeDuration = 5f;

        private void Start()
        {
            _gridManager.Initialize(gridSize, cellSizeOffset, initializeDuration, cellSingleScaleUpTime, cellPrefab,
                gridParent);
        }


        [ContextMenu("Resize Grid")]
        public void ResizeGridFromEditor(int newGridSize)
        {
            if (cellPrefab == null || gridParent == null)
            {
                Debug.LogWarning("cellPrefab or gridParent is null");
                return;
            }

            _gridManager.ResizeGrid(newGridSize, cellSizeOffset, initializeDuration, cellSingleScaleUpTime, cellPrefab,
                gridParent);
        }
    }
}