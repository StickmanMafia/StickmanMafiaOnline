using UnityEngine;

public class NicknameRotation : MonoBehaviour
{
    private void FixedUpdate()
    {
        Transform currentCamera = null;
        foreach(Camera cam in Camera.allCameras){
            if(cam.enabled){
                currentCamera = cam.transform;
            }
        }
        transform.LookAt(currentCamera);
        transform.Rotate(0, 180, 0);
    }
}