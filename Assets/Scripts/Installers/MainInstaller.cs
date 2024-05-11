using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private Transform _cardsPatent;

    private GameController _gameController;

    public override void InstallBindings()
    {
        Container.Bind<GameController>().AsSingle();
        Container.BindFactory<CardModel, CardModel.Factory>().AsSingle();// .WhenInjectedInto<CardFactory>();
        //Container.BindFactory<CardView, CardView.Factory>().WhenInjectedInto<CardFactory>();
        Container.BindInstance<Transform>(_cardsPatent).WhenInjectedInto<CardPool>();
        Container.Bind<CardPool>().AsSingle();
        _gameController = Container.Resolve<GameController>();
    }

    private void OnDestroy()
    {
        _gameController.Dispose();
    }
}