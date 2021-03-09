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

    [SerializeField] private StartScene startScene = new StartScene();
    [SerializeField] private GameScene gameScene = new GameScene();
    [SerializeField] private GameObject[] canvasPrefabs;
    private GameSceneController gameSceneController;
    private bool loadScene;


    [Serializable]
    private class StartScene
    {
        public float currentDifficult;
        public Button easyButton;
        public float easyDifficult = 15f;
        public Button mediumButton;
        public float mediumDifficult = 10f;
        public Button hardButton;
        public float hardDifficult = 7f;
        public Button quitButton;
    }

    [Serializable]
    private class GameScene
    {
        [Header("Restart")]
        public GameObject statusGame;
        public Button restartButton;
        public float speedRotation = 5f;

        [Header("GamePlay")]
        public TextMeshProUGUI scoreText;
        public Image healthBar;
        public Image replayTimeBar;
        public Image replayImage;
        public Image damageImage;

    }

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
    private void SetDefaultStartHUD()//Set methods button for choice difficulty and create canvas in "Start" scene
    {
        InstiniateHUD("Start Canvas");
        startScene.easyButton.onClick.AddListener(Easy);
        startScene.mediumButton.onClick.AddListener(Medium);
        startScene.hardButton.onClick.AddListener(Hard);
        startScene.quitButton.onClick.AddListener(Quit);
    }
    private void SetDefaultGameHUD()//Set methods button for choice difficulty and create canvas in "Main Game" scene
    {
        InstiniateHUD("Game Canvas");
        gameScene.restartButton.onClick.AddListener(RestartGame);
    }

    private void InstiniateHUD(string nameUI)//Create canvas for special scene
    {
        GameObject prefabInstance;
        for (int i = 0; i < canvasPrefabs.Length; i++)
        {
            if (canvasPrefabs[i].name == nameUI && nameUI == "Start Canvas")//Start Menu
            {
                prefabInstance = Instantiate(canvasPrefabs[i]);
                startScene.easyButton = prefabInstance.transform.GetChild(2).GetComponent<Button>();
                startScene.mediumButton = prefabInstance.transform.GetChild(3).GetComponent<Button>();
                startScene.hardButton = prefabInstance.transform.GetChild(4).GetComponent<Button>();
                startScene.quitButton = prefabInstance.transform.GetChild(5).GetComponent<Button>();
            }
            else if (canvasPrefabs[i].name == nameUI && nameUI == "Game Canvas")//Game Menu
            {
                prefabInstance = Instantiate(canvasPrefabs[i]);
                gameScene.replayImage = prefabInstance.transform.GetChild(0).GetComponent<Image>();
                gameScene.damageImage = prefabInstance.transform.GetChild(1).GetComponent<Image>();
                gameScene.scoreText = prefabInstance.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                gameScene.healthBar = prefabInstance.transform.GetChild(4).GetChild(0).GetComponent<Image>();
                gameScene.statusGame = prefabInstance.transform.GetChild(5).gameObject;
                gameScene.restartButton = prefabInstance.transform.GetChild(6).GetComponent<Button>();
                gameScene.replayTimeBar = prefabInstance.transform.GetChild(7).GetChild(0).GetComponent<Image>();
            }
        }
    }
    #endregion

    #region Subject Implementation

    #region Subscribe Methods
    private void SubscribeOnEvents()//Subscribe on event actions in GameSceneControll
    {
        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);//Subscribe on observer

        gameSceneController.ScoreUpdateOnEat += ScoreUpdateOnEat;
        gameSceneController.LifeControl += HealthUpdate;
        gameSceneController.ReplayTimeControl += ReplayTimeUpdate;
        gameSceneController.ReplayControl += ReplayWindowController;
        gameSceneController.DamageControl += DamageWindowController;
    }
    private void UnSubscribeOnEvents()//Unsubscribe from event actions of GameSceneControll
    {
        gameSceneController.RemoveObserver(this);//Unsubscribe on observer

        gameSceneController.ScoreUpdateOnEat -= ScoreUpdateOnEat;
        gameSceneController.LifeControl -= HealthUpdate;
        gameSceneController.ReplayTimeControl -= ReplayTimeUpdate;
        gameSceneController.ReplayControl -= ReplayWindowController;
        gameSceneController.DamageControl -= DamageWindowController;
    }
    private void ScoreUpdateOnEat(int pointValue)//Update Score
    {
        gameScene.scoreText.text = "- " + pointValue.ToString();
    }
    private void HealthUpdate(float lifes)//Update Health Bar
    {
        gameScene.healthBar.fillAmount = lifes;
    }
    private void ReplayTimeUpdate(float time)//Update replay time
    {
        gameScene.replayTimeBar.fillAmount = time;
    }
    private void ReplayWindowController(bool windowStatus)//Activate or Deactivate window when player use "replay"
    {
        gameScene.replayImage.enabled = windowStatus;
    }
    private void DamageWindowController(bool windowStatus)//Activate or Deactivate window when player get damage
    {
        gameScene.damageImage.enabled = windowStatus;
    }

    #endregion

    #region Start Game

    private void Easy()//Set easy game
    {
        RectTransform transformButton = startScene.easyButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(() => LoadScene("Main Game"))) ;
        startScene.currentDifficult = startScene.easyDifficult;
    }
    private void Medium()//Set medium game
    {
        RectTransform transformButton = startScene.mediumButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(() => LoadScene("Main Game")));
        startScene.currentDifficult = startScene.mediumDifficult;
    }
    private void Hard()//Set hard game
    {
        RectTransform transformButton = startScene.hardButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(() => LoadScene("Main Game")));
        startScene.currentDifficult = startScene.hardDifficult;
    }

    private void OnSceneLoadCompleted(AsyncOperation ao)//Call when scene loaded
    {
        if (SceneManager.GetActiveScene().name == "Main Game")
        {
            SetDefaultGameHUD();
            SubscribeOnEvents();
            gameSceneController.Difficulty = startScene.currentDifficult;//Set request difficulty, when scene loaded
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

    private void Quit()//Exit game
    {
        Application.Quit();
    }
    #endregion

    #region GameOver
    private void ShowStatusGame(bool newStatus)//Show Text when player has zero life
    {
        gameScene.statusGame.SetActive(newStatus);
    }
    private void ShowRestartButton()//Activate button "restart" when player has zero life
    {
        gameScene.restartButton.gameObject.SetActive(true);
    }
    private void RestartGame()//Method restart scene. For Restart button
    {
        RectTransform transformButton = gameScene.restartButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(() => LoadScene("Start")));
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
