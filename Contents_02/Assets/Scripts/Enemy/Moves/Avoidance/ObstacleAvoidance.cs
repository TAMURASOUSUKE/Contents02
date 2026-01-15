using UnityEditor.Rendering;
using UnityEngine;

public class ObstacleAvoidance : AvoidanceSteering
{
    //回避が必要フラグ
    bool isAvoiding = false;
    //回避方向
    Vector3 avoidDir = Vector3.zero;
    //離れるベクトルの強さ
    float strength;
    //回避を続ける時間の計測用変数
    float avoidTimer = 0.0f;
    //回避を続ける時間
    const float avoidTime = 0.3f;

    public ObstacleAvoidance(EnemyBlackBoardBase _bb)
    {
        priority = _bb.avoidancePriority;
    }

    public override bool TryGetAvoidance(EnemyBlackBoardBase _bb,out AvoidInfo _info)
    {
        _info = new AvoidInfo();
        //速度がないならゼロ
        if (_bb.vel == Vector3.zero)
        {
            return false;
        }

        //レイ
        Ray frontRay = new Ray(_bb.pos, _bb.vel);

        if (Physics.SphereCast(frontRay, _bb.frontSphereCastRadius, out RaycastHit hitInfo, _bb.vel.magnitude))
        {
            //エネミーじゃないなら
            if(hitInfo.transform.CompareTag("Enemy") == false)
            {
                //回避必要フラグを立てる
                isAvoiding = true;

                //距離
                float dist = hitInfo.distance;

                //法線方向
                _info.normal = hitInfo.normal;
                avoidDir = hitInfo.normal;
                //離れるベクトルの強さ
                if (dist <= _bb.slowRadius)
                {
                    strength = _bb.slowRadius / dist;
                }
                //強さ代入
                _info.strength = strength;

                return true;
            }
        }
        else if (isAvoiding)
        {
            _info.normal = avoidDir;
            _info.strength = strength;

            //時間を計測してフラグを下げる
            avoidTimer += Time.fixedDeltaTime;
            if (avoidTimer >= avoidTime)
            {
                avoidTimer = 0;
                isAvoiding = false;
            }

        }

        //重み反映
        _info.strength *= weight;
        //強さ反映
        _info.strength *= _bb.dodgeStrength;

        return false;
    }
}