using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyData", menuName = "Scriptable Objects/SO_EnemyData")]
public class SO_EnemyData : ScriptableObject
{
    //最大速度
    public float maxSpeed;
    //最大加速度
    public float maxAcc = 20.0f;
    
    //遅くなる半径(減速処理の時に使う)
    public float slowRadius = 10.0f;

    //移動の障害物回避に使う正面のスフィアキャストの半径
    public float frontSphereCastRadius;
    //回避強度
    public float dodgeStrength = 1.0f;

    //ステアリング優先度
    public int seekPriority;
    public int arrivePriority;
    public int avoidancePriority;
}
