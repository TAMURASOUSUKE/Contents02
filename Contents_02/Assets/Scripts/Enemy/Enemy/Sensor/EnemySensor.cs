using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    //視界に使うコライダー
    SphereCollider collider;
    //エネミーのブラックボード
    EnemyBlackBoardBase bb;
    private void Awake()
    {
        collider = transform.GetComponent<SphereCollider>();
    }
    private void OnTriggerStay(Collider other)
    {
        //視野角いないなら
        Vector3 dir = other.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);
        if(angle <= bb.fov)
        {
            //レイ判定(間に障害物がないか)
            Ray ray = new Ray(transform.position, dir);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, collider.radius))
            {
                //当たったコライダーがレイのコライダーと同じなら見える
                if (hitInfo.collider == other)
                {
                    //プレイヤーのタグならプレイヤーと判定
                    if(hitInfo.collider.CompareTag("Player"))
                    {
                        //目標に設定
                        bb.target = hitInfo.transform;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //センサー外に目標対象が逃げたなら
        if (bb.target == other.transform)
        {
            bb.target = null;
        }
    }

    public void SetBB(EnemyBlackBoardBase _bb)
    {
        bb = _bb;
        //ついでにブラックボードに合わせる
        collider.radius = _bb.sensorLen;
    }
}