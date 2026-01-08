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
        //加速度
        Vector3 steering = Vector3.zero;

        //レイ
        Ray frontRay = new Ray(_bb.pos, _bb.vel);

        if (Physics.SphereCast(frontRay, _bb.frontSphereCastRadius, out RaycastHit hitInfo, _bb.vel.magnitude))
        {
            //回避必要フラグを立てる
            isAvoiding = true;

            //--------------回避方向の決定--------------
            //移動ベクトルと、法線との内積
            float dot = Vector3.Dot(hitInfo.normal, _bb.vel);

            //回避方向を計算(移動ベクトルから法線方向成分をのいたベクトルを正規化)
            Vector3 targetDir = (_bb.vel - (dot * hitInfo.normal)).normalized;
            //----------------------------------------------------------

            //--------------回避方向から加速度方向を作る--------------
            //目標ベクトルに必要な加速度を計算
            steering = (targetDir * _bb.maxSpeed - _bb.vel);
            //--------------------------------------------------------

            //--------------障害物方向に離れるベクトルベクトルを追加(距離に応じて強くなる)--------------
            float dist = hitInfo.distance;
            //距離が遅くなる距離以下なら
            if (dist <= _bb.slowRadius)
            {
                //オブジェクトの法線方向に距離に応じて離れるベクトルを作成、追加
                steering += -(dot * hitInfo.normal) * (_bb.slowRadius / dist);
            }

            avoidDir = steering;

            //法線方向
            _info.normal = hitInfo.normal;
            avoidDir = hitInfo.normal;
            //離れるベクトルの強さ
            if (dist <= _bb.slowRadius)
            {
                strength = _bb.slowRadius / dist;
            }

            _info.strength = strength;

            return true;
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
        steering *= weight;
        //強さ反映
        steering *= _bb.dodgeStrength;

        return false;
    }
}