using UnityEngine;
using UnityEngine.Playables;

public class Level1IntroController : MonoBehaviour
{
    private PlayableDirector director;

    private const string key = "Level1IntroPlayed";

    void Awake()
    {
        director = GetComponent<PlayableDirector>();

        if (director == null)
        {
            Debug.LogError("PlayableDirector not found!");
            return;
        }

        if (PlayerPrefs.GetInt(key, 0) == 1)
        {
            // Already played → skip timeline completely
            director.time = director.duration;
            director.Evaluate();
            director.Stop();

            Debug.Log("Intro skipped");
        }
        else
        {
            // First time → play intro
            director.time = 0;
            director.Play();

            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();

            Debug.Log("Intro played first time");
        }
    }
}