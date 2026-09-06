using UnityEngine;
using UnityEngine.UI;

public class option4 : MonoBehaviour
{
    public int answer;


    public Image options1;
    public Image options2;
    public Image options3;
    public Image options4;

    public void option1click() { 
        answer = 1;
    options1.color = Color.green;
        options2.color = Color.blue;
        options3.color = Color.blue;
        options4.color = Color.blue;
        Debug.Log("Option 1 clicked");
    }
    public void option2click()
    {
        answer = 2;
        options1.color = Color.blue;
        options2.color = Color.green;
        options3.color = Color.blue;
        options4.color = Color.blue;
        Debug.Log("Option 2 clicked");

    }
    public void option3click()
    {
        answer = 3;
        options1.color = Color.blue;
        options2.color = Color.blue;
        options3.color = Color.green;
        options4.color = Color.blue;
        Debug.Log("Option 3 clicked");

    }
    public void option4click()
    {
        answer = 4;

        options1.color = Color.blue;
        options2.color = Color.blue;
        options3.color = Color.blue;
        options4.color = Color.green;
        Debug.Log("Option 4 clicked");

    }

}
