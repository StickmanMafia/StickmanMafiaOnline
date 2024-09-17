using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPosition", menuName = "SpawnPositions/SpawnPosition")]
public class SpawnPosition : ScriptableObject
{
    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private Quaternion _rotation;

    [SerializeField]
    private Vector3 _scale;

    public Vector3 Position => _position;
    public Quaternion Rotation => _rotation;
    public Vector3 Scale => _scale;
}