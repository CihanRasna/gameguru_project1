using UnityEngine;

namespace _Project.Scripts
{
    public class GridCell : MonoBehaviour
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        [SerializeField] private GameObject xMark;
        public bool HasX { get; private set; }

        public void Initialize(int x, int y, GridManager manager)
        {
            X = x;
            Y = y;
            HasX = false;
            xMark.SetActive(false);
        }

        public void PlaceX()
        {
            HasX = true;
            xMark.SetActive(true);
        }

        public void ClearX()
        {
            HasX = false;
            xMark.SetActive(false);
        }
    }
}