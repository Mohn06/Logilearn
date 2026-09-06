using UnityEngine;

public class DestroyConnectedWires : MonoBehaviour
{
    void OnDestroy()
    {
        GateSocket[] sockets = GetComponentsInChildren<GateSocket>(true);

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null) continue;

            if (sockets[i].currentWire != null)
            {
                WireConnection wire = sockets[i].currentWire;

                // Prevent duplicate destroy attempts
                sockets[i].currentWire = null;

                if (wire != null)
                    wire.DisconnectAndDestroy();
            }

            sockets[i].connectedSocket = null;
        }
    }
}