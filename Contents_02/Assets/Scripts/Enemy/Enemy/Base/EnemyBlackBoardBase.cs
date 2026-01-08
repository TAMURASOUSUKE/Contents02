using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBlackBoardBase
{
    // Transform
    public Transform trans;
    // RigidBody
    public Rigidbody rb;
    // 現在の位置
    public Vector3 pos => rb.position;
    // 現在のRBにかかっている力
    public Vector3 vel => rb.linearVelocity;

    //---------------steeringで使う変数---------------
    // ステアリングマネージャー
    public SteeringManager steeringManager = new SteeringManager();

    // ステアリング各種
    public Seek seek;
    public Arrive arrive;
    public Wander wander;
    public ObstacleAvoidance obstacleAvoidance;
    public FallAvoidance fallAvoidance;
    //パスフォロー
    public PathFollow pathFollow;

    // 目標地点
    public Transform target = null;

    // 移動目標
    public Vector3? moveTarget = null;

    // 減速する半径(減速処理の時に使う)
    public float slowRadius = 10.0f;
    // 最大速度
    public float maxSpeed;
    // 最大加速度
    public float maxAcc;

    //停止距離
    public float stopDistance;

    // -----障害物回避------
    // 移動の障害物回避に使う正面のスフィアキャストの半径
    public float frontSphereCastRadius;
    // 回避強度
    public float dodgeStrength;

    // -------落下回避-------
    // 落下回避の左右角度
    public Quaternion fallDodgeRayOffsetRotR;
    public Quaternion fallDodgeRayOffsetRotL;
    // 下方向に飛ばすレイの長さ
    public float fallDodgeRayLen;

    // ------WANDER--------
    // wanderの円との距離
    public float wanderDistance;

    // wanderの円の大きさ
    public float wanderRadius;

    // wanderの円周上のランダム位置のずれる角度幅
    public float wanderJitter;

    // ステアリング優先度
    public int seekPriority;
    public int arrivePriority;
    public int wanderPriority;
    public int avoidancePriority;
    public int fallAvoidancePriority;

    // -------------視覚センサーで使うもの----------------
    // 視野角
    public float fov;
    // 視界距離
    public float sensorLen;

    // -----------RVO-------------
    // RVOリスト
    public List<IRvoObj> rvoList = new List<IRvoObj>();

    // RVOで使う自身の大きさ
    public float rvoRadius;

    // コンストラクタ
    public EnemyBlackBoardBase(SO_EnemyData _data, Rigidbody _body, Transform _trans)
    {
        // SOから初期値を代入、計算
        // 減速
        slowRadius = _data.slowRadius;
        // 最大値
        maxSpeed = _data.maxSpeed;
        maxAcc = _data.maxAcc;
        stopDistance = _data.stopDistance;
        
        // 回避系
        frontSphereCastRadius = _data.frontSphereCastRadius;
        dodgeStrength = _data.dodgeStrength;
        fallDodgeRayOffsetRotR = Quaternion.AngleAxis(_data.fallDodgeRayAngle, Vector3.up);
        fallDodgeRayOffsetRotL = Quaternion.AngleAxis(-_data.fallDodgeRayAngle, Vector3.up);
        fallDodgeRayLen = _data.fallDodgeRayLen;

        // 徘徊系
        wanderDistance = _data.wanderDistance;
        wanderRadius = _data.wanderRadius;
        wanderJitter = _data.wanderJitter;

        // 優先度
        seekPriority = _data.seekPriority;
        arrivePriority = _data.arrivePriority;
        wanderPriority = _data.wanderPriority;
        avoidancePriority = _data.avoidancePriority;
        fallAvoidancePriority = _data.fallAvoidancePriority;

        // ステアリング各種生成
        //優先度を決めるためにbbを渡している
        seek = new Seek(this);
        arrive = new Arrive(this);
        wander = new Wander(this);
        obstacleAvoidance = new ObstacleAvoidance(this);
        fallAvoidance = new FallAvoidance(this);
        // パスフォロー
        //巡回ルートの初期化も
        pathFollow = new PathFollow(_data.route);

        // センサー系
        fov = _data.fov;
        sensorLen = _data.sensorLen;

        // RVO系
        rvoRadius = _data.rvoRadius;

        // RigidBodyを取得
        rb = _body;
        // TransFormを変更
        trans = _trans;
    }
}
