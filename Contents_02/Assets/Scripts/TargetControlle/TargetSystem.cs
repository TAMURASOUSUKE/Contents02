using UnityEngine;
using Unity.Cinemachine;

public class TargetSystem : MonoBehaviour
{
    [Header("Cinemachine設定")]
    [SerializeField] CinemachineCamera lockOnCamera; // ロックオン用のカメラ
    [SerializeField] CinemachineTargetGroup targetGroup; // ターゲットグループ
    [Header("ターゲットグループ関連です。weightが強いとそのオブジェクトを中心にとらえます")]
    [Header("Radiusはカメラのズーム距離のようなもので指定した半径を描画内に含めます")]
    [SerializeField] float targetWeight;
    [SerializeField] float targetRadius;
    [SerializeField] float userWeight;
    [SerializeField] float userRadius;


    [Header("設定")]
    [SerializeField] Transform userTransform; // ターゲット使用者のトランスフォーム
    [SerializeField] float searchRadius = 20.0f; // 索敵範囲
    [SerializeField] LayerMask targetLayer; // 敵レイヤー
    [SerializeField] Transform targetCursor; // ロックオンマーカーのUIを入れる。(頭の上に表示するため)
    [SerializeField] float cursorOffsetHeight = 2.0f; // 敵の頭上に表示する高さ

    public GameObject CurrentTarget {  get; private set; } // 現在のターゲットを取得するGetter

    Camera mainCam; // メインカメラ

    private void Start()
    {
       mainCam = Camera.main; // メインカメラを取得

        if(targetCursor != null)
        {
            targetCursor.gameObject.SetActive(false); // 最初はカーソルを隠しておく
        }

        if(lockOnCamera != null)
        {
            lockOnCamera.gameObject.SetActive(false);
        }

        // 最初にターゲットグループにuserを追加しておく
        if (targetGroup != null && userTransform != null)
        {
            // クリア処理
            targetGroup.Targets.Clear();

            // ユーザーの追加
            targetGroup.AddMember(userTransform, userWeight, userRadius);
        }
    }

    private void Update()
    {
        // ターゲットが空でないなら追従する
        if (CurrentTarget != null)
        {
            // 敵がいなくなったり寝台した場合は解除
            if (!CurrentTarget.activeInHierarchy)
            {
                ClearTarget();
                return;
            }


            // カーソルの位置更新
            if(targetCursor != null)
            {
                targetCursor.position = CurrentTarget.transform.position + Vector3.up * cursorOffsetHeight; // 敵の位置 + 上方向 * オフセットで追従

                targetCursor.LookAt(mainCam.transform); // ビルボード処理としてカメラ方向を向かせる
            }
        }
    }


    // トグル式でロックオンとオフの切り替えを行う
    public void ToggleLockOn()
    {


        if (CurrentTarget != null)
        {
            ClearTarget(); // すでにロック中なら解除
        }
        else
        {
            // そうでないなら画面中央に一番近い敵を探す
            CurrentTarget = FindGetNearScreenCenter();

            if (CurrentTarget != null && targetCursor != null)
            {
                
                // グループに敵を追加
                if(targetGroup != null)
                {
                    // 敵をメンバーに追加するweightが大きいほどカメラはその対象を中心にとらえる
                    targetGroup.AddMember(CurrentTarget.transform, 1.0f, 1.5f);
                }

                lockOnCamera.gameObject.SetActive(true);
                targetCursor.gameObject.SetActive(true); // 見えるようにする
            }
        }
    }


    // ターゲットの切り替え
    public void SwitchTarget(int direction)
    {
        if (CurrentTarget == null) return; // ロックしていない場合は処理しない

        Collider[] enemies = Physics.OverlapSphere(userTransform.position, searchRadius, targetLayer); // 自身の場所から設定した範囲分だけ、レイヤーを持った敵を取得する

        GameObject nextTarget = null; // 次の敵を定義
        float minScreenDistance = float.MaxValue; // どの敵が一番近いかを探すときの基準

        // 現在のターゲットのスクリーン座標を出す
        Vector3 currentTargetScreenPos = mainCam.WorldToScreenPoint(CurrentTarget.transform.position);
        
        foreach(var enemy in enemies){
            if(enemy.gameObject == CurrentTarget) continue; // 現在のターゲットと同じならスキップ

            // 敵の画面位置を取得
            Vector3 enemyScreenPos = mainCam.WorldToScreenPoint(enemy.transform.position);

            // 画面外の敵は無視する
            if(enemyScreenPos.z < 0) continue; // カメラの後ろは計算外

            // 引数に入れた値が正ならターゲットより右(xが大きい側)
            // 負なら左側(xが小さい側へ動く)

            bool isCorrectDirection = (direction > 0) ? (enemyScreenPos.x > currentTargetScreenPos.x) : (enemyScreenPos.x < currentTargetScreenPos.x);

            if (isCorrectDirection)
            {
                // 方向があっている敵の中で現在のターゲットとスクリーン座標が一番近い敵を選ぶ
                float dist = Vector2.Distance(currentTargetScreenPos, enemyScreenPos);

                // 最小値処理
                if(dist  < minScreenDistance)
                {
                    minScreenDistance = dist;
                    nextTarget = enemy.gameObject;
                }
            }
        
        }

        // 次の敵が見つかったら更新
        if(nextTarget != null)
        {
            // ターゲットの入れ替えを行う(古い敵を消して新しい敵を入れる)
            if (targetGroup != null)
            {
                targetGroup.RemoveMember(CurrentTarget.transform); // 古い敵を削除
                targetGroup.AddMember(nextTarget.transform, 1.0f, 1.5f); // 新しい敵を追加
            }

            CurrentTarget = nextTarget;
        }
    
    }


    // 画面中央に一番近い敵を探す
    GameObject FindGetNearScreenCenter()
    {
        Collider[] enemies = Physics.OverlapSphere(userTransform.position, searchRadius, targetLayer);  

        GameObject bestTarget = null; // 最終結果
        float minDistFromCenter = float.MaxValue; // 中央からの距離
        Vector2 screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f); // 中層

        foreach (var enemy in enemies)
        {
            // カメラの後ろにいる敵は除外
            Vector3 screenPos = mainCam.WorldToScreenPoint(enemy.transform.position);
            if (screenPos.z < 0) continue;

            float dist = Vector2.Distance(screenCenter, screenPos);

            if (dist < minDistFromCenter)
            {
                minDistFromCenter = dist;
                bestTarget = enemy.gameObject;
            }
        }

        return bestTarget;
    }

    // ターゲットの解除処理
    void ClearTarget()
    {
        CurrentTarget = null;
        if(targetCursor != null)
        {
            lockOnCamera.gameObject.SetActive(false);
            targetCursor.gameObject.SetActive(false);
        }
    }
}
