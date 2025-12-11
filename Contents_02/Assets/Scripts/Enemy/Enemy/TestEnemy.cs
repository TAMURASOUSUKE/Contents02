using UnityEngine;
using UnityEngine.Rendering;

public class TestEnemy : EnemyBase
{
    [SerializeField]
    Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bb = new TestEnemyBB(data, GetComponent<Rigidbody>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bb.targetPos = target.position;
        steeringManager.AddSteering(seek);
        Vector3 vec = steeringManager.SteeringCalc(bb);
        

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
