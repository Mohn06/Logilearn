using UnityEngine;

public class BoardArea : MonoBehaviour
{
    public static BoardArea Instance;

    private BoxCollider2D boardCollider;

    void Awake()
    {
        Instance = this;
        boardCollider = GetComponent<BoxCollider2D>();
    }

    public Vector3 ClampToBoard(Vector3 worldPosition, Bounds objectBounds)
    {
        if (boardCollider == null) return worldPosition;

        Bounds boardBounds = boardCollider.bounds;

        float halfWidth = objectBounds.extents.x;
        float halfHeight = objectBounds.extents.y;

        float clampedX = Mathf.Clamp(
            worldPosition.x,
            boardBounds.min.x + halfWidth,
            boardBounds.max.x - halfWidth
        );

        float clampedY = Mathf.Clamp(
            worldPosition.y,
            boardBounds.min.y + halfHeight,
            boardBounds.max.y - halfHeight
        );

        return new Vector3(clampedX, clampedY, 0f);
    }
}