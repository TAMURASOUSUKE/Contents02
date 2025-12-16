using UnityEngine;

public class EnemyBlackBoardBase
{
    //Transform
    public Transform trans;
    //RigidBody
    public Rigidbody rb;
    //現在の位置
    public Vector3 pos => rb.position;
    //現在のRBにかかっている力
    public Vector3 vel => rb.linearVelocity;

    //---------------steeringで使う変数---------------
    //目標地点
    public Vector3 targetPos;

    //減速する半径(減速処理の時に使う)
    public float slowRadius = 10.0f;
    //最大速度
    public float maxSpeed;
    //最大加速度
    public float maxAcc;

    //移動の障害物回避に使う正面のスフィアキャストの半径
    public float frontSphereCastRadius;
    //回避強度
    public float dodgeStrength;

    //ステアリング優先度
    public int seekPriority;
    public int arrivePriority;
    public int avoidancePriority;

    //コンストラクタ
    public EnemyBlackBoardBase(SO_EnemyData _data,Rigidbody _body, Transform _trans)
    {
        //SOの初期値を代入
        maxSpeed = _data.maxSpeed;
        maxAcc = _data.maxAcc;

        dodgeStrength = _data.dodgeStrength;
        seekPriority = _data.seekPriority;
        arrivePriority = _data.arrivePriority;
        avoidancePriority = _data.avoidancePriority;

        //RigidBodyを取得
        rb = _body;
        //TransFormを変更
        trans = _trans;
    }
}
