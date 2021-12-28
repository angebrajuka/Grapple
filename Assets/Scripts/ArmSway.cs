using UnityEngine;

public class ArmSway : MonoBehaviour
{
    // hierarchy
    public float amount;
    public float timeScale;

    Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = initialPos+Vector3.up*(Mathf.Sin(Time.time*timeScale))*amount;
    }
}