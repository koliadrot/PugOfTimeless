using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameField : MonoBehaviour
{
    #region Field Declaration
    [Header("Game Menu")]
    [SerializeField] private GameObject gameMenuCanvas;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("Option")]
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private Slider audioVolume;
    [SerializeField] private Button closeOptionButton;

    [Header("GamePlay")]
    [SerializeField] private GameObject gamePlayCanvas;
    [SerializeField] private Button gameMenuButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image replayTimeBar;
    [SerializeField] private Image replayImage;
    [SerializeField] private Image damageImage;

    [Header("Restart")]
    [SerializeField] private GameObject statusGame;
    [SerializeField] private Button restartButton;

    [Header("Audio")]
    [SerializeField] private AudioClip gameClip;
    [SerializeField] private AudioClip eatFoodClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip replayTimeClip;

    #endregion

    #region Properties
    public GameObject GameMenuCanvas => gameMenuCanvas;
    public Button ContinueButton => continueButton;
    public Button OptionButton => optionButton;
    public Button ExitButton => exitButton;

    public GameObject OptionCanvas => optionCanvas;
    public Slider AudioVolume => audioVolume;
    public Button CloseOptionButton => closeOptionButton;

    public GameObject GamePlayCanvas => gamePlayCanvas;
    public Button GameMenuButton => gameMenuButton;
    public TextMeshProUGUI ScoreText => scoreText;
    public GameObject StatusGame => statusGame;
    public Button RestartButton => restartButton;
    public Image HealthBar => healthBar;
    public Image ReplayTimeBar => replayTimeBar;
    public Image ReplayImage => replayImage;
    public Image DamageImage => damageImage;

    public AudioClip GameClip => gameClip;
    public AudioClip EatFoodClip => eatFoodClip;
    public AudioClip DamageClip => damageClip;
    public AudioClip ReplayTimeClip => replayTimeClip;

    #endregion
}
