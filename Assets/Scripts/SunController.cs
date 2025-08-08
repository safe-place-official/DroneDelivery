using UnityEngine;
using System;

public class SunController : MonoBehaviour
{
    public Light sunLight; // Перетяни сюда Directional Light
    public float latitude = 55.0f; // Широта (например, Москва)

    void Update()
    {
        DateTime now = DateTime.UtcNow;
        float hours = now.Hour + now.Minute / 60f;

        float sunAngle = Mathf.Lerp(-90, 90, hours / 24f);
        float tilt = latitude - 23.5f; // Наклон солнца от широты

        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 180, tilt);
    }
}