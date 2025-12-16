using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringManager
{
    List<SteeringBase> steerings = new List<SteeringBase>();

    //ステアリング計算用関数
    public Vector3 SteeringCalc(EnemyBlackBoardBase _bb)
    {
        //戻り値を入れる変数
        Vector3 vec = Vector3.zero;
        //計算
        foreach (var steering in steerings.OrderByDescending(s => s.GetPriority())) 
        {
            Vector3 v = steering.SteeringCalc(_bb);

            if(steering.GetPriority() == _bb.avoidancePriority && v != Vector3.zero)
            {
                vec = v;
                break;
            }

            vec += steering.SteeringCalc(_bb);
        }

        //steering調整
        vec = AdjustSteering(_bb, vec);

        // steeringsのリセット
        steerings.Clear();

        return vec;
    }

    //steeringの追加用関数
    public void AddSteering(SteeringBase _steering, float _weight)
    {
        _steering.SetWeight(_weight);
        steerings.Add(_steering);
    }

    //ベクトルの補正する
    //地面に垂直なベクトルに変換
    //最大加速度にClamp
    Vector3 AdjustSteering(EnemyBlackBoardBase _bb ,Vector3 _adjustVec)
    {
        //加速度を最大値に補正
        _adjustVec = Vector3.ClampMagnitude(_adjustVec, _bb.maxAcc);

        //加速度を地面によって補正
        if (Physics.Raycast(_bb.pos, Vector3.down, out RaycastHit info))
        {
            float dot = Vector3.Dot(info.normal, _adjustVec);
            Vector3 exclusionVec = dot * info.normal;
            _adjustVec -= exclusionVec;
        }

        return _adjustVec;
    }
}
