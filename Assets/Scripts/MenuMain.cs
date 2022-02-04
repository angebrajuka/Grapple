using UnityEngine;

public class MenuMain : MonoBehaviour {
    //hierarchy
    public float rotateSpeed;
    public Vector3 pos;

    void OnEnable() {
        PlayerMovement.rb.position = pos;
    }

    void Update() {
        PlayerMovement.rb.transform.RotateAround(PlayerMovement.rb.transform.position, Vector3.up, Time.deltaTime*rotateSpeed);
    }
}