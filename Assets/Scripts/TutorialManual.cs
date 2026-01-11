using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI introText;
    [SerializeField] GameObject[] textArray;
    [SerializeField] private GameObject spotLight;
    [SerializeField] float charDelay = 0.05f;

    private TextMeshProUGUI[] tmps;
    private string[] messages;
    private Coroutine typingCoroutine;
    private int currentIndex = 0;
    private bool isTypingComplete;
    private TextMeshProUGUI currentTMP;

    public void Start()
    {
        // —обираем TMP-цели и сообщени€
        if (textArray != null && textArray.Length > 0)
        {
            tmps = new TextMeshProUGUI[textArray.Length];
            messages = new string[textArray.Length];

            for (int i = 0; i < textArray.Length; i++)
            {
                var go = textArray[i];
                tmps[i] = go != null ? go.GetComponentInChildren<TextMeshProUGUI>() : null;
                messages[i] = tmps[i] != null ? (tmps[i].text ?? string.Empty) : string.Empty;
            }
        }
        else
        {
            tmps = new TextMeshProUGUI[0];
            messages = new string[0];
        }

        // fallback: если нет сообщений Ч используем introText
        if (messages.Length == 0)
        {
            if (introText == null)
            {
                Debug.LogError("[TutorialManual] Ќет текста дл€ показа (textArray пуст и introText не задан).");
                Destroy(gameObject);
                return;
            }

            messages = new string[] { introText.text ?? string.Empty };
            tmps = new TextMeshProUGUI[] { introText };
        }

        // ƒеактивируем все текстовые объекты Ч будут активироватьс€ по Show(index)
        DeactivateAll();

        Time.timeScale = 0f; // пауза игры
        currentIndex = 0;
        Show(currentIndex);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        if (currentTMP == null)
            return;

        if (!isTypingComplete)
        {
            // мгновенно показать весь текст текущего TMP
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            currentTMP.ForceMeshUpdate();
            currentTMP.maxVisibleCharacters = currentTMP.textInfo.characterCount;
            isTypingComplete = true;
            typingCoroutine = null;
            return;
        }

        // перейти к следующему или завершить
        currentIndex++;
        if (currentIndex < messages.Length)
        {
            Show(currentIndex);
        }
        else
        {
            EndTutorial();
        }
    }

    private void Show(int index)
    {
        if (index < 0 || index >= messages.Length) return;

        // выберем TMP дл€ текущего индекса, иначе fallback на introText
        currentTMP = (index < tmps.Length && tmps[index] != null) ? tmps[index] : introText;

        // гарантируем, что текущий TMP активен
        if (currentTMP != null)
            currentTMP.gameObject.SetActive(true);

        // запускаем печать
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTypingComplete = false;
        typingCoroutine = StartCoroutine(TypeText(currentTMP, messages[index]));
    }

    private IEnumerator TypeText(TextMeshProUGUI target, string text)
    {
        if (target == null)
        {
            isTypingComplete = true;
            typingCoroutine = null;
            yield break;
        }

        target.text = text ?? string.Empty;
        target.ForceMeshUpdate();
        int total = target.textInfo.characterCount;

        if (total <= 0)
        {
            target.maxVisibleCharacters = 0;
            isTypingComplete = true;
            typingCoroutine = null;
            yield break;
        }

        target.maxVisibleCharacters = 0;
        for (int i = 0; i < total; i++)
        {
            target.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(charDelay);
        }

        isTypingComplete = true;
        typingCoroutine = null;
    }

    private void MoveLightToPosition(Vector2 position)
    {
        if (spotLight != null && currentIndex == 2)
        {
            position = new Vector2(-7f, -1.5f);
            spotLight.transform.position = position;
        }
    }

    private void DeactivateAll()
    {
        if (tmps == null) return;
        for (int i = 0; i < tmps.Length; i++)
        {
            if (tmps[i] == null) continue;
            tmps[i].gameObject.SetActive(false);
            spotLight.gameObject.SetActive(false);
        }

        // тоже скрываем introText, если он отдельно не входит в tmps
        if (introText != null)
        {
            introText.gameObject.SetActive(false);
        }
    }

    public void EndTutorial()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}