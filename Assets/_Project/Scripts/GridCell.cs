using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public class GridCell : MonoBehaviour
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        [SerializeField] private GameObject xMark;
        private CanvasGroup _canvasGroup;

        public bool HasX { get; private set; }

        public void Initialize(int x, int y, GridManager manager)
        {
            X = x;
            Y = y;
            HasX = false;

            if (xMark == null) return;
            xMark.SetActive(false);

            _canvasGroup = xMark.GetComponent<CanvasGroup>() ?? xMark.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            xMark.transform.localScale = Vector3.zero;
        }

        public void PlaceX()
        {
            HasX = true;
            xMark.SetActive(true);

            _canvasGroup.alpha = 0f;
            xMark.transform.localScale = Vector3.zero;

            _canvasGroup.DOFade(1f, 0.2f);
            xMark.transform.DOScale(Vector3.one * 0.15f, 0.25f).SetEase(Ease.OutBack);
        }

        public void ClearX()
        {
            HasX = false;

            _canvasGroup.DOFade(0f, 0.2f);
            xMark.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                xMark.SetActive(false);
            });
        }
    }
}