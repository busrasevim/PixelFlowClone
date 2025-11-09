using _Project.Scripts.Audio;
using _Project.Scripts.Level;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.Managers;
using _Project.Scripts.Pools;
using _Project.Scripts.SaveSystem;
using _Project.Scripts.Signals;
using _Project.Scripts.State_Machine.State_Machines;
using _Project.Scripts.UI;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Installers
{
    //If there is a need, open the object pool lines.
    public class GameSceneMonoInstaller : MonoInstaller
    {
        [SerializeField] private GameObject objectPoolObject;
       [SerializeField] private GameObject audioManagerObject;

        public override void InstallBindings()
        {
            //Signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<OnLevelCompletedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnLevelEndSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnLevelStartSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnLevelRestartSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnSoundSettingChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnMusicSettingChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnPlaySignal>().OptionalSubscriber();
            Container.DeclareSignal<OnRestartSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnLevelSetupRequestedSignal>().OptionalSubscriber();



            Container.Bind<ISaveSystem>().To<JsonSaveSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<DataManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<EconomyManager>().AsSingle();
            Container.Bind<AudioManager>().FromComponentInNewPrefab(audioManagerObject).AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
            Container.Bind<ObjectPool>().FromComponentInNewPrefab(objectPoolObject).AsSingle();
            Container.BindInterfacesAndSelfTo<MainStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIStateMachine>().AsSingle();
            Container.BindInterfacesTo<CreativeManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FXManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SettingsManager>().AsSingle();
        }
    }
}