using UnityEngine;

public class MenuSpin : MonoBehaviour {
    //hierarchy
    public float rotateSpeed;
    public Vector3 pos;
    public Vector3 eul;

    public void Init() {
        PlayerMovement.rb.position = pos;
        PlayerMovement.instance.t_camera.localEulerAngles = eul;
    }

    void Update() {
        if(!MenuHandler.mainMenu) return;
        PlayerMovement.rb.transform.RotateAround(PlayerMovement.rb.transform.position, Vector3.up, Time.deltaTime*rotateSpeed);
    }
}