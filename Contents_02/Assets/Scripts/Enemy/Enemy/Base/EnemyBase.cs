using UnityEngine;

public class EnemyBase<T> : MonoBehaviour
    where T : EnemyBlackBoardBase
{
    //エネミーの初期値
    [SerializeField]
    protected SO_EnemyData data;
    [SerializeField]
    protected EnemySensor sensor;
    //ブラックボード
    protected T bb;
    //ステート
    protected EnemyStateBase<T> state;

    //移動方向を向かせる関数
    protected void RotateLookFront()
    {
        Vector3 velXZ = new Vector3(bb.vel.x, 0, bb.vel.z);

        if(velXZ != Vector3.zero)
        {
            Quaternion frontRot = Quaternion.LookRotation(velXZ);

            transform.rotation = frontRot;
        }
    }
}
