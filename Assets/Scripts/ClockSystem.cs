using UnityEngine;

public class ClockSystem : MonoBehaviour
{
    float elapsedTime;
    int lastDisplayedSeconds = -1;
    bool running;
    [SerializeField] TMPro.TextMeshProUGUI timeTXT;
    [SerializeField] UIManager uiManager;

    void Start()
    {
        ResetTimer();
        StartTimer();
    }

    void Update()
    {
        if (!running) return;
        elapsedTime += Time.deltaTime;
        int seconds = Mathf.FloorToInt(elapsedTime);
        if (seconds != lastDisplayedSeconds)
        {
            lastDisplayedSeconds = seconds;
            UpdateTimeText(seconds);
        }
    }

    void UpdateTimeText(int seconds)
    {
        int minutes = seconds / 60;
        int secs = seconds % 60;
        if (timeTXT != null)
        {
            timeTXT.text = $"Time: {minutes:00}:{secs:00}";
        }
    }
    public void StartTimer()
    {
        running = true;
    }
    public void StopTimer()
    {
        running = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        lastDisplayedSeconds = -1;
        UpdateTimeText(0);
        running = false;
    }

    public int GetElapsedSeconds()
    {
        return Mathf.FloorToInt(elapsedTime);
    }

    public string GetFormattedTime()
    {
        int totalSeconds = GetElapsedSeconds();
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
