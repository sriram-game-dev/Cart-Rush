
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    //Singleton
    public static UIManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        AtStart();
        //OpeningScreen
        TitleScreen();

        //OpeningScreen Click
        PlayButton();
        InfoButton();

        //Tutorial Panel Click
        OkButton();
        CloseButton();

        //In-game & GameOver & Pause Buttons
        SetupGameUIButtons();
    }

    //before Opening
    void AtStart()
    {
        if (ResourceManager.instance.Gameplay != null)
            ResourceManager.instance.Gameplay.SetActive(false);
        if (ResourceManager.instance.OpeningPanel != null)
            ResourceManager.instance.OpeningPanel.SetActive(false);
        if (ResourceManager.instance.GamePlayUIPanel != null)
            ResourceManager.instance.GamePlayUIPanel.SetActive(false);
        if (ResourceManager.instance.TutorialPanel != null)
            ResourceManager.instance.TutorialPanel.SetActive(false);
        if (ResourceManager.instance.FirstIntro != null)
            ResourceManager.instance.FirstIntro.SetActive(false);
        if (ResourceManager.instance.PauseUIPanel != null)
            ResourceManager.instance.PauseUIPanel.SetActive(false);
        if (ResourceManager.instance.GameOverUIPanel != null)
            ResourceManager.instance.GameOverUIPanel.SetActive(false);
    }

    //--------------------------------------------//
    //--------------Title Card--------------------//
    //--------------------------------------------//

    public void TitleScreen()
    {
        ResourceManager.instance.OpeningPanel.SetActive(true);
    }

    //Opening Panel Button Function
    void PlayButton()
    {
        ResourceManager.instance.playButton.onClick.AddListener(PlayButtonFunction);
    }

    void InfoButton()
    {
        ResourceManager.instance.infoButton.onClick.AddListener(
            () => TutorialButtonFunction("Entry_TitleScreen_Open")
        );
    }
    public void PlayButtonFunction()
    {
        string login = PlayerPrefsManager.instance.LoginStatus;

        ResourceManager.instance.OpeningPanel.SetActive(false);
        ResourceManager.instance.Gameplay.SetActive(true);

        if (login == "Entry_LoginFirstTime")
        {
            GameManager.instance.SetLoginStatus(
                "Entry_Tutorial_PlayPressed"
            );

            ResourceManager.instance.TutorialPanel.SetActive(true);
            ResourceManager.instance.GamePlayUIPanel.SetActive(false);
        }
        else
        {
            ResourceManager.instance.TutorialPanel.SetActive(false);
            StartActualGameplay();
        }
    }

    //-----------------------------------------------//
    //--------------Tutorial Card--------------------//
    //-----------------------------------------------//

    void OkButton()
    {
        ResourceManager.instance.okButton.onClick.AddListener(
            OkButtonFunction
        );
    }

    void CloseButton()
    {
        ResourceManager.instance.closeButton.onClick.AddListener(
            CloseButtonFunction
        );
    }

    void OkButtonFunction()
    {
        if (GameManager.instance.IsLoginStatus(
            "Entry_Tutorial_PlayPressed"))
        {
            StartCoroutine(StartGameplayIntro());
        }
        else if (GameManager.instance.IsLoginStatus(
            "Entry_Intro_Pressed"))
        {
            ResourceManager.instance.TutorialPanel.SetActive(false);
            ResourceManager.instance.OpeningPanel.SetActive(true);
        }
    }

    void CloseButtonFunction()
    {
        if (GameManager.instance.IsLoginStatus(
            "Entry_Tutorial_PlayPressed"))
        {
            // Treat closing the tutorial as skipping/completing it.
            PlayerPrefsManager.instance.SaveLoginStatus(
                "Entry_InGamePlay"
            );

            StartCoroutine(StartGameplayIntro());
        }
        else if (GameManager.instance.IsLoginStatus(
            "Entry_Intro_Pressed"))
        {
            ResourceManager.instance.TutorialPanel.SetActive(false);
            ResourceManager.instance.OpeningPanel.SetActive(true);
        }
    }

    public void TutorialButtonFunction(string msgInfo)
    {
        switch (msgInfo)
        {
            case "Entry_TitleScreen_Open":
                GameManager.instance.SetLoginStatus("Entry_Intro_Pressed");
                ResourceManager.instance.OpeningPanel.SetActive(true);
                ResourceManager.instance.TutorialPanel.SetActive(true);
                break;

            case "Entry_TitleScreen_Close_CloseBtn":

                ResourceManager.instance.OpeningPanel.SetActive(true);
                ResourceManager.instance.TutorialPanel.SetActive(false);
                Debug.Log("Pressed");
                break;

            case "Entry_TitleScreen_Ok_OkBtn":

                ResourceManager.instance.OpeningPanel.SetActive(true);
                ResourceManager.instance.TutorialPanel.SetActive(false);
                break;

            case "Entry_InGame_Close_CloseBtn":

                ResourceManager.instance.TutorialPanel.SetActive(false);
                break;

            case "Entry_InGame_Ok_OkBtn":

                ResourceManager.instance.TutorialPanel.SetActive(false);
                break;
        }
    }
    IEnumerator StartGameplayIntro()
    {
        GameManager.instance.StartTutorial();

        ResourceManager.instance.TutorialPanel.SetActive(false);
        ResourceManager.instance.Gameplay.SetActive(true);
        ResourceManager.instance.GamePlayUIPanel.SetActive(true);

        ResourceManager.instance.FirstIntro.SetActive(true);
        ResourceManager.instance.FirstStep.SetActive(true);
        ResourceManager.instance.SecondStep.SetActive(false);

        yield return new WaitForSeconds(0f);

        GameManager.instance.isWaitingForPlayerClick = true;
    }

    public void NextStep()
    {
        ResourceManager.instance.SecondStep.SetActive(true);
        ResourceManager.instance.FirstStep.SetActive(false);

    }
    public void FinishStep()
    {
        if (ResourceManager.instance.SecondStep != null)
            ResourceManager.instance.SecondStep.SetActive(false);

        if (ResourceManager.instance.FirstIntro != null)
            ResourceManager.instance.FirstIntro.SetActive(false);

        StartActualGameplay();
    }
    public void StartActualGameplay()
    {
        if (GameManager.instance.IsLoginStatus("Entry_Tutorial_PlayPressed"))
        {
            if (PlayerPrefsManager.instance != null)
                PlayerPrefsManager.instance.SaveLoginStatus("Entry_InGamePlay");

            GameManager.instance.SetLoginStatus("Entry_InGamePlay");
        }

        if (ResourceManager.instance.TutorialPanel != null)
            ResourceManager.instance.TutorialPanel.SetActive(false);
        if (ResourceManager.instance.FirstIntro != null)
            ResourceManager.instance.FirstIntro.SetActive(false);
        if (ResourceManager.instance.GamePlayUIPanel != null)
            ResourceManager.instance.GamePlayUIPanel.SetActive(true);

        GameManager.instance.isWaitingForPlayerClick = false;
        GameManager.instance.isWaitingForPlayerMovement = false;

        GameManager.instance.StartGameplay();
    }

    private void SetupGameUIButtons()
    {
        if (ResourceManager.instance.pauseBtn != null)
            ResourceManager.instance.pauseBtn.onClick.AddListener(PauseGame);

        if (ResourceManager.instance.ResumeBtn != null)
            ResourceManager.instance.ResumeBtn.onClick.AddListener(ResumeGame);

        if (ResourceManager.instance.pauseCloseBtn != null)
            ResourceManager.instance.pauseCloseBtn.onClick.AddListener(ResumeGame);

        if (ResourceManager.instance.MainmenuBtn != null)
            ResourceManager.instance.MainmenuBtn.onClick.AddListener(ReturnToMainMenu);

        if (ResourceManager.instance.PlayAgainBtn != null)
            ResourceManager.instance.PlayAgainBtn.onClick.AddListener(PlayAgain);

        if (ResourceManager.instance.MainmenuGOBtn != null)
            ResourceManager.instance.MainmenuGOBtn.onClick.AddListener(ReturnToMainMenu);
    }

    public void UpdateScoreDisplay(int groceries, int savings)
    {
        if (ResourceManager.instance.groceriesScoreCard != null)
            ResourceManager.instance.groceriesScoreCard.text = groceries.ToString();

        if (ResourceManager.instance.SavingsScoreCard != null)
            ResourceManager.instance.SavingsScoreCard.text = savings.ToString();
    }

    //GameOver UI

    public void ShowGameOver(int groceries, int savings)
    {
        if (ResourceManager.instance.GameOverUIPanel != null)
            ResourceManager.instance.GameOverUIPanel.SetActive(true);

        int totalScore = groceries + savings;
        if (ResourceManager.instance.Overall_Score != null)
            ResourceManager.instance.Overall_Score.text = totalScore.ToString();

        if (ResourceManager.instance.groceriesScoreCollected != null)
            ResourceManager.instance.groceriesScoreCollected.text = groceries.ToString();

        if (ResourceManager.instance.SavingsScoreCollected != null)
            ResourceManager.instance.SavingsScoreCollected.text = savings.ToString();

        // Update Daily Objectives progress
        if (PlayerPrefsManager.instance != null)
        {
            PlayerPrefsManager.instance.CheckAndInitDailyObjectives();
            PlayerPrefsManager.instance.AddDailyGroceries(groceries);
            PlayerPrefsManager.instance.AddDailySavings(savings);

            if (ResourceManager.instance.Objective_1 != null)
            {
                ResourceManager.instance.Objective_1.text = $"{PlayerPrefsManager.instance.DailyGroceriesProgress} / {PlayerPrefsManager.DailyGroceriesTarget}";
            }

            if (ResourceManager.instance.Objective_2 != null)
            {
                ResourceManager.instance.Objective_2.text = $"{PlayerPrefsManager.instance.DailySavingsProgress} / {PlayerPrefsManager.DailySavingsTarget}";
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (ResourceManager.instance.PauseUIPanel != null)
            ResourceManager.instance.PauseUIPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (ResourceManager.instance.PauseUIPanel != null)
            ResourceManager.instance.PauseUIPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        if (ResourceManager.instance.GameOverUIPanel != null)
            ResourceManager.instance.GameOverUIPanel.SetActive(false);

        // Reset falling objects in GameManager
        GameManager.instance.StopFallingObjects();
        foreach (Transform child in GameManager.instance.transform)
        {
            child.gameObject.SetActive(false);
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
                rb.linearVelocity = Vector2.zero;
            }
        }

        StartActualGameplay();
    }
}
