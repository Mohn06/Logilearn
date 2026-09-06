using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform start;
    public RectTransform end;
    public RectTransform stretch;

    public float magnet = 40f;

    private Vector2 originalPos;

    private RectTransform currentConnection;

    public GameObject wirePrefab;

    private bool hasSpawnedNewWire = false;

    private Port startPort;
    private Port endPort;

    private ILogicInteract sourceLogic;
    private ILogicInteract targetLogic;

    void Start()
    {
        originalPos = end.anchoredPosition;

        startPort = start.GetComponent<Port>();

        if (startPort != null)
            sourceLogic = startPort.logicNode;

        UpdateWireStretch();
    }

    void Update()
    {
        if (currentConnection != null)
        {
            end.position = currentConnection.position;
            UpdateWireStretch();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentConnection = null;
        endPort = null;
        targetLogic = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 outPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)end.parent,
            eventData.position,
            eventData.pressEventCamera,
            out outPos
        );

        end.anchoredPosition = outPos;

        UpdateWireStretch();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Port nearestPort = FindNearestPort();

        if (nearestPort != null && IsValidConnection(startPort, nearestPort))
        {
            currentConnection = nearestPort.GetComponent<RectTransform>();
            end.position = nearestPort.transform.position;

            endPort = nearestPort;

            targetLogic = endPort.logicNode;

            ConnectLogic();

            SpawnNewWire();
        }
        else
        {
            currentConnection = null;
            end.anchoredPosition = originalPos;
        }

        UpdateWireStretch();
    }

    Port FindNearestPort()
    {
        Port[] ports = FindObjectsByType<Port>(FindObjectsSortMode.None);

        Port nearest = null;
        float minDist = magnet;

        foreach (Port p in ports)
        {
            if (p == startPort) continue; // ignore same port

            float dist = Vector2.Distance(end.position, p.transform.position);

            if (dist < minDist)
            {
                nearest = p;
                minDist = dist;
            }
        }

        return nearest;
    }

    bool IsValidConnection(Port a, Port b)
    {
        if (a == null || b == null)
            return false;

        if (a == b)
            return false;

        if (a.portType == Port.PortType.Output &&
            b.portType == Port.PortType.Input)
            return true;

        return false;
    }

    void ConnectLogic()
    {
        if (sourceLogic == null || targetLogic == null)
            return;

        targetLogic.SetInput(sourceLogic.Output, endPort.inputIndex);
    }

    void SpawnNewWire()
    {
        if (hasSpawnedNewWire)
            return;

        GameObject newWire = Instantiate(wirePrefab, transform.parent.parent);

        RectTransform newStart = newWire.transform.Find("Start").GetComponent<RectTransform>();
        RectTransform newEnd = newWire.transform.Find("End").GetComponent<RectTransform>();
        RectTransform newStretch = newWire.transform.Find("Stretch").GetComponent<RectTransform>();

        WireDrag script = newStretch.GetComponent<WireDrag>();

        script.start = newStart;
        script.end = newEnd;
        script.stretch = newStretch;
        script.wirePrefab = wirePrefab;

        hasSpawnedNewWire = true;
    }

    public void UpdateWireStretch()
    {
        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;

        Vector2 middle = (startPos + endPos) / 2f;
        stretch.anchoredPosition = middle;

        float length = Vector2.Distance(startPos, endPos);
        stretch.sizeDelta = new Vector2(length, stretch.sizeDelta.y);

        Vector2 dir = endPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        stretch.localRotation = Quaternion.Euler(0, 0, angle);
    }
}