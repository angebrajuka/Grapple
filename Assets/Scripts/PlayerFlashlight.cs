using UnityEngine;

public class PlayerFlashlight : MonoBehaviour {
    // hierarchy
    public Light pointLight;
    public Light spotLight;
    public float range;

    bool on=false;

    void Update() {
        if(PlayerInput.GetKeyDown("flashlight")) {
            on = !on;
            spotLight.enabled = on;
            pointLight.enabled = on;
        }

        if(!on) return;
        if(PlayerEyes.Raycast(out RaycastHit hit, out Vector3 direction, range)) {
            pointLight.enabled = true;
            pointLight.intensity = Math.Remap(hit.distance, 0, range, 2, 0);
            pointLight.transform.position = hit.point-direction;
        } else {
            pointLight.enabled = false;
        }
    }
}