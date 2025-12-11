using UnityEngine;

public class EnemyBlackBoardBase
{
    //RigidBody
    public Rigidbody rb;
    //現在の位置
    public Vector3 pos => rb.position;
    //現在のRBにかかっている力
    public Vector3 vel => rb.linearVelocity;
    //目標地点
    public Vector3 targetPos;

    //Arriveで減速する半径
    public float slowRadius = 10.0f;
    //最大速度
    public float maxSpeed;
    //最大加速度
    public float maxAcc;

    //コンストラクタ
    public EnemyBlackBoardBase(SO_EnemyData _data,Rigidbody _body)
    {
        //SOの初期値を代入
        maxSpeed = _data.maxSpeed;
        maxAcc = _data.maxAcc;

        //RigidBodyを取得
        rb = _body;
    }
}
