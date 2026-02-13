using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class SteeringManager
{
    List<AvoidanceSteering> avoidSteerings = new List<AvoidanceSteering>();
    List<IntentionSteering> intentSteerings = new List<IntentionSteering>();

    //ステアリング計算用関数
    public Vector3 SteeringCalc(EnemyBlackBoardBase _bb)
    {
        //戻り値を入れる変数
        Vector3 vec = Vector3.zero;
        //意思計算
        foreach (var intent in intentSteerings)
        {
            vec += intent.SteeringCalc(_bb);
        }

        //回避ステアリング計算
        foreach (var avoid in avoidSteerings)
        {
            if (avoid.TryGetAvoidance(_bb, out AvoidInfo info))
            {
                //移動ベクトルと、法線との内積
                float dot = Vector3.Dot(info.normal, vec);

                //回避方向を計算(移動ベクトルから法線方向成分をのいたベクトルを正規化)
                Vector3 targetDir = (vec - (dot * info.normal)).normalized;

                //目標ベクトルに必要な加速度を計算
                vec = (targetDir * vec.magnitude - _bb.vel);
                //--------------------------------------------------------

                vec += -info.normal * info.strength;

                Vector3 tangent = Vector3.Cross(info.normal, Vector3.up).normalized;

                // 横方向が弱すぎたら、強制的に接線を足す(targetDir * _bb.maxSpeed - vec)
                if (vec.sqrMagnitude < _bb.maxAcc)
                {
                    vec += tangent * _bb.maxAcc;
                }
            }
        }

        //steering調整
        vec = AdjustSteering(_bb, vec);

        // steeringsのリセット
        avoidSteerings.Clear();
        intentSteerings.Clear();

        return vec;
    }

    //steeringの追加用関数
    public void AddSteering(AvoidanceSteering _steering, float _weight)
    {
        _steering.SetWeight(_weight);
        avoidSteerings.Add(_steering);
    }

    public void AddSteering(IntentionSteering _steering, float _weight)
    {
        _steering.SetWeight(_weight);
        intentSteerings.Add(_steering);
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
