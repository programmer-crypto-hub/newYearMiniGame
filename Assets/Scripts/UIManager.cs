using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameObject endScreen;
    [SerializeField] public GameObject restartScreen;
    [SerializeField] UnityEngine.UI.Button levelButton;
    [SerializeField] TextMeshProUGUI scoreTXT;
    [SerializeField] TextMeshProUGUI timeTXT;
    [SerializeField] TextMeshProUGUI restartScoreTXT;
    [SerializeField] CharController charController;
    [SerializeField] TextMeshProUGUI restartTimeTXT;
    [SerializeField] ClockSystem clockSystem;
    [SerializeField] TutorialManual tutorialManual;
    [SerializeField] AllCoinCounter allCoinCounter;
    [SerializeField] TextMeshProUGUI healthTXT;
    [SerializeField] GameObject heart1;
    [SerializeField] GameObject heart2;
    [SerializeField] GameObject heart3;
    [SerializeField] int currentLevel;
    [SerializeField] int reachedLevel;
    [SerializeField] int levelToLoad;

    void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelToLoad = currentLevel + 1;
        reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        endScreen.SetActive(false);
        restartScreen.SetActive(false);

        if (clockSystem == null && charController != null)
            clockSystem = charController.clockSystem;
        
        UpdateHUDStats();
    }

    public void UpdateHearts()
    {
        if (charController != null)
        {
            int lives = charController.lives;
            if (lives == 2)
            {
                heart3.SetActive(false);
            }
            else if (lives == 1)
            {
                heart2.SetActive(false);
                heart3.SetActive(false);
            }
            else if (lives == 0)
            {
                heart1.SetActive(false);
                heart2.SetActive(false);
                heart3.SetActive(false);
            }
        }
    }

    // Обновляет статистику на HUD (во время игры)
    public void UpdateHUDStats()
    {
        if (charController != null)
        {
            healthTXT.text = "Lives: " + charController.lives.ToString();
            scoreTXT.text = "Coins: " + charController.coins.ToString();
        }
    }

    public void ActivateUI()
    {
        // показываем UI когда игра уже окончена
        if (charController != null && charController.isGameOver)
        {
            Debug.Log("UI is shown");
            levelButton.interactable = false;
            endScreen.SetActive(true);
            Time.timeScale = 0;
            // Останавливаем таймер у того экземпляра, который использует персонаж
            charController.clockSystem?.StopTimer();

            // Обновляем финальные значения перед показом
            scoreTXT.text = "Coins: " + charController.coins.ToString();
            timeTXT.text = "Time: " + FormatSeconds(charController.clockSystem?.GetElapsedSeconds() ?? 0);
            healthTXT.text = "Lives: " + charController.lives.ToString();
        }

        if (Input.GetKeyDown(KeyCode.R) || (Input.GetKeyDown(KeyCode.Escape)) && charController != null && charController.isGameOver && tutorialManual == null)
        {
            Debug.Log("Restart UI is shown");
            restartScreen.SetActive(true);
            Time.timeScale = 0;
            charController.clockSystem?.StopTimer();
        }
    }

    public void Restart()
    {
        charController?.clockSystem?.ResetTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        charController?.clockSystem?.ResetTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PreviousLevel()
    {
        charController?.clockSystem?.ResetTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Update()
    {
        if (currentLevel > reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedLevel", currentLevel);
            reachedLevel = currentLevel;
            PlayerPrefs.Save();
        }
        if (levelButton == null)
        {
            return;
        }
        if (levelToLoad <= reachedLevel)
        {
            levelButton.interactable = true;
        }
        else
        {
            levelButton.interactable = false;
        }
        

            // Обновляем HUD во время игры
            if (!endScreen.activeSelf && !restartScreen.activeSelf && charController != null && !charController.isGameOver)
        {
            UpdateHUDStats();
        }

        // Если endScreen активен — останавливаем таймер
        if (endScreen.activeSelf)
        {
            charController?.clockSystem?.StopTimer();
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Escape) && charController != null && charController.isGameOver)
        {
            Debug.Log("Restart UI is shown");
            restartScreen.SetActive(true);
            Time.timeScale = 0;
            charController.clockSystem?.StopTimer();
        }

        // Обновляем restart screen статистику
        if (restartScreen.activeSelf)
        {
            restartScoreTXT.text = (charController != null ? charController.coins.ToString() : "0");
            restartTimeTXT.text = FormatSeconds(charController?.clockSystem?.GetElapsedSeconds() ?? 0);
            clockSystem.StopTimer();
        }
    }

    public void Continue()
    {
        restartScreen.SetActive(false);
        Time.timeScale = 1;
        charController?.clockSystem?.StartTimer();
    }

    private string FormatSeconds(int totalSeconds)
    {
        int m = totalSeconds / 60;
        int s = totalSeconds % 60;
        return $"{m:00}:{s:00}";
    }
}
