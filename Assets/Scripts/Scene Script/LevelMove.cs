using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelMove : MonoBehaviour
{
    public int scenebuildindex;   // Next scene build index
    public int currentLevel;      // Current level number

    [Header("Unlock Settings")]
    public bool unlockNextLevelOnFinish = true;

    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isTransitioning = false;
    public AudioClip Levelcomplete;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(Levelcomplete);

            isTransitioning = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        if (unlockNextLevelOnFinish)
        {
            UnlockNextLevel();
        }

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(scenebuildindex);
    }

    void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}