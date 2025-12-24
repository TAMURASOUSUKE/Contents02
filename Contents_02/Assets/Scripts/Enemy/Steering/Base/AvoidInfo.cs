using Unity.Mathematics;
using UnityEngine;

public struct AvoidInfo
{
    public Vector3 normal;
    public float strength;
    public AvoidInfo(Vector3 _normal, float _strength)
    {
        normal = _normal;
        strength = _strength;
    }
}
