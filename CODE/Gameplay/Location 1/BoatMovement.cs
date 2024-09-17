using UnityEngine;
using UnityEngine.Serialization;

public class BoatMovement : MonoBehaviour
{
    [FormerlySerializedAs("_speedIncrement")] [SerializeField] 
    private float _targetSpeedIncrement;

    private float _actualSpeedIncrement;

    private bool _speedingUp;
    
    public void SpeedUp() => _speedingUp = true;

    private void Start()
    {
        _speedingUp = false;
        _actualSpeedIncrement = 0;
    }

    private void Update()
    {
        if (_speedingUp && _actualSpeedIncrement < _targetSpeedIncrement)
        {
            _actualSpeedIncrement += 0.005f;
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_actualSpeedIncrement == _targetSpeedIncrement)
                _speedingUp = false;
        }
        else
        {
            _speedingUp = false;
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, _actualSpeedIncrement / 20);
        transform.position += transform.up * (_actualSpeedIncrement / 10);
    }
}