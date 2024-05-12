using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CardsHolders _cardsHolders;

    public override void InstallBindings()
    {
        Container.Bind<GamePresenter>().AsSingle().NonLazy();
        Container.BindFactory<CardModel, CardModel.Factory>().AsSingle();
        Container.Bind<CardPool>().AsSingle();
        Container.BindInstance(_cardsHolders);
    }
}