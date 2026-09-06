using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorial", menuName = "Game/Tutorial")]
public class TutorialSO : ScriptableObject
{
    public string tutorialID;
    public string title;

    public TutorialPage[] pages;
}

[System.Serializable]
public class TutorialPage
{
    public Sprite image;

    [TextArea(5, 10)]
    public string description;
}