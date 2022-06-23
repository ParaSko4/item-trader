using ItemTrader.Gameplay;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private PlayerController playerController;

    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromInstance(playerController).AsSingle().NonLazy();
    }
}