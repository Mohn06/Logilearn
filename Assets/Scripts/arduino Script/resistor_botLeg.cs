using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class resistor_botLeg : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool resistors_botLeg = false;

    public RectTransform start;
    public RectTransform end;
    public RectTransform stretch;

    public RectTransform correct;
    public RectTransform incorrect;

    public float magnet = 30f;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = end.anchoredPosition;
        UpdateWireStretch();
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
    public void OnBeginDrag(PointerEventData eventData)
    {
        UpdateWireStretch();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        float DistanceCorrect = Vector2.Distance(end.position, correct.position);
        float DistanceIncorrect = Vector2.Distance(end.position, incorrect.position);

        if (DistanceCorrect < magnet)
        {
            end.position = correct.position;
            resistors_botLeg = true;

        }
        else if (DistanceIncorrect < magnet)
        {
            end.position = incorrect.position;
            resistors_botLeg = false;
        }
        else
        {
            end.anchoredPosition = originalPos;
        }
        UpdateWireStretch();


    }

    public void UpdateWireStretch()
    {
        Vector3 middle = (start.position + end.position) / 2f;
        stretch.position = middle;

        float length = Vector3.Distance(start.position, end.position);
        stretch.sizeDelta = new Vector2(length, stretch.sizeDelta.y);

        Vector3 direction = end.position - start.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        stretch.rotation = Quaternion.Euler(0, 0, angle);


    }
}
