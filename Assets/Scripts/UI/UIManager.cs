using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIManager : Singleton<UIManager>, IObserverable
{
    #region Field Declarations

    public event Action<TweenCallback> OnMenuAction = (TweenCallback method) => { };

    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image timeBar;
    [SerializeField] private Image replayImage;
    [SerializeField] private Image damageImage;

    [Header("Main Game")]
    [SerializeField] private Canvas mainGame;
    [SerializeField] private Button restartButton;
    [SerializeField] private float speedRotation = 5f;
    [SerializeField] private StatusText statusText;

    [Header("Start Menu")]
    [SerializeField] private Canvas startMenu;
    [SerializeField] private Button easyButton;
    [SerializeField] private float easyDifficult = 15f;
    [SerializeField] private Button mediumButton;
    [SerializeField] private float mediumDifficult = 10f;
    [SerializeField] private Button hardButton;
    [SerializeField] private float hardDifficult = 7f;
    [SerializeField] private Button quitButton;

    private GameSceneController gameSceneController;



    #endregion

    #region Startup

    private void Start()
    {

        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);//Subscribe on observer
        SubscribeOnEvents();
        SetButtons();
    }

    #endregion

    #region Subject Implementation

    #region Subscribe Methods
    private void SubscribeOnEvents()
    {
        gameSceneController.ScoreUpdateOnEat += ScoreUpdateOnEat;
        gameSceneController.LifeControl += HealthUpdate;
        gameSceneController.ReplayTimeControl += ReplayTimeUpdate;
        gameSceneController.ReplayControl += ReplayWindowController;
        gameSceneController.DamageControl += DamageWindowController;
    }
    private void ScoreUpdateOnEat(int pointValue)//Update Score
    {
        scoreText.text = "- " + pointValue.ToString();
    }
    private void HealthUpdate(float lifes)//Update Health Bar
    {
        healthBar.fillAmount = lifes;
    }
    private void ReplayTimeUpdate(float time)//Update replay time
    {
        timeBar.fillAmount = time;
    }
    private void ReplayWindowController(bool windowStatus)//Activate or Deactivate window when player use "replay"
    {
        replayImage.enabled = windowStatus;
    }
    private void DamageWindowController(bool windowStatus)//Activate or Deactivate window when player get damage
    {
        damageImage.enabled = windowStatus;
    }

    #endregion

    #region Start Game
    private void SetButtons()//Set methods button for choice difficulty
    {
        restartButton.onClick.AddListener(RestartGame);
        easyButton.onClick.AddListener(() => Easy(() => StartGame(easyDifficult)));
        mediumButton.onClick.AddListener(() => Medium(() => StartGame(mediumDifficult)));
        hardButton.onClick.AddListener(() => Hard(() => StartGame(hardDifficult)));
        quitButton.onClick.AddListener(Quit);
    }

    private void Easy(TweenCallback method)//Set easy game
    {
        RectTransform transformButton = easyButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(method));
    }
    private void Medium(TweenCallback method)//Set medium game
    {
        RectTransform transformButton = mediumButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(method));
    }
    private void Hard(TweenCallback method)//Set hard game
    {
        RectTransform transformButton = hardButton.GetComponent<RectTransform>();
        transformButton.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2, 0.25f).OnComplete(() => OnMenuAction(method));
    }
    private void StartGame(float difficulty)//Start main game and set difficult
    {
        if (!gameSceneController.StartGame)
        {
            gameSceneController.difficulty = difficulty;
            startMenu.enabled = false;
            mainGame.enabled = true;
            gameSceneController.StartGame = true;
        }
    }
    private void Quit()//Exit game
    {
        Application.Quit();
    }
    #endregion

    #region GameOver
    private void ShowStatus(string newStatus)//Show Text when player has zero life
    {
        statusText.gameObject.SetActive(true);
        StartCoroutine(statusText.ChangeStatus(newStatus));
    }
    private void ShowRestartButton()//Activate button "restart" when player has zero life
    {
        restartButton.gameObject.SetActive(true);
        StartCoroutine(EffectRestartButton(speedRotation, restartButton.transform));
    }
    private void RestartGame()//Method restart scene.For Restart button
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator EffectRestartButton(float speedRotation, Transform targetObject)//Visual Effect for restart button. Rotate around self
    {
        while (true)
        {
            targetObject.Rotate(Vector3.forward * speedRotation * Time.deltaTime);
            yield return null;
        }
    }
    #endregion

    #region Observer Action
    public void Notify()
    {
        ShowStatus("Game Over");
        ShowRestartButton();
    }

    #endregion

    #endregion
}
