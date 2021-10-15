using UnityEngine;

public class PlayerEyes : MonoBehaviour
{
    const int defaultLayerMask = ~(Layers.PLAYER_ARMS | Layers.PLAYER | Layers.GRAPPLE_HOOK);

    public static bool Raycast(float range, out Transform hit, Vector3 directionOffset=default(Vector3), int layermask=defaultLayerMask)
    {
        if(directionOffset == default(Vector3)) directionOffset = Vector3.zero;

        hit = null;

        return false;
    }
}