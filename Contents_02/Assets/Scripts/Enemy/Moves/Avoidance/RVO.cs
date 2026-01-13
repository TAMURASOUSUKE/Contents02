using UnityEngine;

public class RVO
{
    static public Vector3 AdjustVec(EnemyBlackBoardBase _bb,Vector3 adjestVec)
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
            Vector3 vRel = adjestVec - other.GetVel();

            //VOの軸と相対速度の角度
            float angle = Vector3.Angle(axis, vRel);

            //角度内で当たりそうなら
            if(theta >= angle)
            {
                //新ベクトルの決定
                adjestVec =
                    Quaternion.AngleAxis((theta + angle) * 0.5f, _bb.trans.up) * axis
                    * adjestVec.magnitude;
            }
        }

        return adjestVec;
    }
}
