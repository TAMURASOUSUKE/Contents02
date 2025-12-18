using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyData", menuName = "Scriptable Objects/SO_EnemyData")]
public class SO_EnemyData : ScriptableObject
{

    //最大速度
    [Header("最大速度")]
    public float maxSpeed;
    //最大加速度
    [Header("最大加速度")]
    public float maxAcc = 20.0f;

    //遅くなる半径(減速処理の時に使う)
    [Header("遅くなる半径(減速処理の時に使う)")]
    public float slowRadius = 10.0f;

    //--------------------^--回避関係------------------------------------
    //移動の障害物回避に使う正面のスフィアキャストの半径
    [Header("移動の障害物回避に使う正面のスフィアキャストの半径")]
    public float frontSphereCastRadius;
    //回避強度
    [Header("回避強度")]
    public float dodgeStrength = 1.0f;


    //ステアリング優先度
    [Header("ステアリング優先度")]
    [Header("基本初期値で動くと思われる")]
    public int seekPriority = 1;
    public int arrivePriority = 1;
    public int avoidancePriority = 100;

    //視界関係
    //視野角
    [Header("視野角")]
    public float fov;
    //視界距離
    [Header("視界距離")]
    public float sensorLen;
}
