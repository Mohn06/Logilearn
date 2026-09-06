using UnityEngine;

public class submitScript : MonoBehaviour
{
   public TMPro.TMP_InputField password;
    void Start()
    {
        
    }



    public void submit()
    {
        if (password.text == "01010101")
        {
            Debug.Log("Correct password!");
            // You can add additional actions here, such as loading a new scene or unlocking content.
        }
        else
        {
            Debug.Log("Incorrect password. Try again.");
        }
    }
}
