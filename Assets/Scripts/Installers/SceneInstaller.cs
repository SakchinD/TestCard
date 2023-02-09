using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] ImageLoader _imageLoader;
    [SerializeField] ObjectPool _objectPool;
    [SerializeField] HandCardsController _handCards;
    public override void InstallBindings()
    {
        BindImageLoader();
        BindObjectPool();
        BindHandCardController();
    }

    void BindImageLoader()
    {
        Container.Bind<ImageLoader>().FromInstance(_imageLoader).AsSingle();
    }

    void BindObjectPool()
    {
        Container.Bind<ObjectPool>().FromInstance(_objectPool).AsSingle();
    }

    void BindHandCardController()
    {
        Container.Bind<HandCardsController>().FromInstance(_handCards).AsSingle();
    }
}