using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour, IObserverable
{
    #region Field Declarations

    [Header("Player UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image healthBar;
    [SerializeField] Image timeBar;
    [SerializeField] Image replayImage;
    [SerializeField] Image damageImage;

    [Header("Main Game")]
    [SerializeField] Canvas mainGame;
    [SerializeField] Button restartButton;
    [SerializeField] float speedRotation = 5f;
    [SerializeField] StatusText statusText;

    [Header("Start Menu")]
    [SerializeField]  Canvas startMenu;
    [SerializeField]  Button easyButton;
    [SerializeField]  Button mediumButton;
    [SerializeField]  Button hardButton;
    [SerializeField] Button  quitButton;

    private GameSceneController gameSceneController;

    #endregion

    #region Startup
    private void Awake()
    {
        SetButtons();
    }

    private void Start()
    {
        gameSceneController = GameSceneController.Instance;
        gameSceneController.AddObserver(this);//Subscribe on observer
        SubscribeOnEvents();
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
        easyButton.onClick.AddListener(Easy);
        mediumButton.onClick.AddListener(Medium);
        hardButton.onClick.AddListener(Hard);
        quitButton.onClick.AddListener(Quit);
    }
    private void Easy()//Set easy game
    {
        gameSceneController.difficulty = 15f;
        StartGame();
    }
    private void Medium()//Set medium game
    {
        gameSceneController.difficulty = 10f;
        StartGame();
    }
    private void Hard()//Set hard game
    {
        gameSceneController.difficulty = 7f;
        StartGame();
    }
    private void StartGame()//Start main game
    {
        startMenu.enabled = false;
        mainGame.enabled = true;
        gameSceneController.StartGame = true;
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
