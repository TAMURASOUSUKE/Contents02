using UnityEngine;

public class FallAvoidance : SteeringBase
{
    //回避が必要フラグ
    bool isAvoiding = false;
    //回避方向
    Vector3 avoidVec = Vector3.zero;
    //回避を続ける時間の計測用変数
    float avoidTimer = 0.0f;
    //回避を続ける時間
    const float avoidTime = 0.3f;
    public FallAvoidance(EnemyBlackBoardBase _bb)
    {
        priority = _bb.fallAvoidancePriority;
    }
    public override Vector3 SteeringCalc(EnemyBlackBoardBase _bb)
    {
        //速度がないならゼロ
        if (_bb.vel == Vector3.zero)
        {
            return Vector3.zero;
        }
        //前方
        Vector3 fwd = _bb.trans.forward;

        //前、左右のオフセット作成
        //前
        Vector3 fwdRayOffset = _bb.trans.forward * _bb.vel.magnitude;
        //右
        Vector3 rightAngleRayOffset = _bb.fallDodgeRayOffsetRotR * fwdRayOffset;
        Vector3 rightRayOffset = _bb.trans.right * _bb.vel.magnitude;
        //左
        Vector3 leftAngleRayOffset = _bb.fallDodgeRayOffsetRotL * fwdRayOffset;
        Vector3 leftRayOffset = -_bb.trans.right * _bb.vel.magnitude;

        Vector3[] offsets = { 
            fwdRayOffset, 
            rightRayOffset, 
            rightAngleRayOffset, 
            leftRayOffset, 
            leftAngleRayOffset 
        };

        //疑似法線ベクトル
        Vector3 normal = Vector3.zero;

        //どれか一つのレイが当たったフラグ
        bool isHit = false;

        foreach(Vector3 offset in offsets)
        {
            if(Physics.Raycast(_bb.pos + offset, Vector3.down, _bb.fallDodgeRayLen) == false)
            {
                isHit = true;
                normal += offset;
            }
        }
        if (!isHit && isAvoiding)
        {
            //時間を計測してフラグを下げる
            avoidTimer += Time.fixedDeltaTime;
            if (avoidTimer >= avoidTime)
            {
                avoidTimer = 0;
                isAvoiding = false;
            }

            return avoidVec;
        }

        //疑似法線がないのならゼロを返して終了
        if (normal == Vector3.zero)
        {
            return Vector3.zero;
        }

        //疑似法線を正規化
        normal.Normalize();
        isAvoiding = true;

        //--------------回避方向の決定--------------
        //移動ベクトルと、法線との内積
        float dot = Vector3.Dot(normal, _bb.vel);

        //回避方向を計算(移動ベクトルから法線方向成分をのいたベクトルを正規化)
        Vector3 targetDir = (_bb.vel - (dot * normal)).normalized;
        //----------------------------------------------------------

        //--------------回避方向から加速度方向を作る--------------
        //目標ベクトルに必要な加速度を計算
        Vector3 steering = (targetDir * _bb.maxSpeed - _bb.vel);
        //--------------------------------------------------------
        //--------------障害物方向に離れるベクトルを追加--------------
        steering += -(dot * normal) * _bb.slowRadius;

        Debug.DrawLine(_bb.pos, _bb.pos + fwdRayOffset, Color.yellow);
        Debug.DrawLine(_bb.pos, _bb.pos + rightRayOffset,Color.blue);
        Debug.DrawLine(_bb.pos, _bb.pos + leftRayOffset, Color.red);
        Debug.DrawLine(_bb.pos, _bb.pos + rightAngleRayOffset, Color.blue);
        Debug.DrawLine(_bb.pos, _bb.pos + leftAngleRayOffset, Color.red);

        Debug.DrawRay(_bb.pos + fwdRayOffset, Vector3.down, Color.yellow);
        Debug.DrawRay(_bb.pos + rightRayOffset, Vector3.down, Color.blue);
        Debug.DrawRay(_bb.pos + leftRayOffset, Vector3.down, Color.red);
        Debug.DrawRay(_bb.pos + rightAngleRayOffset, Vector3.down, Color.blue);
        Debug.DrawRay(_bb.pos + leftAngleRayOffset, Vector3.down, Color.red);


        //重み反映
        steering *= weight;
        //回避強度反映
        steering *= _bb.dodgeStrength;

        avoidVec = steering;

        return steering;
    }
}
