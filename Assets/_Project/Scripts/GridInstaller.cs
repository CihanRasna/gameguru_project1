using UnityEngine;
using Zenject;

namespace _Project.Scripts
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            Container.Bind<GridManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
        }
    }
}