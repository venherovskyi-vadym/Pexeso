using UnityEngine;
using Zenject;

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
        //Container.Bind<ISettings>().FromInstance(_roundSettings);
        //Container.Bind<ISettingsStorage>().FromInstance(_roundSettings);
        //Container.BindInterfacesAndSelfTo<PlayerPrefsStorage>().AsSingle();
        //Container.BindInterfacesAndSelfTo<JsonSettingsStorage>().AsSingle();
        Container.BindInterfacesAndSelfTo<BinarySettingsStorage>().AsSingle();

        Container.BindFactory<CardView, CardView.Factory>().FromComponentInNewPrefab(_cardView).WithGameObjectName("Card");
    }
}