using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIController : Singleton<UIController>, IObserverable
{
    #region Field Declarations

    public event Action<TweenCallback> OnMenuAction = (TweenCallback method) => { };
    private GameSceneController gameSceneController;
    private UIStartField startSceneUI;
    private UIGameField gameSceneUI;

    [SerializeField] private GameObject[] canvasPrefabs;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private bool loadScene;
    private float currentDifficult;

    #endregion

    #region Startup

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SetDefaultStartHUD();
    }
    #endregion

    #region Subject Implementation

    #region Initialization
    private void SetDefaultStartHUD()//Set methods button for choice difficulty and create canvas in "Start" scene
    {
        InstiniateHUD("Start Canvas");
        LoadAudioData();
    }
    private void SetDefaultGameHUD()//Set methods button for choice difficulty and create canvas in "Main Game" scene
    {
        InstiniateHUD("Game Canvas");
        LoadAudioData();
    }
    private void InstiniateHUD(string nameUI)//Create canvas for special scene
    {
        GameObject prefabInstance;
        for (int i = 0; i < canvasPrefabs.Length; i++)
        {
            if (canvasPrefabs[i].name == nameUI && nameUI == "Start Canvas")//Start Menu
            {
                prefabInstance = Instantiate(canvasPrefabs[i]);//Create UI "Start" Canvas
                startSceneUI = prefabInstance.GetComponent<UIStartField>();//Initialized components
                SetStartMenuButtons();//Set methods for buttons
                StartSoundPlay();//Play main sound in "Start" scene
            }
            else if (canvasPrefabs[i].name == nameUI && nameUI == "Game Canvas")//Game Menu
            {
                prefabInstance = Instantiate(canvasPrefabs[i]);//Create UI "Main Game" Canvas
                gameSceneUI = prefabInstance.GetComponent<UIGameField>();//Initialized components
                SetGameMenuButtons();//Set methods for buttons
                GameSoundPlay();//Play main sound in "Main Game" scene
            }
        }
    }
    private void SetGameMenuButtons()//Initialization buttons and methods for UI "Main Game" scene
    {
        gameSceneUI.RestartButton.onClick.AddListener(() => RestartGame(gameSceneUI.RestartButton));//--->Gameplay
        gameSceneUI.GameMenuButton.onClick.AddListener(() => GameMenu(gameSceneUI.GameMenuButton));//<---

        gameSceneUI.ContinueButton.onClick.AddListener(() => Continue(gameSceneUI.ContinueButton));//--->
        gameSceneUI.OptionButton.onClick.AddListener(() => OptionGameMenu(gameSceneUI.OptionButton));//Game Menu
        gameSceneUI.ExitButton.onClick.AddListener(() => Exit(gameSceneUI.ExitButton));//<---

        gameSceneUI.AudioVolume.onValueChanged.AddListener((float volume) => ChangeAudioValueGameMenu());//--->Options
        gameSceneUI.CloseOptionButton.onClick.AddListener(() => CloseOptionGameMenu(gameSceneUI.CloseOptionButton));//<---
    }
    private void SetStartMenuButtons()//Initialization buttons and methods for UI "Start" scene
    {
        startSceneUI.StartButton.onClick.AddListener(() => StartGame(startSceneUI.StartButton));//--->
        startSceneUI.OptionButton.onClick.AddListener(() => OptionStartMenu(startSceneUI.OptionButton));//Start Menu
        startSceneUI.QuitButton.onClick.AddListener(() => Quit(startSceneUI.QuitButton));//<---

        startSceneUI.EasyButton.onClick.AddListener(() => Easy(startSceneUI.EasyButton));//--->
        startSceneUI.MediumButton.onClick.AddListener(() => Medium(startSceneUI.MediumButton));//Choose Difficulty
        startSceneUI.HardButton.onClick.AddListener(() => Hard(startSceneUI.HardButton));//..
        startSceneUI.CloseDifficultButton.onClick.AddListener(() => CloseDifficult(startSceneUI.CloseDifficultButton));//<---

        startSceneUI.AudioVolume.onValueChanged.AddListener((float volume) => ChangeAudioValueStartMenu());//--->Options
        startSceneUI.CloseOptionButton.onClick.AddListener(() => CloseOptionStartMenu(startSceneUI.CloseOptionButton));//<---
    }
    private void OnSceneLoadCompleted(AsyncOperation ao)//Call when scene loaded
    {
        if (SceneManager.GetActiveScene().name == "Main Game")
        {
            SetDefaultGameHUD();
            SubscribeOnEvents();
            gameSceneController.Difficulty = currentDifficult;//Set request difficulty, when scene loaded
        }
        else if (SceneManager.GetActiveScene().name == "Start")
        {
            SetDefaultStartHUD();
            UnSubscribeOnEvents();
        }
        loadScene = false;
    }
    private void LoadScene(string level)//Load scene
    {
        if (!loadScene)//Protect from copy calls
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(level);
            ao.completed += OnSceneLoadCompleted;
            loadScene = true;
        }
    }
    #endregion

    #region Subscribe Methods
    private void SubscribeOnEvents()//Subscribe on event actions in GameSceneControll
    {
        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);//Subscribe on observer

        gameSceneController.ScoreUpdateOnEat += ScoreUpdateOnEat;
        gameSceneController.LifeControl += HealthUpdate;
        gameSceneController.FoodControl += EatFoodSound;
        gameSceneController.ReplayTimeControl += ReplayTimeUpdate;
        gameSceneController.ReplayControl += ReplayWindowController;
        gameSceneController.ReplayControl += ReplaySound;
        gameSceneController.DamageControl += DamageWindowController;
        gameSceneController.DamageControl += DamageSound;
    }
    private void UnSubscribeOnEvents()//Unsubscribe from event actions of GameSceneControll
    {
        gameSceneController.RemoveObserver(this);//Unsubscribe on observer

        gameSceneController.ScoreUpdateOnEat -= ScoreUpdateOnEat;
        gameSceneController.LifeControl -= HealthUpdate;
        gameSceneController.FoodControl -= EatFoodSound;
        gameSceneController.ReplayTimeControl += ReplayTimeUpdate;
        gameSceneController.ReplayControl -= ReplayWindowController;
        gameSceneController.ReplayControl -= ReplaySound;
        gameSceneController.DamageControl -= DamageWindowController;
        gameSceneController.DamageControl -= DamageSound;
    }
    private void ScoreUpdateOnEat(int pointValue)//Update Score
    {
        gameSceneUI.ScoreText.text = "- " + pointValue.ToString();
    }
    private void HealthUpdate(float lifes)//Update Health Bar
    {
        gameSceneUI.HealthBar.fillAmount = lifes;
    }
    private void ReplayTimeUpdate(float time)//Update replay time
    {
        gameSceneUI.ReplayTimeBar.fillAmount = time;
    }
    private void ReplayWindowController(bool windowStatus)//Activate or Deactivate window when player use "replay"
    {
        gameSceneUI.ReplayImage.enabled = windowStatus;
    }
    private void DamageWindowController(bool windowStatus)//Activate or Deactivate window when player get damage
    {
        gameSceneUI.DamageImage.enabled = windowStatus;
    }
    private void EatFoodSound(bool status)//Play eat sound
    {
        if (status)
            audioSource.PlayOneShot(gameSceneUI.EatFoodClip);
    }
    private void DamageSound(bool status)//Play damage sound
    {
        if (status)
            audioSource.PlayOneShot(gameSceneUI.DamageClip);
    }
    private void ReplaySound(bool status)//Play replay time sound and return play main game sound
    {
        if (status)
        {
            audioSource.clip = gameSceneUI.ReplayTimeClip;
            audioSource.Play();
        }
        else
            GameSoundPlay();
    }
    #endregion

    #region UI "Start" Scene

    #region Start Menu
    private void StartGame(Button button)
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            startSceneUI.StartMenuCanvas.SetActive(false);
            startSceneUI.DifficultCanvas.SetActive(true);
        }));
    }
    private void OptionStartMenu(Button button)
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            startSceneUI.StartMenuCanvas.SetActive(false);
            startSceneUI.OptionCanvas.SetActive(true);
        }));
    }
    private void Quit(Button button)//Exit game
    {
        button.DOPunch(() => OnMenuAction(() => Application.Quit()));
    }
    #endregion

    #region Choose Difficulty
    private void Easy(Button button)//Set easy game
    {
        button.DOPunch(() => OnMenuAction(() => LoadScene("Main Game")));
        currentDifficult = startSceneUI.EasyDifficult;
    }
    private void Medium(Button button)//Set medium game
    {
        button.DOPunch(() => OnMenuAction(() => LoadScene("Main Game")));
        currentDifficult = startSceneUI.MediumDifficult;
    }
    private void Hard(Button button)//Set hard game
    {
        button.DOPunch(() => OnMenuAction(() => LoadScene("Main Game")));
        currentDifficult = startSceneUI.HardDifficult;
    }
    private void CloseDifficult(Button button)//Close difficulty choice and back start menu
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            startSceneUI.DifficultCanvas.SetActive(false);
            startSceneUI.StartMenuCanvas.SetActive(true);
        }));
    }
    #endregion

    #region Options
    private void CloseOptionStartMenu(Button button)
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            startSceneUI.OptionCanvas.SetActive(false);
            startSceneUI.StartMenuCanvas.SetActive(true);
        }));
    }
    private void ChangeAudioValueStartMenu()
    {
        audioSource.volume = startSceneUI.AudioVolume.value;
        SaveAudioData();
    }
    #endregion

    #endregion

    #region UI "Main Game" Scene

    #region GamePlay
    private void GameMenu(Button button)//Open game menu in main game scene
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            gameSceneUI.GamePlayCanvas.SetActive(false);
            gameSceneUI.GameMenuCanvas.SetActive(true);
        }));
    }
    #endregion

    #region Game Menu
    private void Continue(Button button)//Close game menu and activate gameplay UI
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            gameSceneUI.GameMenuCanvas.SetActive(false);
            gameSceneUI.GamePlayCanvas.SetActive(true);
        }));
    }
    private void OptionGameMenu(Button button)//Open option settings and close game menu
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            gameSceneUI.GameMenuCanvas.SetActive(false);
            gameSceneUI.OptionCanvas.SetActive(true);
        }));
    }
    private void Exit(Button button)//Exit game in start scene
    {
        button.DOPunch(() => OnMenuAction(() => LoadScene("Start")));
    }
    #endregion

    #region Options
    private void CloseOptionGameMenu(Button button)//Close option and back in game menu
    {
        button.DOPunch(() => OnMenuAction(() =>
        {
            gameSceneUI.OptionCanvas.SetActive(false);
            gameSceneUI.GameMenuCanvas.SetActive(true);
        }));
    }
    private void ChangeAudioValueGameMenu()//Method for audio slider in game scene
    {
        audioSource.volume = gameSceneUI.AudioVolume.value;
        SaveAudioData();
    }

    #endregion

    #endregion

    #region GameOver
    private void ShowStatusGame(bool newStatus)//Show Text when player has zero life
    {
        gameSceneUI.StatusGame.SetActive(newStatus);
    }
    private void ShowRestartButton()//Activate button "restart" when player has zero life
    {
        gameSceneUI.RestartButton.gameObject.SetActive(true);
    }
    private void RestartGame(Button button)//Method restart scene. For Restart button
    {
        button.DOPunch(() => OnMenuAction(() => LoadScene("Main Game")));
    }
    #endregion

    #region Audio
    private void StartSoundPlay()//Play main theme in start scene
    {
        audioSource.clip = startSceneUI.StartClip;
        audioSource.Play();
    }
    private void GameSoundPlay()//Play main theme in game scene
    {
        audioSource.clip = gameSceneUI.GameClip;
        audioSource.Play();
    }
    private void SaveAudioData()//Save audio settings
    {
        PlayerPrefs.SetFloat("Audio", audioSource.volume);
    }
    private void LoadAudioData()//Load audio settings
    {
        if (!PlayerPrefs.HasKey("Audio"))
        {
            SaveAudioData();
        }
        if (SceneManager.GetActiveScene().name == "Start")
        {
            startSceneUI.AudioVolume.value = PlayerPrefs.GetFloat("Audio");
        }
        else if (SceneManager.GetActiveScene().name == "Main Game")
        {
            gameSceneUI.AudioVolume.value = PlayerPrefs.GetFloat("Audio");
        }
    }
    #endregion

    #region Observer Action
    public void Notify()
    {
        ShowStatusGame(true);
        ShowRestartButton();
    }

    #endregion

    #endregion
}
