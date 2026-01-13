using UnityEngine;
using static Unity.Cinemachine.CinemachineDeoccluder;

public class TestEnemyPatrolState:EnemyStateBase<TestEnemyBB>
{
    public override EnemyStateBase<TestEnemyBB> StateUpdate(TestEnemyBB _bb)
    {
        _bb.pathFollow.PathRandMoveCalc(_bb);
        _bb.steeringManager.AddSteering(_bb.obstacleAvoidance, 1.0f);
        _bb.steeringManager.AddSteering(_bb.fallAvoidance, 1.0f);
        _bb.steeringManager.AddSteering(_bb.arrive, 1.0f);
        Vector3 vec = _bb.steeringManager.SteeringCalc(_bb);


        Debug.DrawRay(_bb.pos, _bb.trans.forward);
        Debug.DrawRay(_bb.pos, _bb.pos + vec, Color.gray);
        Debug.DrawRay(_bb.pos, _bb.pos + _bb.vel, Color.black);

        //ç≈ëÂë¨ìxÇÊÇËíxÇ¢Ç»ÇÁ
        if (_bb.vel.magnitude < _bb.maxSpeed)
        {
            //â¡ë¨ìxí«â¡
            _bb.rb.AddForce(vec, ForceMode.Acceleration);
            //â¡ë¨ìxÇí«â¡ÇµÇƒí¥Ç¶ÇΩÇÁï‚ê≥
            if (_bb.vel.magnitude > _bb.maxSpeed)
            {
                _bb.rb.linearVelocity = _bb.vel.normalized * _bb.maxSpeed;
            }
        }

        return this;
    }
}
