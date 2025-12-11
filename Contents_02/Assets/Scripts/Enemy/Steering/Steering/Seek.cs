using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//目的地へ最大速度まで加速する加速度
//Arriveとの併用は未対応
public class Seek : SteeringBase
{
    public override Vector3 SteeringCalc(EnemyBlackBoardBase _bb, float weight)
    {
        //目標への最大速度ベクトル
        Vector3 maxVec = (_bb.targetPos - _bb.pos).normalized * _bb.maxSpeed;
        //加速度
        Vector3 steering = maxVec - _bb.rb.linearVelocity;

        return steering;
    }
}
