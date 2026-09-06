using System.Collections;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float animationDuration = 0.25f;

    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OpenPopup()
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimatePopup(0, 1));
    }

    public void ClosePopup()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        StartCoroutine(AnimatePopup(1, 0));
    }

    IEnumerator AnimatePopup(float startAlpha, float endAlpha)
    {
        float time = 0;

        Vector3 startScale = (startAlpha == 0) ? Vector3.one * 0.8f : Vector3.one;
        Vector3 endScale = (endAlpha == 0) ? Vector3.one * 0.8f : Vector3.one;

        if (endAlpha == 1)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        if (endAlpha == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}