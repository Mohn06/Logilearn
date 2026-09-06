using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class led_positive : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //scripts ng iba
    public led_negative led_Negative;
    public resistor_botLeg Resistor_BotLeg;
    public Battery_negative Battery_Negative;
    public Battery_positive Battery_Positive;

    public GameObject darkRoom;
    public Image led_color;


    public bool leds_positive = false;

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
            leds_positive = true;

        }
        else if (DistanceIncorrect < magnet)
        {
            end.position = incorrect.position;
            leds_positive = false;
        }
        else
        {
            end.anchoredPosition = originalPos;
        }
        UpdateWireStretch();
    }

    public void Update()
    {
        if (leds_positive == true && led_Negative.leds_negative == true 
            && Resistor_BotLeg.resistors_botLeg == true 
            && Battery_Negative.battery_negative == true 
            && Battery_Positive.battery_positive == true)
       
        {
            led_color.color = Color.orange; //led color orange
            Destroy(darkRoom,2);

            Debug.Log("Light up!");

        }

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
