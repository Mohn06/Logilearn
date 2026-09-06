using UnityEngine;

public class SandboxReset : MonoBehaviour
{
    public void DeleteAllPlacedObjects()
    {
        // Delete gates / switches / lamps
        GameObject[] placedObjects = GameObject.FindGameObjectsWithTag("Placeable");

        for (int i = 0; i < placedObjects.Length; i++)
        {
            Destroy(placedObjects[i]);
        }

        // Delete wires
        WireConnection[] wires = FindObjectsByType<WireConnection>(FindObjectsSortMode.None);

        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i] != null)
                Destroy(wires[i].gameObject);
        }

        Debug.Log("Sandbox Reset Complete");
    }
}