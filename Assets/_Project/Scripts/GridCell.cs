using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public class GridCell : MonoBehaviour
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        [SerializeField] private GameObject xMark;

        public bool HasX { get; private set; }

        public void Initialize(int x, int y)
        {
            X = x;
            Y = y;
            HasX = false;

            if (xMark == null) return;
            xMark.SetActive(false);
            xMark.transform.localScale = Vector3.zero;
        }

        public void PlaceX()
        {
            HasX = true;
            xMark.SetActive(true);

            xMark.transform.localScale = Vector3.zero;
            xMark.transform.DOScale(Vector3.one * 0.15f, 0.25f).SetEase(Ease.OutBack);
        }

        public void ClearX()
        {
            HasX = false;

            xMark.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                xMark.SetActive(false);
            });
        }

        public void Shake()
        {
            var shakeSeq = DOTween.Sequence().SetId(GetInstanceID()+"_Shake");
            
        }
    }
}