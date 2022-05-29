using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
//using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
//using static Cysharp.Threading.Tasks.AddressablesAsyncExtensions;

public class SceneController : MonoBehaviour
{
    public SceneInstance currentScene;
    [SerializeField] private AssetReference MainMenuScene;
    [SerializeField] private AssetReference LoadingScene; // address string
    [SerializeField] private AssetReference GameScene; // address string
    public string key; // address string

    private AsyncOperationHandle<SceneInstance> loadHandle;

    private SceneInstance previousLoadedScene;
    private bool clearPreviousScene;
    private void Awake()
    {
        if (FindObjectsOfType<SceneController>().Length > 1)
        {
            DestroyImmediate(this);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        LoadAddressableScene(MainMenuScene, LoadSceneMode.Single);
    }

    public void LoadGameScene() => LoadAddressableScene(GameScene, LoadSceneMode.Single);

    private void LoadAddressableScene(AssetReference key, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        //var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync("Scenes", typeof(Scene));
        
        if (clearPreviousScene && mode == LoadSceneMode.Additive)
        {
            Addressables.UnloadSceneAsync(previousLoadedScene).Completed += (asyncHandle) =>
            {
                clearPreviousScene = false;
                //Addressables.Release(asyncHandle);
                previousLoadedScene = new SceneInstance();
            };
        }

        Addressables.LoadSceneAsync(key, mode).Completed += (asyncHandle) =>
        {
            clearPreviousScene = true;
            previousLoadedScene = asyncHandle.Result;
        };
        
        //Addressables.Release(loadResourceLocationsHandle);
    }
}
