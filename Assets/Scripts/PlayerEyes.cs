using UnityEngine;

public class PlayerEyes : MonoBehaviour
{
    const int defaultLayerMask = (Layers.PLAYER_ARMS | Layers.PLAYER | Layers.GRAPPLE_HOOK);

    public static bool Raycast(out RaycastHit hit, float range=Mathf.Infinity, Vector3 directionOffset=default(Vector3), int layermask=defaultLayerMask)
    {
        if(directionOffset == default(Vector3)) directionOffset = Vector3.zero;

        return Physics.Raycast(PlayerMovement.instance.t_camera.position, PlayerMovement.instance.t_camera.TransformDirection(Vector3.forward), out hit, range, layermask);
    }
}