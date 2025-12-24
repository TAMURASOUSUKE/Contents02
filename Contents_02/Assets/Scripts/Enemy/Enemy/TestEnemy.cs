using UnityEngine;

public class TestEnemy : EnemyBase<TestEnemyBB>
{
    [SerializeField]
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ブラックボード生成
        bb = new TestEnemyBB(data, GetComponent<Rigidbody>(), transform);
        //ステート生成
        state = new TestEnemyPatrolState();

        //ステアリング各種生成
        bb.seek = new Seek(bb);
        bb.arrive = new Arrive(bb);
        bb.wander = new Wander(bb);
        bb.obstacleAvoidance = new ObstacleAvoidance(bb);
        bb.fallAvoidance = new FallAvoidance(bb);

        //視界セットアップ
        if(sensor != null)
        sensor.SetBB(bb);

        bb.target = target;
    }

    private void Update()
    {
        RotateLookFront();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        state.StateUpdate(bb);
    }
}
