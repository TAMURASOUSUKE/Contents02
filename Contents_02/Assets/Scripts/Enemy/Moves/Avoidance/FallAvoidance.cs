using UnityEngine;

public class FallAvoidance : AvoidanceSteering
{
    public FallAvoidance(EnemyBlackBoardBase _bb)
    {
        priority = _bb.fallAvoidancePriority;
    }
    public override bool TryGetAvoidance(EnemyBlackBoardBase _bb,out AvoidInfo _info)
    {
        _info = new AvoidInfo();
        //速度がないならゼロ
        if (_bb.vel == Vector3.zero)
        {
            return false;
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

        foreach(Vector3 offset in offsets)
        {
            if(Physics.Raycast(_bb.pos + offset, Vector3.down, _bb.fallDodgeRayLen) == false)
            {
                normal += offset;
            }
        }
        //疑似法線がないのならゼロを返して終了
        if (normal == Vector3.zero)
        {
            return false;
        }

        //疑似法線を正規化
        normal.Normalize();

        //法線代入
        _info.normal = normal;
        //強さは、減速半径をそのまま
        _info.strength = _bb.slowRadius;

        //Debug.DrawLine(_bb.pos, _bb.pos + fwdRayOffset, Color.yellow);
        //Debug.DrawLine(_bb.pos, _bb.pos + rightRayOffset,Color.blue);
        //Debug.DrawLine(_bb.pos, _bb.pos + leftRayOffset, Color.red);
        //Debug.DrawLine(_bb.pos, _bb.pos + rightAngleRayOffset, Color.blue);
        //Debug.DrawLine(_bb.pos, _bb.pos + leftAngleRayOffset, Color.red);

        //Debug.DrawRay(_bb.pos + fwdRayOffset, Vector3.down, Color.yellow);
        //Debug.DrawRay(_bb.pos + rightRayOffset, Vector3.down, Color.blue);
        //Debug.DrawRay(_bb.pos + leftRayOffset, Vector3.down, Color.red);
        //Debug.DrawRay(_bb.pos + rightAngleRayOffset, Vector3.down, Color.blue);
        //Debug.DrawRay(_bb.pos + leftAngleRayOffset, Vector3.down, Color.red);


        //重み反映
        _info.strength *= weight;
        //回避強度反映
        _info.strength *= _bb.dodgeStrength;

        return true;
    }
}
