﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    public const float k_MORNING=240, k_DAY=15, k_EVENING=135, k_NIGHT=150;

    // hierarchy
    public float brightness_day, brightness_night;

    public static float time = k_DAY;

    void Update()
    {
        time += PauseHandler.paused ? 0 : Time.deltaTime;
        time %= k_MORNING;
        transform.rotation = Quaternion.Euler(Math.Remap(time, 0, k_MORNING, 0, 180), transform.rotation.y, transform.rotation.z);
    }
}