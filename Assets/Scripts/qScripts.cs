using UnityEngine;
using UnityEngine.UI;

public class qScripts : MonoBehaviour
{
    public int number;
    public GameObject button1;
    public GameObject button2;


    public void Start()
    {
        number = 0;
        button2.SetActive(false);
        button1.SetActive(false);

    }



    public void number1()
    {
               number = 1;

        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number2()
    {
        number = 2;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number3()
    {
        number = 3;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number4()
    {
        number = 4;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number5()
    {
        number = 5;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number6()
    {
        number = 6;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number7()
    {
        number = 7;
        button2.SetActive(true);
        button1.SetActive(true);
    }
    public void number8()
    {
        number = 8;
        button2.SetActive(true);
        button1.SetActive(true);
    }
}
