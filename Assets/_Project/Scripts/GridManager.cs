using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public class GridManager
    {
        private GridCell[,] _gridCells;
        private int _gridSize;
        private float _cellSize;
        private float _cellSizeOffset;

        public void Initialize(int size, float offset, float initializeDuration, float cellSingleScaleUpTime,
            GridCell cellPrefab, Transform parent)
        {
            _gridSize = size;
            _cellSizeOffset = offset;
            _cellSize = CalculateCellSize();
            _gridCells = new GridCell[size, size];

            const float singleScaleDuration = 0.1f;
            var maxDelay = initializeDuration - singleScaleDuration;
            var maxIndex = (_gridSize - 1) * 2f;

            DOTween.SetTweensCapacity(750, 250);

            for (var y = 0; y < _gridSize; y++)
            {
                for (var x = 0; x < _gridSize; x++)
                {
                    var delay = ((x + y) / maxIndex) * maxDelay;
                    var cell = Object.Instantiate(cellPrefab, parent);
                    cell.transform.localPosition = new Vector3(x * _cellSize, y * _cellSize, 0);
                    cell.transform.localScale = Vector3.zero;
                    cell.transform.DOScale(Vector3.one * _cellSize * _cellSizeOffset, cellSingleScaleUpTime)
                        .SetDelay(delay).SetEase(Ease.OutBack);

                    cell.Initialize(x, y);
                    _gridCells[x, y] = cell;
                }
            }

            CenterGridInView();
        }

        public void ResizeGrid(int newSize, float offset, float initializeDuration, float scaleUpTime,
            GridCell cellPrefab, Transform parent)
        {
            var newCellSize = CalculateCellSize();
            var newCells = new GridCell[newSize, newSize];

            const float singleScaleDuration = 0.1f;
            var maxDelay = initializeDuration - singleScaleDuration;
            var maxIndex = (newSize - 1) * 2f;

            for (var y = 0; y < newSize; y++)
            {
                for (var x = 0; x < newSize; x++)
                {
                    var delay = ((x + y) / maxIndex) * maxDelay;

                    if (x < _gridSize && y < _gridSize && _gridCells[x, y])
                    {
                        var exist = _gridCells[x, y];
                        exist.transform.SetParent(parent);
                        exist.transform.localPosition = new Vector3(x * newCellSize, y * newCellSize, 0);
                        exist.transform.DOScale(Vector3.one * (newCellSize * offset), scaleUpTime)
                            .SetDelay(delay).SetEase(Ease.OutBack);
                        newCells[x, y] = exist;
                    }
                    else
                    {
                        var cell = Object.Instantiate(cellPrefab, parent);
                        cell.transform.localPosition = new Vector3(x * newCellSize, y * newCellSize, 0);
                        cell.transform.localScale = Vector3.zero;
                        cell.transform.DOScale(Vector3.one * (newCellSize * offset), scaleUpTime)
                            .SetDelay(delay).SetEase(Ease.OutBack);

                        cell.Initialize(x, y);
                        newCells[x, y] = cell;
                    }
                }
            }

            for (var y = 0; y < _gridSize; y++)
            {
                for (var x = 0; x < _gridSize; x++)
                {
                    if (x >= newSize || y >= newSize)
                    {
                        Object.Destroy(_gridCells[x, y].gameObject);
                    }
                }
            }

            _gridCells = newCells;
            _gridSize = newSize;
            _cellSize = newCellSize;
            _cellSizeOffset = offset;

            CenterGridInView(true);
        }

        public void OnCellClicked(GridCell cell)
        {
            if (!cell.HasX)
            {
                cell.PlaceX();
                CheckMatches(cell.X, cell.Y);
            }
            else
            {
                cell.Shake();
            }
        }

        private void CheckMatches(int x, int y)
        {
            var toClear = new HashSet<GridCell>();

            var horizontal = GetMatchLine(x, y, Vector2Int.left, Vector2Int.right);
            var vertical = GetMatchLine(x, y, Vector2Int.down, Vector2Int.up);
            var diagonal1 = GetMatchLine(x, y, new Vector2Int(-1, -1), new Vector2Int(1, 1));
            var diagonal2 = GetMatchLine(x, y, new Vector2Int(-1, 1), new Vector2Int(1, -1));

            if (horizontal.Count >= 3) foreach (var c in horizontal) toClear.Add(c);
            if (vertical.Count >= 3) foreach (var c in vertical) toClear.Add(c);
            if (diagonal1.Count >= 3) foreach (var c in diagonal1) toClear.Add(c);
            if (diagonal2.Count >= 3) foreach (var c in diagonal2) toClear.Add(c);
            var lMatches = GetLMatchesAround(x, y); 
            if (lMatches.Count >= 3) foreach (var c in lMatches) toClear.Add(c);

            foreach (var c in toClear) c.ClearX();
        }


        private List<GridCell> GetMatchLine(int x, int y, Vector2Int dir1, Vector2Int dir2)
        {
            var line = GetLine(x, y, dir1)
                .Concat(new[] { _gridCells[x, y] })
                .Concat(GetLine(x, y, dir2))
                .ToList();

            return line.Count >= 3 ? line : new List<GridCell>();
        }

        private IEnumerable<GridCell> GetLine(int x, int y, Vector2Int dir)
        {
            x += dir.x;
            y += dir.y;

            while (IsValidX(x, y))
            {
                yield return _gridCells[x, y];
                x += dir.x;
                y += dir.y;
            }
        }

        private List<GridCell> GetLMatchesAround(int x, int y)
        {
            var result = new HashSet<GridCell>();
            var positions = new Vector2Int[]
            {
                new(x, y),
                new(x - 1, y),
                new(x + 1, y),
                new(x, y - 1),
                new(x, y + 1)
            };

            foreach (var pos in positions)
            {
                foreach (var (offsetA, offsetB) in new[]
                         {
                             (new Vector2Int(0, 1), new Vector2Int(1, 0)), // up + right
                             (new Vector2Int(0, 1), new Vector2Int(-1, 0)), // up + left
                             (new Vector2Int(0, -1), new Vector2Int(1, 0)), // down + right
                             (new Vector2Int(0, -1), new Vector2Int(-1, 0)), // down + left
                         })
                {
                    var a = pos + offsetA;
                    var b = pos + offsetB;

                    if (!IsValidX(pos) || !IsValidX(a) || !IsValidX(b)) continue;
                    result.Add(_gridCells[pos.x, pos.y]);
                    result.Add(_gridCells[a.x, a.y]);
                    result.Add(_gridCells[b.x, b.y]);
                }
            }

            return result.ToList();
        }

        private bool IsValidX(Vector2Int pos) => IsValidX(pos.x, pos.y);

        private bool IsValidX(int x, int y) => x >= 0 && x < _gridSize && y >= 0 && y < _gridSize && _gridCells[x, y] && _gridCells[x, y].HasX;

        private float CalculateCellSize()
        {
            var camera = Camera.main;
            if (!camera) return 0f;
            var screenHeight = camera.orthographicSize * 2f;
            var screenWidth = screenHeight * camera.aspect;
            return Mathf.Min(screenWidth, screenHeight) / _gridSize;
        }

        private void CenterGridInView(bool lerp = false)
        {
            var camera = Camera.main;
            if (!camera) return;

            var center = new Vector3((_gridSize - 1) * _cellSize / 2f, (_gridSize - 1) * _cellSize / 2f, -10f);
            var screenRatio = (float)Screen.width / Screen.height;
            var targetSize = (_gridSize * _cellSize) / 2f;
            if (screenRatio < 1f)
                targetSize /= screenRatio;

            if (lerp)
            {
                camera.transform.DOMove(center, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.1f);
                camera.DOOrthoSize(targetSize, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.1f);
            }
            else
            {
                camera.transform.position = center;
                camera.orthographicSize = targetSize;
            }
        }
    }
}