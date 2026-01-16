using UnityEngine;

public class RVO
{
    static public void AdjustVec(EnemyBlackBoardBase _bb)
    {
        foreach(var other in _bb.rvoList)
        {
            //差
            Vector3 diff = other.GetPos() - _bb.pos;
            //距離
            float dist = diff.magnitude;
            //VOの軸
            Vector3 axis = diff.normalized;

            //二つの半径の合計
            float r = _bb.rvoRadius + other.GetRvoRadius();

            //VOの角度
            float theta = Mathf.Asin(r / dist) * Mathf.Rad2Deg;

            //相対速度
            Vector3 vRel = _bb.vel - other.GetVel();

            //VOの軸と相対速度の角度
            float angle = Vector3.Angle(axis, vRel);

            Debug.Log(angle);
            Debug.Log("theta" + theta);
            //角度内で当たりそうなら
            if(theta >= angle)
            {
                // 逃げる方向（左右判定）
                Vector3 cross = Vector3.Cross(axis, vRel);
                float sign =Mathf.Sign(Vector3.Dot(cross, _bb.trans.up));

                // VO境界までの角度
                float escapeAngle = theta + 0.5f; // 少し余裕

                Vector3 newVec =
                    Quaternion.AngleAxis(escapeAngle * sign, _bb.trans.up) * _bb.vel;

                _bb.rb.linearVelocity = newVec.normalized * _bb.vel.magnitude;
            }
        }
    }
}