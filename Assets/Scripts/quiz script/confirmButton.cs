using UnityEngine;
using UnityEngine.UI;

public class confirmButton : MonoBehaviour
{

    public int randomNumber;
    public int correctAnswer;
    public int answer;
    public int score;

    public Image question;

    public TMPro.TextMeshProUGUI option1;
    public TMPro.TextMeshProUGUI option2;
    public TMPro.TextMeshProUGUI option3;
    public TMPro.TextMeshProUGUI option4;

    public TMPro.TextMeshProUGUI scoreEnd;
    public TMPro.TextMeshProUGUI questionNumber;

    public int qNumber = 0;


    public Image options1;
    public Image options2;
    public Image options3;
    public Image options4;

    // reference
    public option4 option4Script;

    public GameObject afterquiz;

    public TMPro.TextMeshProUGUI correctAnswerHere;

    public AudioClip PopUp;

    public void Start()
    {
        randomNumber = Random.Range(1, 5); // para may 1,2,3
        randomizer(); // tawagin agad yung function

        options1.color = Color.blue;
        options2.color = Color.blue;
        options3.color = Color.blue;
        options4.color = Color.blue;

        questionNumber.text = "Question " + qNumber;
        afterquiz.SetActive(false);

    }

    public void confirmButon()
    {
        answer = option4Script.answer;



        if (answer > 0) {


            GetComponent<AudioSource>().PlayOneShot(PopUp);


            qNumber++;
            questionNumber.text = "Question " + qNumber;
            options1.color = Color.blue;
            options2.color = Color.blue;
            options3.color = Color.blue;
            options4.color = Color.blue;


            if (answer == correctAnswer)
        {
            score++;
            Debug.Log("Correct! Your score is: " + score);
            answer = 0;

                randomNumber = Random.Range(1, 5);
                randomizer();



            }
        else
        {
            Debug.Log("Wrong! the correct Answer is " + correctAnswer);
            answer = 0;
                randomNumber = Random.Range(1, 5);
                randomizer();

            }


    } else

        {
            Debug.Log("Please select an answer before confirming.");
        }

        if(qNumber >= 10)
        {
            scoreEnd.text = "Your final score is: " + score + "/10";
            afterquiz.SetActive(true);
        }
}



    // Update is called once per frame
    void Update()
    {
        
    }



    public void randomizer()
    {


        if (randomNumber == 1)
        {
            Sprite question1 = Resources.Load<Sprite>("question1");
            question.sprite = question1;

            correctAnswerHere.text = "1";
             correctAnswer = 1;
            option1.text = "Option 1";
            option2.text = "Option 2";
            option3.text = "Option 3";
            option4.text = "Option 4";



        }
        else if (randomNumber == 2)


        {
            Sprite question1 = Resources.Load<Sprite>("question2");
            question.sprite = question1;

            correctAnswerHere.text = "2";
            correctAnswer = 2;
            option1.text = "Option 12";
            option2.text = "Option 22";
            option3.text = "Option 32";
            option4.text = "Option 42";

        }
        else if (randomNumber == 3)


        {
            Sprite question1 = Resources.Load<Sprite>("question3");
            question.sprite = question1;

            correctAnswerHere.text = "3";
            correctAnswer = 3;
            option1.text = "Option 13";
            option2.text = "Option 23";
            option3.text = "Option 33";
            option4.text = "Option 43";

        }

        else if (randomNumber == 4)


        {
            Sprite question1 = Resources.Load<Sprite>("question4");
            question.sprite = question1;

            correctAnswerHere.text = "4";
            correctAnswer = 4;
            option1.text = "Option 14";
            option2.text = "Option 24";
            option3.text = "Option 34";
            option4.text = "Option 44";

        }




    }
}
