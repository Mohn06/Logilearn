using UnityEngine;
using TMPro;
using System.Collections;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage Instance;

    public GameObject toastPanel;
    public TextMeshProUGUI toastText;
    public CanvasGroup canvasGroup; // 👈 add this

    public float fadeDuration = 0.3f;

    void Awake()
    {
        Instance = this;
        toastPanel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowToast(string message, float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ToastRoutine(message, duration));
    }

    IEnumerator ToastRoutine(string message, float duration)
    {
        toastText.text = message;
        toastPanel.SetActive(true);

        // 🔥 Fade IN
        yield return StartCoroutine(Fade(0f, 1f));

        // ⏱ Stay
        yield return new WaitForSeconds(duration);

        // 🔥 Fade OUT
        yield return StartCoroutine(Fade(1f, 0f));

        toastPanel.SetActive(false);
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = end;
    }
}