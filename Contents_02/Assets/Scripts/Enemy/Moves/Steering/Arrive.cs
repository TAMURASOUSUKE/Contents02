using UnityEngine;

//目的地からの距離に応じて減速させる加速度を作る
//Seekとの併用は未対応
public class Arrive : IntentionSteering
{
    public Arrive(EnemyBlackBoardBase _bb)
    {
        priority = _bb.arrivePriority;
    }
    public override Vector3 SteeringCalc(EnemyBlackBoardBase _bb)
    {
        //ターゲットが決まってないなら移動しない
        if (_bb.moveTarget == null)
        {
            return Vector3.zero;
        }
        
        //ターゲットが決まってないなら移動しない
        //距離
        float dist = (_bb.moveTarget.Value - _bb.pos).magnitude;

        // 距離が、停止距離より近いならゼロベクトルを返す
        if(dist < _bb.stopDistance)
        {
            return Vector3.zero;
        }

        //目標速度
        float targetSpeed = _bb.maxSpeed;

        //距離が限りなく近いなら
        if (dist <= _bb.rb.linearVelocity.magnitude)
        {
            //急停止
            return -_bb.vel;
        }

        //距離が減速距離なら
        if (dist <= _bb.slowRadius)
        {
            //最大速度から距離によって目標速度を決める
            targetSpeed = _bb.maxSpeed * (dist / _bb.slowRadius);
        }

        //目標ベクトル
        Vector3 targetVec = (_bb.moveTarget.Value - _bb.pos).normalized * targetSpeed;
        //加速度
        Vector3 steering = targetVec - _bb.vel;

        //重み反映
        steering *= weight;

        return steering;
    }
}
