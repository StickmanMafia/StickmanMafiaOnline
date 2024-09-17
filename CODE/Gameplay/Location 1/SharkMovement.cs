using UnityEngine;

public class SharkMovement : MonoBehaviour
{
    private const float RotationAngle = 0.14f;
    private const float SpeedIncrement = 2f;
    
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * (Time.deltaTime * SpeedIncrement));
        transform.Rotate(0, RotationAngle * SpeedIncrement, 0);
    }
}