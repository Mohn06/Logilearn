using UnityEngine;

public class UIPanelMagnifier : MonoBehaviour
{
    [Header("References")]
    public RectTransform magnifierRoot;
    public RectTransform magnifiedContent;
    public RectTransform sourcePanel;
    public RectTransform sourcePanelClone;

    [Header("Zoom")]
    public float zoom = 2f;

    [Header("Position")]
    public Vector2 screenOffset = new Vector2(120f, 120f);

    [Header("Canvas")]
    public Canvas canvas;

    [Header("Follow")]
    public RectTransform followTarget;

    [Header("Wire Sync")]
    public RectTransform originalWireEnd;
    public RectTransform cloneWireEnd;
    public RectTransform originalWireBody;
    public RectTransform cloneWireBody;

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (magnifierRoot == null || !magnifierRoot.gameObject.activeSelf)
            return;

        if (followTarget != null)
            MoveToTarget();

        SyncWireVisual();
    }

    public void Show(Vector2 screenPosition)
    {
        if (magnifierRoot != null)
            magnifierRoot.gameObject.SetActive(true);

        if (followTarget != null)
            MoveToTarget();
        else
            Move(screenPosition);

        SyncWireVisual();
    }

    public void Hide()
    {
        if (magnifierRoot != null)
            magnifierRoot.gameObject.SetActive(false);
    }

    public void SetFollowTarget(RectTransform target)
    {
        followTarget = target;
    }

    public void ClearFollowTarget()
    {
        followTarget = null;
    }

    public void SetWireTargets(RectTransform realEnd, RectTransform realBody, RectTransform copyEnd, RectTransform copyBody)
    {
        originalWireEnd = realEnd;
        originalWireBody = realBody;
        cloneWireEnd = copyEnd;
        cloneWireBody = copyBody;
    }

    public void ClearWireTargets()
    {
        originalWireEnd = null;
        originalWireBody = null;
        cloneWireEnd = null;
        cloneWireBody = null;
    }

    public void MoveToTarget()
    {
        if (followTarget == null)
            return;

        Camera cam = null;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, followTarget.position);
        Move(screenPos);
    }

    public void Move(Vector2 screenPosition)
    {
        if (magnifierRoot == null || sourcePanel == null || magnifiedContent == null || sourcePanelClone == null || canvas == null)
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera cam = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        Vector2 magnifierLocalPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition + screenOffset,
            cam,
            out magnifierLocalPos))
        {
            magnifierRoot.anchoredPosition = magnifierLocalPos;
        }

        Vector2 localPointInSource;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            sourcePanel,
            screenPosition,
            cam,
            out localPointInSource))
        {
            sourcePanelClone.localScale = new Vector3(zoom, zoom, 1f);
            sourcePanelClone.anchoredPosition = -localPointInSource * zoom;
        }

        SyncWireVisual();
    }

    void SyncWireVisual()
    {
        if (originalWireEnd != null && cloneWireEnd != null)
        {
            cloneWireEnd.anchoredPosition = originalWireEnd.anchoredPosition;
            cloneWireEnd.localEulerAngles = originalWireEnd.localEulerAngles;
            cloneWireEnd.localScale = originalWireEnd.localScale;
        }

        if (originalWireBody != null && cloneWireBody != null)
        {
            cloneWireBody.anchoredPosition = originalWireBody.anchoredPosition;
            cloneWireBody.sizeDelta = originalWireBody.sizeDelta;
            cloneWireBody.localEulerAngles = originalWireBody.localEulerAngles;
            cloneWireBody.localScale = originalWireBody.localScale;
        }
    }
}