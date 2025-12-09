using UnityEngine;
using UnityEngine.Rendering;

public class TestEnemy : EnemyBase
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float slowRadius = 10.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vec =
        Steering.Arrive(target.position, transform.position, body.linearVelocity, data.maxSpeed, slowRadius);

        //ç≈ëÂë¨ìxÇÊÇËíxÇ¢Ç»ÇÁ
        if (body.linearVelocity.magnitude < data.maxSpeed)
        {
            //â¡ë¨ìxí«â¡
            body.AddForce(vec);
            //â¡ë¨ìxÇí«â¡ÇµÇƒí¥Ç¶ÇΩÇÁï‚ê≥
            if (body.linearVelocity.magnitude > data.maxSpeed)
            {
                body.linearVelocity = body.linearVelocity.normalized * data.maxSpeed;
            }
        }
    }
}
