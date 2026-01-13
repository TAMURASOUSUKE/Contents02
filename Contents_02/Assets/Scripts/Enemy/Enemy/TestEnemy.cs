using UnityEngine;

public class TestEnemy : EnemyBase<TestEnemyBB>
{
    [SerializeField]
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ブラックボード生成
        bb = new TestEnemyBB(data, GetComponent<Rigidbody>(), transform);
        // ステート生成
        state = new TestEnemyPatrolState();

        // 視界セットアップ
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
        state = state.StateUpdate(bb);
    }
}
