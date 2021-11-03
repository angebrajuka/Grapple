using UnityEngine;

public class GrenadeLauncherShells : MonoBehaviour
{
    // hierarchy
    public GameObject[] shells;
    public Transform cylinder;
    public float rotateSpeed;

    int top = 0;
    int ammo;
    Gun gun;
    float targetRotation;

    void Start()
    {
        ammo = 0;
        gun = GetComponent<Gun>();
        targetRotation = 0;

        foreach(var shell in shells)
        {
            shell.SetActive(false);
        }
    }

    void Update()
    {
        int pAmmo = gun.ammo;
        if(ammo < pAmmo)
        {
            shells[Mathf.Abs((top-1)%4)].SetActive(true);
            targetRotation += 90;
            top -= 1;
        }
        else if(ammo > pAmmo)
        {
            shells[Mathf.Abs(top%4)].SetActive(false);
            targetRotation -= 90;
            top += 1;
        }
        ammo = pAmmo;

        var eulers = cylinder.localEulerAngles;
        eulers.y = Mathf.LerpAngle(eulers.y, targetRotation%360, Time.deltaTime*rotateSpeed);
        cylinder.localEulerAngles = eulers;
    }
}