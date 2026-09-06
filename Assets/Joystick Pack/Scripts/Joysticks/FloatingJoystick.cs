using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingJoystick : Joystick
{
    private Image bgImage;
    private Image handleImage;

    private Vector2 startPosition; // original position

    protected override void Start()
    {
        base.Start();

        background.gameObject.SetActive(true);

        // Save original position
        startPosition = background.anchoredPosition;

        // Get Images
        bgImage = background.GetComponent<Image>();
        handleImage = handle.GetComponent<Image>();

        // Idle opacity
        SetOpacity(0.3f);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // Move joystick to touch position
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

        // Full opacity when touching
        SetOpacity(1f);

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // Return to original position
        background.anchoredPosition = startPosition;

        // Back to idle opacity
        SetOpacity(0.3f);

        base.OnPointerUp(eventData);
    }

    void SetOpacity(float alpha)
    {
        if (bgImage != null)
        {
            Color c = bgImage.color;
            c.a = alpha;
            bgImage.color = c;
        }

        if (handleImage != null)
        {
            Color c = handleImage.color;
            c.a = alpha;
            handleImage.color = c;
        }
    }
}