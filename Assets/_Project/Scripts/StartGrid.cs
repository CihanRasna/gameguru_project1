using UnityEngine;
using Zenject;

namespace _Project.Scripts
{
    public class StartGrid : MonoBehaviour
    {
        [Inject] private GridManager _gridManager;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private int gridSize = 6;

        private void Start()
        {
            _gridManager.Initialize(gridSize, cellPrefab, gridParent);
        }
    }
}