using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWireEnd : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Wire Parts")]
    public RectTransform startPoint;
    public RectTransform endPoint;
    public RectTransform wireBody;

    [Header("Clone Wire Parts (for Magnifier)")]
    public RectTransform cloneEndPoint;
    public RectTransform cloneWireBody;

    [Header("Snap")]
    public string startPointID;
    public string requiredTargetID;
    public float snapDistance = 80f;

    [Header("Manager")]
    public ArduinoPuzzlePopup puzzlePopup;

    [Header("Mobile Drag")]
    public bool smoothDrag = true;
    public float dragSmoothSpeed = 20f;

    [Header("Magnifier")]
    public UIPanelMagnifier magnifier;

    private RectTransform parentRect;
    private Vector2 startEndAnchoredPos;
    private ArduinoDropPoint snappedPoint;
    private bool connected = false;

    private Vector2 targetPosition;
    private bool dragging = false;

    void Start()
    {
        if (endPoint != null)
            parentRect = endPoint.parent as RectTransform;

        if (endPoint != null)
        {
            startEndAnchoredPos = endPoint.anchoredPosition;
            targetPosition = startEndAnchoredPos;
        }

        UpdateWireVisual();

        if (magnifier != null)
        {
            magnifier.ClearFollowTarget();
            magnifier.ClearWireTargets();
            magnifier.Hide();
        }
    }

    void Update()
    {
        if (dragging && smoothDrag && endPoint != null)
        {
            endPoint.anchoredPosition = Vector2.Lerp(
                endPoint.anchoredPosition,
                targetPosition,
                Time.deltaTime * dragSmoothSpeed
            );

            UpdateWireVisual();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;

        if (connected && snappedPoint != null)
        {
            if (puzzlePopup != null)
                puzzlePopup.RemoveConnection(startPointID, snappedPoint.pointID);

            connected = false;
            snappedPoint = null;
        }

        if (magnifier != null && endPoint != null)
        {
            magnifier.SetFollowTarget(endPoint);

            // Tell magnifier which real wire and clone wire to sync
            magnifier.SetWireTargets(endPoint, wireBody, cloneEndPoint, cloneWireBody);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                endPoint.position
            );

            magnifier.Show(screenPos);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null || endPoint == null)
            return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            if (smoothDrag)
            {
                targetPosition = localPoint;
            }
            else
            {
                endPoint.anchoredPosition = localPoint;
                UpdateWireVisual();
            }
        }

        if (magnifier != null && endPoint != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                endPoint.position
            );

            magnifier.Move(screenPos);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        if (smoothDrag)
        {
            endPoint.anchoredPosition = targetPosition;
            UpdateWireVisual();
        }

        ArduinoDropPoint[] allPoints = Object.FindObjectsByType<ArduinoDropPoint>(FindObjectsSortMode.None);

        ArduinoDropPoint nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < allPoints.Length; i++)
        {
            RectTransform pointRect = allPoints[i].GetComponent<RectTransform>();
            if (pointRect == null) continue;

            float dist = Vector2.Distance(endPoint.position, pointRect.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = allPoints[i];
            }
        }

        if (nearest != null && nearestDist <= snapDistance)
        {
            RectTransform pointRect = nearest.GetComponent<RectTransform>();

            Vector2 snappedLocalPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, pointRect.position),
                eventData.pressEventCamera,
                out snappedLocalPoint))
            {
                endPoint.anchoredPosition = snappedLocalPoint;
                targetPosition = snappedLocalPoint;
            }

            snappedPoint = nearest;
            connected = true;

            if (puzzlePopup != null)
                puzzlePopup.AddConnection(startPointID, snappedPoint.pointID);
        }
        else
        {
            endPoint.anchoredPosition = startEndAnchoredPos;
            targetPosition = startEndAnchoredPos;
            connected = false;
            snappedPoint = null;
        }

        UpdateWireVisual();

        if (magnifier != null)
        {
            magnifier.ClearFollowTarget();
            magnifier.ClearWireTargets();
            magnifier.Hide();
        }
    }

    void UpdateWireVisual()
    {
        if (startPoint == null || endPoint == null || wireBody == null)
            return;

        Vector2 start = startPoint.anchoredPosition;
        Vector2 end = endPoint.anchoredPosition;

        Vector2 dir = end - start;
        float length = dir.magnitude;

        wireBody.anchoredPosition = start + dir * 0.5f;
        wireBody.sizeDelta = new Vector2(length, wireBody.sizeDelta.y);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        wireBody.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    public void ResetWire()
    {
        connected = false;
        snappedPoint = null;
        dragging = false;

        if (endPoint != null)
            endPoint.anchoredPosition = startEndAnchoredPos;

        targetPosition = startEndAnchoredPos;

        UpdateWireVisual();

        if (magnifier != null)
        {
            magnifier.ClearFollowTarget();
            magnifier.ClearWireTargets();
            magnifier.Hide();
        }
    }

    public bool IsConnectedCorrectly()
    {
        return connected && snappedPoint != null && snappedPoint.pointID == requiredTargetID;
    }
}