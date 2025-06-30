
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private string _info;

    [Inject]
    public void Construct()
    {
        Debug.Log("✅ Construct() çağrıldı!");
        _info = "Zenject çalışıyor!";
    }

    private void Start()
    {
        Debug.Log("GameManager.Start() => " + _info);
    }
}
