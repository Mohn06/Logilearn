using UnityEngine;
using UnityEngine.UI;

public class button1Scripts : MonoBehaviour
{

    public qScripts qScript;
    public int numbers;
    public GameObject button0;
    public GameObject button1;


    //q
    public TMPro.TextMeshProUGUI q1;
    public TMPro.TextMeshProUGUI q2;
    public TMPro.TextMeshProUGUI q3;
    public TMPro.TextMeshProUGUI q4;
    public TMPro.TextMeshProUGUI q5;
    public TMPro.TextMeshProUGUI q6;
    public TMPro.TextMeshProUGUI q7;
    public TMPro.TextMeshProUGUI q8;

    private void Start()
    {
      
    }
    void Update()
    {

        numbers = qScript.number;





    }

    public void Button1()
    {
        button0.SetActive(false);
        button1.SetActive(false);
        if (qScript.number == 1)
        {
            q1.text = "1";
        }
        if (qScript.number == 2)
        {
            q2.text = "1";
        }
        if (qScript.number == 3)
        {
            q3.text = "1";
        }
        if (qScript.number == 4)
        {
            q4.text = "1";
        }
        if (qScript.number == 5)
        {
            q5.text = "1";
        }
        if (qScript.number == 6)
        {
            q6.text = "1";
        }
        if (qScript.number == 7)
        {
            q7.text = "1";
        }
        if (qScript.number == 8)
        {
            q8.text = "1";
        }

    }


}
