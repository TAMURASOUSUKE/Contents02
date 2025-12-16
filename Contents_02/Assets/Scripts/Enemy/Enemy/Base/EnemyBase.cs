using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //エネミーの初期値
    [SerializeField]
    protected SO_EnemyData data;
    //ブラックボード
    protected EnemyBlackBoardBase bb;
    //ステアリングマネージャー
    protected SteeringManager steeringManager = new SteeringManager();

    //ステアリング各種
    protected Seek seek;
    protected Arrive arrive;
    protected ObstacleAvoidance obstacleAvoidance;

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
