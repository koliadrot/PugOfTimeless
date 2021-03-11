using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartField : MonoBehaviour
{
    #region Field Declaration
    [Header("Start Menu")]
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Header("Option")]
    [SerializeField] private GameObject optionCanvas;
    [SerializeField] private Slider audioVolume;
    [SerializeField] private Button closeOptionButton;

    [Header("Difficult")]
    [SerializeField] private GameObject difficultCanvas;
    [SerializeField] private Button easyButton;
    [SerializeField] private float easyDifficult;
    [SerializeField] private Button mediumButton;
    [SerializeField] private float mediumDifficult;
    [SerializeField] private Button hardButton;
    [SerializeField] private float hardDifficult;
    [SerializeField] private Button closeDifficultButton;  

    [Header("Audio")]
    [SerializeField] private AudioClip startClip;
    #endregion

    #region Properties
    public GameObject StartMenuCanvas => startMenuCanvas;
    public Button StartButton =>startButton;
    public Button OptionButton => optionButton;
    public Button QuitButton => quitButton;

    public GameObject DifficultCanvas => difficultCanvas;
    public Button EasyButton => easyButton;
    public float EasyDifficult => easyDifficult;
    public Button MediumButton => mediumButton;
    public float MediumDifficult => mediumDifficult;
    public Button HardButton => hardButton;
    public float HardDifficult => hardDifficult;
    public Button CloseDifficultButton => closeDifficultButton;

    public GameObject OptionCanvas => optionCanvas;
    public Slider AudioVolume => audioVolume;
    public Button CloseOptionButton => closeOptionButton;

    public AudioClip StartClip => startClip;
    #endregion
}
