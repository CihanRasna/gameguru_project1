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
            DOTween.Kill(GetInstanceID()+"_ClearXSeq",true);
            HasX = true;
            xMark.SetActive(true);

            xMark.transform.localScale = Vector3.zero;
            xMark.transform.DOScale(Vector3.one * 0.15f, 0.25f).SetEase(Ease.OutBack);
        }

        public void ClearX()
        {
            HasX = false;
            DOTween.Kill(GetInstanceID()+"_ClearXSeq",true);
            var seq = DOTween.Sequence().SetId(GetInstanceID()+"_ClearXSeq");
            seq.Join(transform.DOScale(Vector3.one * 0.15f, 0.25f).SetLoops(2, LoopType.Yoyo).SetRelative());
            seq.Append(xMark.transform.DOScale(Vector3.zero, 0.25f));
            seq.OnComplete(() =>
            {
                xMark.SetActive(false);
            });
        }

        public void Shake()
        {
            DOTween.Kill(GetInstanceID()+"_Shake",true);
            var shakeSeq = DOTween.Sequence().SetId(GetInstanceID()+"_Shake");
            shakeSeq.Join(transform.DOShakeRotation(0.1f, Vector3.forward * 15f, 10, 90f, true, ShakeRandomnessMode.Harmonic));
            shakeSeq.Join(xMark.transform.DOPunchScale(Vector3.one * 0.025f, 0.1f));
        }
    }
}