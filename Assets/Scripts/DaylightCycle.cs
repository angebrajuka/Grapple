using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    public const float k_MORNING=240, k_DAY=40, k_EVENING=130, k_NIGHT=150;

    // hierarchy
    public Transform sun;
    public Light lightSource;
    public Color colorDay, colorNight;
    public float brightness_day, brightness_night;
    public float sunriseAngle;

    public static float time = k_DAY;

    void Update() {
        time += PauseHandler.paused ? 0 : Time.deltaTime;
        time %= k_MORNING;
        sun.localRotation = Quaternion.Euler(
            time < k_NIGHT ? Math.Remap(time, 0, k_NIGHT, -sunriseAngle, 180+sunriseAngle) : 270,
            sun.localRotation.y,
            sun.localRotation.z
        );
        lightSource.color = colorDay;
        lightSource.intensity = 
            (time < k_DAY)      ? Math.Remap(time, 0, k_DAY, brightness_night, brightness_day) :
            (time < k_EVENING   ? brightness_day :
            (time < k_NIGHT     ? Math.Remap(time, k_EVENING, k_NIGHT, brightness_day, brightness_night) :
            brightness_night));
    }
}
