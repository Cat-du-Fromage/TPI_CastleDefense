using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
public class MainMenuManager : MonoBehaviour
{
    private VisualElement root;
    
    private VisualElement buttonsFrame;
    
    private Button buttonNewGame;
    
    //Progress Bar related
    private ProgressBar loadingBar;
    private bool isLoadingScreen;
    private float target;
    
    //Quit button related
    private Button buttonQuit;
    private bool onButtonQuit;

    private bool gameLaunch;

    private AsyncOperation gameScene;

    private void Awake()
    {
        root ??= GetComponent<UIDocument>().rootVisualElement;
        buttonsFrame  = root.Q<VisualElement>("LeftScreen");
        buttonNewGame = root.Q<Button>("BtnPlay");
        buttonQuit    = root.Q<Button>("BtnQuit");
        loadingBar    = root.Q<ProgressBar>("LoadingProgress");
    }

    private void OnEnable()
    {
        if (buttonsFrame.ClassListContains("fadeOutAnimation"))
        {
            buttonsFrame.RemoveFromClassList("fadeOutAnimation");
        }
    }

    private void OnDisable()
    {
        DisableQuitButton();
        buttonsFrame?.UnregisterCallback<TransitionEndEvent>(EnableProgressBar);
        buttonNewGame.clicked -= DisableMenuButtons;
    }

    private void Start()
    {
        EnableQuitButton();
        buttonNewGame.clicked += DisableMenuButtons;
    }

    private void OnDestroy()
    {
        DisableQuitButton();
        buttonNewGame.clicked -= DisableMenuButtons;
        buttonsFrame?.UnregisterCallback<TransitionEndEvent>(EnableProgressBar);
    }
    
    private void Update()
    {
        if (!isLoadingScreen) return;
        float newProgress = Max(MoveTowards(loadingBar.value, target, Time.deltaTime), loadingBar.value);
        float value = select(newProgress, 1f, loadingBar.value > 0.89f);
        loadingBar.SetValueWithoutNotify(value);
    }

    //Make ButtonsFrame Fade Out
    private void DisableMenuButtons()
    {
        buttonsFrame.AddToClassList("fadeOutAnimation");
        buttonsFrame.RegisterCallback<TransitionEndEvent>(EnableProgressBar);
    }

    //Make LoadingBar Fade In
    private void EnableProgressBar(TransitionEndEvent evt)
    {
        loadingBar.AddToClassList("fadeInAnimation");
        
        //if (!gameLaunch);
        //{
            //gameScene = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
            //gameScene.allowSceneActivation = false;
            //gameLaunch = true;
        //}
        
        LoadScene("GameScene");
    }
    
    private async void LoadScene(string sceneName)
    {
        //Reset loading bar variables
        isLoadingScreen = true;

        if (gameLaunch) return;
        gameLaunch = true;

        //Load scene
        gameScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        gameScene.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);
            target = gameScene.progress;
        } while(gameScene.progress < 0.9f);
        
        await Task.Delay(1000);
        //activate new scene
        gameScene.allowSceneActivation = true;
        isLoadingScreen = false;
    }

    private void OnQuit(MouseUpEvent evt)
    {
        if (onButtonQuit)
        {
            Application.Quit();
        }
    }
    
    //==================================================================================================================
    //EVENTS CALLBACKS
    //==================================================================================================================

    //------------------------------------------------------------------------------------------------------------------
    //QUIT BUTTON
    //-----------
    private void EnableQuitButton()
    {
        buttonQuit.RegisterCallback<MouseUpEvent>(OnQuit);
        buttonQuit.RegisterMouseEnterExitEvent(evt=> onButtonQuit = true, evt=> onButtonQuit = false);
    }

    private void DisableQuitButton()
    {
        buttonQuit.UnregisterCallback<MouseUpEvent>(OnQuit);
        buttonQuit.UnRegisterMouseEnterExitEvent(evt=> onButtonQuit = true, evt=> onButtonQuit = false);
    }
    //------------------------------------------------------------------------------------------------------------------
}
