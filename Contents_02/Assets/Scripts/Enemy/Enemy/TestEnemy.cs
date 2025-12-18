using UnityEngine;

public class TestEnemy : EnemyBase
{
    [SerializeField]
    Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ブラックボード生成
        bb = new TestEnemyBB(data, GetComponent<Rigidbody>(), transform);

        //ステアリング各種生成
        seek = new Seek(bb);
        arrive = new Arrive(bb);
        obstacleAvoidance = new ObstacleAvoidance(bb);

        //視界セットアップ
        sensor.SetBB(bb);
    }

    private void Update()
    {
        RotateLookFront();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        steeringManager.AddSteering(seek,1.0f);
        steeringManager.AddSteering(obstacleAvoidance,1.0f);
        Vector3 vec = steeringManager.SteeringCalc(bb);


        Debug.DrawRay(bb.pos, bb.trans.forward);
        Debug.DrawRay(bb.pos, bb.pos + vec,Color.gray);
        Debug.DrawRay(bb.pos, bb.pos + bb.vel,Color.black);

        //最大速度より遅いなら
        if (bb.vel.magnitude < data.maxSpeed)
        {
            //加速度追加
            bb.rb.AddForce(vec, ForceMode.Acceleration);
            //加速度を追加して超えたら補正
            if (bb.vel.magnitude > data.maxSpeed)
            {
                bb.rb.linearVelocity = bb.vel.normalized * data.maxSpeed;
            }
        }
    }
}
