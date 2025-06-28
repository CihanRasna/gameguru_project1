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
            _cellSize = CalculateCellSize(size);
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

                    var targetScale = Vector3.one * _cellSize * _cellSizeOffset;
                    cell.transform.DOScale(targetScale, cellSingleScaleUpTime)
                        .SetDelay(delay).SetEase(Ease.OutBack);

                    cell.Initialize(x, y);
                    _gridCells[x, y] = cell;
                }
            }

            CenterGridInView();
        }

        public void OnCellClicked(GridCell cell)
        {
            if (!cell.HasX)
            {
                cell.PlaceX();
                CheckMatches(cell);
            }
            else
            {
                cell.Shake();
            }
        }

        private void CheckMatches(GridCell cell)
        {
            var x = cell.X;
            var y = cell.Y;
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

            if (horizontal.Count >= 3) horizontal.ForEach(c => c.ClearX());
            if (vertical.Count >= 3) vertical.ForEach(c => c.ClearX());
            if (diagonal1.Count >= 3) diagonal1.ForEach(c => c.ClearX());
            if (diagonal2.Count >= 3) diagonal2.ForEach(c => c.ClearX());
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

        public void ResizeGrid(int newSize, float cellOffset, float initializeDuration,
            float cellSingleScaleUpTime, GridCell cellPrefab, Transform parent)
        {
            var newCellSize = CalculateCellSize(newSize);
            var newCells = new GridCell[newSize, newSize];

            const float singleScaleDuration = 0.1f;
            var maxDelay = initializeDuration - singleScaleDuration;
            var maxIndex = (newSize - 1) * 2f;

            for (var y = 0; y < newSize; y++)
            {
                for (var x = 0; x < newSize; x++)
                {
                    var delay = ((x + y) / maxIndex) * maxDelay;

                    if (x < _gridSize && y < _gridSize && _gridCells[x, y] != null)
                    {
                        var cell = _gridCells[x, y];
                        cell.transform.SetParent(parent);
                        cell.transform.localPosition = new Vector3(x * newCellSize, y * newCellSize, 0);

                        var targetScale = Vector3.one * newCellSize * cellOffset;
                        cell.transform.localScale = targetScale;
                        cell.transform.DOScale(targetScale, cellSingleScaleUpTime)
                            .SetDelay(delay).SetEase(Ease.OutBack);

                        newCells[x, y] = cell;
                    }
                    else
                    {
                        var cell = Object.Instantiate(cellPrefab, parent);
                        cell.transform.localPosition = new Vector3(x * newCellSize, y * newCellSize, 0);
                        cell.transform.localScale = Vector3.zero;

                        var targetScale = Vector3.one * newCellSize * cellOffset;
                        cell.transform.DOScale(targetScale, cellSingleScaleUpTime)
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
                        Object.Destroy(_gridCells[x, y].gameObject);
                }
            }

            _gridSize = newSize;
            _gridCells = newCells;
            _cellSize = newCellSize;
            _cellSizeOffset = cellOffset;

            CenterGridInView(true);
        }

        private void CenterGridInView(bool lerp = false)
        {
            if (Camera.main == null) return;

            var center = new Vector3((_gridSize * _cellSize) / 2f - (_cellSize / 2f), (_gridSize * _cellSize) / 2f - (_cellSize / 2f), -10f);

            var screenRatio = (float)Screen.width / Screen.height;
            var targetSize = (_gridSize * _cellSize) / 2f;

            if (screenRatio < 1f) targetSize /= screenRatio;

            if (lerp)
            {
                Camera.main.transform.DOMove(center, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.25f);
                Camera.main.DOOrthoSize(targetSize, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.25f);
            }
            else
            {
                Camera.main.transform.position = center;
                Camera.main.orthographicSize = targetSize;
            }
        }

        private static float CalculateCellSize(int gridSize)
        {
            var screenRatio = (float)Screen.width / Screen.height;
            const float referenceHeight = 20f;
            var referenceWidth = referenceHeight * screenRatio;

            return Mathf.Min(referenceWidth, referenceHeight) / gridSize;
        }
    }
}
