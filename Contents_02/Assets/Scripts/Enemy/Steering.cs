using System.IO;
using UnityEngine;

public class Steering
{
    const float maxAcc = 20.0f;
    //目的地へ最大速度まで加速する加速度
    //Arriveとの併用は未対応
    public static Vector3 Seek(Vector3 _target, Vector3 _pos, Vector3 _velocity, float _maxSpeed)
    {
        //目標への最大速度ベクトル
        Vector3 maxVec = (_target - _pos).normalized * _maxSpeed;
        //加速度
        Vector3 steering = maxVec - _velocity;

        //加速度を最大値に補正
        steering = Vector3.ClampMagnitude(steering, maxAcc);
        //地面補正
        steering = AdjustSteering(steering, _pos);

        return steering;
    }

    //目的地からの距離に応じて減速させる加速度を作る
    //Seekとの併用は未対応
    public static Vector3 Arrive(Vector3 _target,Vector3 _pos,Vector3 _velocity,float _maxSpeed,float _slowRadius)
    {
        //距離
        float dist = (_target - _pos).magnitude;
        //目標速度
        float targetSpeed = _maxSpeed;

        //距離が移動速度を上回るなら
        if(dist <= _velocity.magnitude)
        {
            return -_velocity;
        }

        //距離が減速距離なら
        if(dist <= _slowRadius)
        {
            //最大速度から距離によって目標速度を決める
            targetSpeed = _maxSpeed * (dist / _slowRadius);
        }

        //目標ベクトル
        Vector3 targetVec = (_target - _pos).normalized * targetSpeed;
        //加速度
        Vector3 steering = targetVec - _velocity;

        //加速度を最大値に補正
        steering = Vector3.ClampMagnitude(steering, maxAcc);
        //地面補正
        steering = AdjustSteering(steering, _pos);

        return steering;
    }

    //地面の傾きに対してベクトルを補正する
    static Vector3 AdjustSteering(Vector3 _steering,Vector3 _pos)
    {
        //加速度を地面によって補正
        if (Physics.Raycast(_pos, Vector3.down, out RaycastHit info))
        {
            float dot = Vector3.Dot(info.normal, _steering);
            Vector3 exclusionVec = dot * info.normal;
            _steering -= exclusionVec;
        }

        return _steering;
    }

}
