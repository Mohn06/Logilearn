using UnityEngine;
using UnityEngine.UI;

public class LogicGatePagedPopup : MonoBehaviour
{
    public static LogicGatePagedPopup Instance;

    [Header("UI")]
    public Canvas canvas;
    public GameObject panel;
    public Image infoImage;
    public Button leftButton;
    public Button rightButton;

    private Animator animator;
    private CanvasGroup canvasGroup;

    private Sprite[] pages;
    private int currentPage;

    void Awake()
    {
        Instance = this;

        animator = canvas.GetComponent<Animator>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        // Initial state
        panel.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        leftButton.onClick.AddListener(PrevPage);
        rightButton.onClick.AddListener(NextPage);
    }

    public void Show(LogicGateInfoPages info)
    {
        pages = new Sprite[]
        {
           
            info.truthTablePage,
            info.booleanExpressionPage,
          
        };

        currentPage = 0;
        UpdatePage();

        panel.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        animator.SetBool("isOpen", true);
    }

    public void Hide()
    {
        animator.SetBool("isOpen", false);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    // Called by animation event (recommended)
    public void OnCloseAnimationFinished()
    {
        panel.SetActive(false);
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    void UpdatePage()
    {
        infoImage.sprite = pages[currentPage];

        leftButton.interactable = currentPage > 0;
        rightButton.interactable = currentPage < pages.Length - 1;
    }
}
