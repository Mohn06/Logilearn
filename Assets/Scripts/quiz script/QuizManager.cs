using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Question
{
    public string questionText;

    public string option1;
    public string option2;
    public string option3;
    public string option4;

    public int correctAnswer;

    [Header("Optional Image")]
    public Sprite questionImage;
}

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;

    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;
    public TextMeshProUGUI option4Text;

    public Image option1Image;
    public Image option2Image;
    public Image option3Image;
    public Image option4Image;

    [Header("Question Image")]
    public Image questionImageUI;

    [Header("Quiz UI")]
    public TextMeshProUGUI questionNumberText;
    public TextMeshProUGUI scoreEndText;
    public GameObject afterQuizPanel;

    [Header("Audio")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip buttonSFX;

    [Header("Questions")]
    public List<Question> questions = new List<Question>();

    [Header("Quiz Settings")]
    public float delayBeforeNextQuestion = 1.5f;
    public bool shuffleQuestions = true;
    public int maxQuestionsToAsk = 10;
    public int requiredCorrectAnswers = 7;

    [Header("Progression Unlock Settings")]
    public int levelToUnlock = 10;

    [Header("Result Messages")]
    [TextArea] public string passMessage = "Level unlocked!";
    [TextArea] public string failMessage = "You did not get enough correct answers.";

    private bool isProcessing = false;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private int selectedAnswer = 0;

    private Color defaultColor1;
    private Color defaultColor2;
    private Color defaultColor3;
    private Color defaultColor4;

    void Start()
    {
        defaultColor1 = option1Image.color;
        defaultColor2 = option2Image.color;
        defaultColor3 = option3Image.color;
        defaultColor4 = option4Image.color;

        currentQuestionIndex = 0;
        score = 0;
        selectedAnswer = 0;
        isProcessing = false;

        if (afterQuizPanel != null)
            afterQuizPanel.SetActive(false);

        if (shuffleQuestions)
            ShuffleQuestions();

        LoadQuestion();
        UpdateQuestionNumber();
    }

    public void RetryQuiz()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MAIN MENU");
    }

    void LoadQuestion()
    {
        if (questions.Count == 0)
            return;

        if (currentQuestionIndex >= questions.Count)
            return;

        Question q = questions[currentQuestionIndex];

        questionText.text = q.questionText;
        option1Text.text = q.option1;
        option2Text.text = q.option2;
        option3Text.text = q.option3;
        option4Text.text = q.option4;

        if (questionImageUI != null)
        {
            if (q.questionImage != null)
            {
                questionImageUI.sprite = q.questionImage;
                questionImageUI.gameObject.SetActive(true);
            }
            else
            {
                questionImageUI.sprite = null;
                questionImageUI.gameObject.SetActive(false);
            }
        }

        ResetOptionColors();
        selectedAnswer = 0;
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            Question temp = questions[i];
            int randomIndex = Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(buttonSFX);

        if (isProcessing)
            return;

        selectedAnswer = answerIndex;

        ResetOptionColors();

        switch (answerIndex)
        {
            case 1:
                option1Image.color = Color.yellow;
                break;
            case 2:
                option2Image.color = Color.yellow;
                break;
            case 3:
                option3Image.color = Color.yellow;
                break;
            case 4:
                option4Image.color = Color.yellow;
                break;
        }
    }

    public void ConfirmAnswer()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(buttonSFX);

        if (selectedAnswer == 0 || isProcessing)
            return;

        if (currentQuestionIndex >= questions.Count)
            return;

        isProcessing = true;

        Question q = questions[currentQuestionIndex];

        HighlightCorrectAnswer(q.correctAnswer);

        if (selectedAnswer != q.correctAnswer)
        {
            HighlightWrongAnswer(selectedAnswer);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongSound);

            if (ToastMessage.Instance != null)
                ToastMessage.Instance.ShowToast("Wrong! :(", 1f);
        }
        else
        {
            score++;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctSound);

            if (ToastMessage.Instance != null)
                ToastMessage.Instance.ShowToast("Correct! :)", 1f);
        }

        StartCoroutine(NextQuestionAfterDelay());
    }

    void HighlightCorrectAnswer(int correctIndex)
    {
        switch (correctIndex)
        {
            case 1:
                option1Image.color = Color.green;
                break;
            case 2:
                option2Image.color = Color.green;
                break;
            case 3:
                option3Image.color = Color.green;
                break;
            case 4:
                option4Image.color = Color.green;
                break;
        }
    }

    void HighlightWrongAnswer(int wrongIndex)
    {
        switch (wrongIndex)
        {
            case 1:
                option1Image.color = Color.red;
                break;
            case 2:
                option2Image.color = Color.red;
                break;
            case 3:
                option3Image.color = Color.red;
                break;
            case 4:
                option4Image.color = Color.red;
                break;
        }
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextQuestion);

        currentQuestionIndex++;

        int totalQuestionsThisRun = Mathf.Min(maxQuestionsToAsk, questions.Count);

        if (currentQuestionIndex >= totalQuestionsThisRun)
        {
            EndQuiz(totalQuestionsThisRun);
            yield break;
        }

        UpdateQuestionNumber();
        LoadQuestion();
        isProcessing = false;
    }

    void EndQuiz(int totalQuestionsThisRun)
    {
        bool passed = score >= requiredCorrectAnswers;

        if (passed)
        {
            UnlockLevelFromQuiz();
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(buttonSFX);

        if (afterQuizPanel != null)
            afterQuizPanel.SetActive(true);

        if (scoreEndText != null)
        {
            scoreEndText.text =
                "Your final score is: " + score + "/" + totalQuestionsThisRun +
                "\n\n" +
                (passed ? passMessage : failMessage);
        }
    }

    void UnlockLevelFromQuiz()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Same progression style as your LevelMove script
        if (levelToUnlock > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelToUnlock);
            PlayerPrefs.Save();
        }
    }

    void ResetOptionColors()
    {
        option1Image.color = defaultColor1;
        option2Image.color = defaultColor2;
        option3Image.color = defaultColor3;
        option4Image.color = defaultColor4;
    }

    void UpdateQuestionNumber()
    {
        int totalQuestionsThisRun = Mathf.Min(maxQuestionsToAsk, questions.Count);
        questionNumberText.text = "Question " + (currentQuestionIndex + 1) + "/" + totalQuestionsThisRun;
    }
}