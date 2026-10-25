using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    //Reference
    //Player
    [Header("Player")]
    public GameObject _player;

    [Header("Groceries")]
    public GameObject[] objects;

    [Header("Bonus")]
    public GameObject Bonus;

    [Header("Camera")]
    public Camera mainCamera;

    //GamePlay
    [Header("GamePlay")]
    public GameObject Gameplay;

    //UI Management

    //Main UI
    [Header("UI")]
    //OpeningPanelUI
    [Header("TitleScreen UI")]
    [Header("Main UI")]
    public GameObject OpeningPanel;
    [Header("Buttons")]
    public Button playButton;
    public Button infoButton;
    [Header("Tutorial UI")]
    public GameObject TutorialPanel;


    [Header("Buttons")]
    public Button closeButton;
    public Button okButton;

    [Header("GamePlay UI")]
    [Header("Main UI")]
    public GameObject GamePlayUIPanel;
    public GameObject FirstIntro;
    public GameObject FirstStep;
    public GameObject SecondStep;
    [Header("Score Card")]
    public TMP_Text groceriesScoreCard;
    public TMP_Text SavingsScoreCard;
    [Header("Button")]
    public Button pauseBtn;
    [Header("Pause UI")]
    public GameObject PauseUIPanel;
    [Header("Button")]
    public Button ResumeBtn;
    public Button MainmenuBtn;
    public Button pauseCloseBtn;

    [Header("GameOver UI")]
    public GameObject GameOverUIPanel;

    [Header("Score Card")]
    public TMP_Text Overall_Score;
    public TMP_Text groceriesScoreCollected;
    public TMP_Text SavingsScoreCollected;

    [Header("Daily Objectives")]
    public TMP_Text Objective_1;
    public TMP_Text Objective_2;

    [Header("Button")]
    public Button PlayAgainBtn;
    public Button MainmenuGOBtn;
    //Singleton
    public static ResourceManager instance;


    public void Awake()
    {
        if (instance == null)

            instance = this;
    }
}
