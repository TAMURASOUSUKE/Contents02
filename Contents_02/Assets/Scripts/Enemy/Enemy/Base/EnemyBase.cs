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
    protected Seek seek = new Seek();
    protected Arrive arrive = new Arrive();
}
