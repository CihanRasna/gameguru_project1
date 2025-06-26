namespace _Project.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GridManager
    {
        private GridCell[,] _gridCells;
        private int _gridSize;
        private float _cellSize;
        private GameObject _cellPrefab;
        private Transform _parent;

        public void Initialize(int size, GameObject cellPrefab, Transform parent)
        {
            _gridSize = size;
            _cellPrefab = cellPrefab;
            _parent = parent;
            _gridCells = new GridCell[size, size];
            _cellSize = CalculateCellSize();

            for (var y = 0; y < _gridSize; y++)
            {
                for (var x = 0; x < _gridSize; x++)
                {
                    var cellGo = Object.Instantiate(cellPrefab, parent);
                    cellGo.transform.localPosition = new Vector3(x * _cellSize, y * _cellSize, 0);
                    cellGo.transform.localScale = Vector3.one * _cellSize;

                    var cell = cellGo.GetComponent<GridCell>();
                    cell.Initialize(x, y, this);
                    _gridCells[x, y] = cell;
                }
            }

            CenterGridInView();
        }

        public void OnCellClicked(int x, int y)
        {
            var cell = _gridCells[x, y];
            if (!cell.HasX)
            {
                cell.PlaceX();
                CheckMatches(x, y);
            }
        }

        private void CheckMatches(int x, int y)
        {
            var horizontal = GetLine(x, y, Vector2Int.left)
                .Concat(new[] { _gridCells[x, y] })
                .Concat(GetLine(x, y, Vector2Int.right))
                .ToList();

            var vertical = GetLine(x, y, Vector2Int.down)
                .Concat(new[] { _gridCells[x, y] })
                .Concat(GetLine(x, y, Vector2Int.up))
                .ToList();

            var diagonal1 = GetLine(x, y, new Vector2Int(-1, -1))
                .Concat(new[] { _gridCells[x, y] })
                .Concat(GetLine(x, y, new Vector2Int(1, 1)))
                .ToList();

            var diagonal2 = GetLine(x, y, new Vector2Int(-1, 1))
                .Concat(new[] { _gridCells[x, y] })
                .Concat(GetLine(x, y, new Vector2Int(1, -1)))
                .ToList();

            if (horizontal.Count >= 3)
                horizontal.ForEach(c => c.ClearX());

            if (vertical.Count >= 3)
                vertical.ForEach(c => c.ClearX());

            if (diagonal1.Count >= 3)
                diagonal1.ForEach(c => c.ClearX());

            if (diagonal2.Count >= 3)
                diagonal2.ForEach(c => c.ClearX());
        }


        private IEnumerable<GridCell> GetLine(int x, int y, Vector2Int dir)
        {
            x += dir.x;
            y += dir.y;

            while (x >= 0 && x < _gridSize && y >= 0 && y < _gridSize && _gridCells[x, y].HasX)
            {
                yield return _gridCells[x, y];
                x += dir.x;
                y += dir.y;
            }
        }

        private float CalculateCellSize()
        {
            if (Camera.main == null) return 0f;
            var screenHeight = Camera.main.orthographicSize * 2f;
            var screenWidth = screenHeight * Camera.main.aspect;
            return Mathf.Min(screenWidth, screenHeight) / _gridSize;
        }

        private void CenterGridInView()
        {
            var center = new Vector3((_gridSize - 1) * _cellSize / 2f, (_gridSize - 1) * _cellSize / 2f, -10f);

            if (Camera.main == null) return;
            Camera.main.transform.position = center;

            var screenRatio = (float)Screen.width / Screen.height;
            var targetSize = (_gridSize * _cellSize) / 2f;

            if (screenRatio >= 1f)
            {
                Camera.main.orthographicSize = targetSize;
            }
            else
            {
                Camera.main.orthographicSize = targetSize / screenRatio;
            }
        }
    }
}