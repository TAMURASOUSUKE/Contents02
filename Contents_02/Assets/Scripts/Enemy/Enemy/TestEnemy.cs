using UnityEngine;

public class TestEnemy : EnemyBase
{
    [SerializeField]
    Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bb = new TestEnemyBB(data, GetComponent<Rigidbody>(), transform);

        seek = new Seek(bb);
        arrive = new Arrive(bb);
        obstacleAvoidance = new ObstacleAvoidance(bb);
    }

    private void Update()
    {
        RotateLookFront();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bb.targetPos = target.position;
        steeringManager.AddSteering(seek,1.0f);
        steeringManager.AddSteering(obstacleAvoidance,1.0f);
        Vector3 vec = steeringManager.SteeringCalc(bb);


        Debug.DrawRay(bb.pos, bb.trans.forward);
        Debug.DrawRay(bb.pos, bb.pos + vec,Color.gray);
        Debug.DrawRay(bb.pos, bb.pos + bb.vel,Color.black);

        //ç≈ëÂë¨ìxÇÊÇËíxÇ¢Ç»ÇÁ
        if (bb.vel.magnitude < data.maxSpeed)
        {
            //â¡ë¨ìxí«â¡
            bb.rb.AddForce(vec, ForceMode.Acceleration);
            //â¡ë¨ìxÇí«â¡ÇµÇƒí¥Ç¶ÇΩÇÁï‚ê≥
            if (bb.vel.magnitude > data.maxSpeed)
            {
                bb.rb.linearVelocity = bb.vel.normalized * data.maxSpeed;
            }
        }
    }
}
