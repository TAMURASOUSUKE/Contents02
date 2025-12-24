using UnityEngine;

public class Wander : IntentionSteering
{
    float wanderAngle;

    public Wander(EnemyBlackBoardBase _bb)
    {
        priority = _bb.wanderPriority;
    }
    public override Vector3 SteeringCalc(EnemyBlackBoardBase _bb)
    {
        //円周上のランダム点を求めるための角度を計算
        wanderAngle += Random.Range(-_bb.wanderJitter, _bb.wanderJitter);

        //円の中心点計算
        Vector3 circlePos = _bb.trans.forward * _bb.wanderDistance;

        //円周上のランダム点
        Vector3 wanderTarget = new Vector3(
            Mathf.Sin(wanderAngle),
            0,
            Mathf.Cos(wanderAngle)
            ) * _bb.wanderRadius;

        //中心位置にずらす
        wanderTarget += circlePos;

        // Seek と同じ計算
        Vector3 desired = (wanderTarget - _bb.pos).normalized * _bb.maxSpeed;
        Vector3 steering = desired - _bb.vel;

        //重み調整した値を返す
        return steering * weight;
    }
}
