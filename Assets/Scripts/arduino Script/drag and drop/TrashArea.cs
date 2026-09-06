using UnityEngine;

public class TrashArea : MonoBehaviour
{
    public static TrashArea Instance;

    private Collider2D trashCol;

    void Awake()
    {
        Instance = this;
        trashCol = GetComponent<Collider2D>();
    }

    public bool IsInsideTrash(Collider2D targetCol)
    {
        if (trashCol == null || targetCol == null) return false;

        return trashCol.bounds.Intersects(targetCol.bounds);
    }
}