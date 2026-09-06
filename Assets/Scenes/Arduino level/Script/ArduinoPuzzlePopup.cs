using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ArduinoPuzzlePopup : MonoBehaviour
{
    [Header("Popup")]
    public GameObject popupPanel;
    public TextMeshProUGUI resultText;

    [Header("Wires")]
    public DraggableWireEnd[] wires;

    [Header("Door")]
    public TruthTableDoor door;

    [Header("Lighting")]
    public Light2D globalLight;
    public float correctLightIntensity = 1f;

    [Header("Flicker Effect")]
    public bool useFlicker = true;
    public float flickerDuration = 0.5f;
    public float flickerMinIntensity = 0.1f;
    public float flickerMaxIntensity = 1f;
    public float flickerSpeed = 0.05f;

    [Header("Fade Effect")]
    public bool fadeGlobalLight = true;
    public float globalLightFadeDuration = 1f;

    [Header("Player Light")]
    public Light2D playerLight;
    public bool disablePlayerLightOnSolve = true;
    public bool fadePlayerLight = false;
    public float playerLightFadeDuration = 0.5f;

    [Header("Result Text")]
    public float resultDisplayDuration = 2f;

    [Header("Audio")]
    public AudioClip successClip;
    public float successVolume = 1f;
    private AudioSource audioSource;

    private bool solved = false;
    private Coroutine resultCoroutine;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (resultText != null)
            resultText.text = "";

        // 🔊 Setup AudioSource automatically
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OpenPopup()
    {
        if (solved) return;

        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "";
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void SubmitPuzzle()
    {
        if (solved) return;

        if (wires == null || wires.Length == 0)
        {
            ShowResult("No wires assigned.");
            return;
        }

        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i] == null || !wires[i].IsConnectedCorrectly())
            {
                ShowResult("The wirings are wrong.");
                return;
            }
        }

        solved = true;

        ShowResult("Correct wiring!");

        // 🔊 PLAY SUCCESS SOUND
        if (successClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(successClip, successVolume);
        }

        if (globalLight != null)
            StartCoroutine(LightSequence());

        if (disablePlayerLightOnSolve && playerLight != null)
        {
            if (fadePlayerLight)
                StartCoroutine(FadePlayerLight());
            else
                playerLight.enabled = false;
        }

        if (door != null)
            door.OpenDoor();

        ClosePopup();
    }

    void ShowResult(string message)
    {
        if (resultText == null) return;

        resultText.text = message;

        if (resultCoroutine != null)
            StopCoroutine(resultCoroutine);

        resultCoroutine = StartCoroutine(HideResultAfterDelay());
    }

    IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);

        if (resultText != null)
            resultText.text = "";
    }

    public void ClearPuzzle()
    {
        if (wires != null)
        {
            for (int i = 0; i < wires.Length; i++)
            {
                if (wires[i] != null)
                    wires[i].ResetWire();
            }
        }

        if (resultText != null)
            resultText.text = "";
    }

    IEnumerator LightSequence()
    {
        if (useFlicker)
            yield return StartCoroutine(FlickerLight());

        if (fadeGlobalLight)
            yield return StartCoroutine(FadeGlobalLight());
        else
            globalLight.intensity = correctLightIntensity;
    }

    IEnumerator FlickerLight()
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            timer += flickerSpeed;

            float randomIntensity = Random.Range(flickerMinIntensity, flickerMaxIntensity);
            globalLight.intensity = randomIntensity;

            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    IEnumerator FadeGlobalLight()
    {
        float startIntensity = globalLight.intensity;
        float time = 0f;

        while (time < globalLightFadeDuration)
        {
            time += Time.deltaTime;

            globalLight.intensity = Mathf.Lerp(
                startIntensity,
                correctLightIntensity,
                time / globalLightFadeDuration
            );

            yield return null;
        }

        globalLight.intensity = correctLightIntensity;
    }

    IEnumerator FadePlayerLight()
    {
        float startIntensity = playerLight.intensity;
        float time = 0f;

        while (time < playerLightFadeDuration)
        {
            time += Time.deltaTime;
            playerLight.intensity = Mathf.Lerp(startIntensity, 0f, time / playerLightFadeDuration);
            yield return null;
        }

        playerLight.intensity = 0f;
        playerLight.enabled = false;
    }

    public void AddConnection(string a, string b) { }
    public void RemoveConnection(string a, string b) { }
}