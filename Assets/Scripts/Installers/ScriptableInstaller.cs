using UnityEngine;
using Zenject;
using Zenject.Asteroids;

[CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "Installers/ScriptableInstaller")]
public class ScriptableInstaller : ScriptableObjectInstaller<ScriptableInstaller>
{
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private RoundSettings _roundSettings;
    [SerializeField] private CardView _cardView;

    public override void InstallBindings()
    {
        Container.BindInstance(_cardCollection);
        Container.BindInstance(_roundSettings);

        Container.BindFactory<CardView, CardView.Factory>()
    // This means that any time Asteroid.Factory.Create is called, it will instantiate
    // this prefab and then search it for the Asteroid component
    .FromComponentInNewPrefab(_cardView)
    // We can also tell Zenject what to name the new gameobject here
    .WithGameObjectName("Card");
    // GameObjectGroup's are just game objects used for organization
    // This is nice so that it doesn't clutter up our scene hierarchy
    //.UnderTransformGroup("Cards");
    }
}