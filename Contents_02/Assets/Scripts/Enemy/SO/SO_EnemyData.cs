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
    [Header("上げると回避の動きが強くなります\n初期値でも正常に動くと思いますが調整が必要な可能性あり")]
    public float dodgeStrength = 1.0f;

    //落下回避の左右角度
    [Header("落下回避時の左右の見る地面の角度")]
    public float fallDodgeRayAngle;

    //下方向に飛ばすレイの長さ
    [Header("どれだけの高さを落下とみなすか")]
    public float fallDodgeRayLen;
    
    //------------------徘徊関係-----------------
    //wanderの円との距離
    [Header("値を大きくすると、徘徊時に、動きが直線的になります")]
    public float wanderDistance;

    //wanderの円の大きさ
    [Header("値を大きくすると、徘徊時に、動きが円形に曲線的になります")]
    public float wanderRadius;

    //wanderの円周上のランダム位置のずれる角度幅
    [Header("値を大きくすると、徘徊時に、ふらつきが大きくなります")]
    public float wanderJitter;


    //ステアリング優先度
    [Header("ステアリング優先度\n基本初期値で動くと思われる")]
    public int seekPriority = 1;
    public int arrivePriority = 1;
    public int wanderPriority = 1;
    public int avoidancePriority = 100;
    public int fallAvoidancePriority = 100;

    //視界関係
    //視野角
    [Header("視界判定するときに視野角")]
    public float fov;
    //視界距離
    [Header("見える距離")]
    public float sensorLen;
}
