using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{

    private InputSystem_Actions inputActions;

    [Header("オブジェクト")]
    [SerializeField] PlayerCamera playerCamera; // カメラ制御スクリプト
    [SerializeField] Transform origin;          // プレイヤーの中心座標
    [SerializeField] GameObject lockOnCursor;   // ロックオンマーカーUI

    [Header("設定")]
    [SerializeField] float lockOnRange = 20.0f;     // ロックオン可能距離
    [SerializeField] LayerMask lockOnLayers;        // 敵のレイヤー
    [SerializeField] LayerMask lockOnObstacleLayers;// 壁などの障害物レイヤー

    [SerializeField] float cursorHeightOffset = 1.5f; // UIを表示する高さ 
    // 内部パラメータ
    float lockOnFactor = 0.3f;    // 距離による優先度重みづけ
    float lockOnThreshold = 0.5f; // 正面判定の閾値

    // 状態管理
    bool isLockOn = false;
    bool stickReturnFlag = true; // スティックが中央に戻ったかどうかのフラグ
    Camera mainCamera;
    Transform cameraTransform;

    // 現在のターゲットのGetter
    public GameObject TargetObj { get; private set; }

    // --- Input System 初期化処理 ---
    private void Awake()
    {
        // クラスのインスタンス生成
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // UI等を含めた実行順序を合わせるためイベント処理を行う
        inputActions.Enable();
        CinemachineCore.CameraUpdatedEvent.AddListener(OncameraUpdated);
    }

    private void OnDisable()
    {
        inputActions.Disable();
        CinemachineCore.CameraUpdatedEvent?.RemoveListener(OncameraUpdated);
    }
    // ----------------------------

    void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;

        if (lockOnCursor)
        {
            lockOnCursor.SetActive(false);
        }
    }

    void Update()
    {
        // ロックオンボタン入力の処理
        HandleLockOnInput();

        // ターゲット切り替え入力の処理（右スティック）
        HandleTargetSwitching();

        // カメラのリセット
        InputResetCamera();
    }

    // イベント処理としてカメラの移動後に合わせてカーソルなどを動かす
    void OncameraUpdated(CinemachineBrain brain)
    {
        if (isLockOn)
        {
            UpdateLockOnState();
        }
    }

    // カメラリセット処理
    void InputResetCamera()
    {
        if (inputActions.Player.CameraReset.WasPressedThisFrame())
        {
            playerCamera.ResetFreeLookCamera();
        }
    }

    // ロックオン開始処理
    void HandleLockOnInput()
    {
        if (inputActions.Player.LockOn.WasPressedThisFrame())
        {
            if (isLockOn)
            {
                // ロックオン解除
                DisableLockOn();
            }
            else
            {
                // ロックオン開始
                TargetObj = GetLockOnTarget();
                if (TargetObj != null)
                {
                    EnableLockOn(TargetObj);
                }
                else
                {
                    // 敵がいない場合はカメラリセット（正面を向く等）
                    playerCamera.ResetFreeLookCamera();
                }
            }
        }
    }

    // ターゲット切り替え処理
    void HandleTargetSwitching()
    {
        if (!isLockOn) return;

        float inputVal = inputActions.Player.TargetChange.ReadValue<float>();
        float threshold = 0.5f;

        // ★この行を追加して、Console画面で数値が出るか確認！
        if (inputVal != 0) Debug.Log("入力値: " + inputVal);
        // スティックを大きく倒した時
        if (Mathf.Abs(inputVal) > threshold)
        {
            if (stickReturnFlag)
            {
                stickReturnFlag = false; // 連続切り替え防止
                GameObject nextTarget = null;

                if (inputVal <= 0)
                {
                    // 右入力
                    nextTarget = GetLockOnTargetLeftOrRight("right");
                }
                else
                {
                    // 左入力
                    nextTarget = GetLockOnTargetLeftOrRight("left");
                }

                if (nextTarget != null)
                {
                    EnableLockOn(nextTarget);
                }
            }
        }
        // スティックが中央付近に戻ったらフラグをリセット
        else if (Mathf.Abs(inputVal) < 0.2f)
        {
            stickReturnFlag = true;
        }
    }

    void UpdateLockOnState()
    {
        if (!isLockOn) return;

        // ターゲットが存在しない、または非アクティブになった場合
        if (TargetObj == null || !TargetObj.activeInHierarchy)
        {
            DisableLockOn();
            return;
        }

        // カーソル位置の更新
        if (lockOnCursor != null)
        {
            // 1. 敵の足元の座標を取得
            Vector3 targetWorldPos = TargetObj.transform.position;

            // 2. 高さを足す (例: 1.5メートル上にずらす)
            targetWorldPos.y += cursorHeightOffset;

            // 3. スクリーン座標（画面上の2D座標）に変換
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

            // 敵がカメラの「後ろ」にいる場合は表示しない
            // screenPos.z がマイナスの場合はカメラの後ろにいる
            if (screenPos.z > 0)
            {
                lockOnCursor.SetActive(true); // 見えている
                lockOnCursor.transform.position = screenPos;
            }
            else
            {
                lockOnCursor.SetActive(false); // カメラの裏側なので隠す
            }
        }

        // 距離による解除判定
        float distance = Vector3.Distance(TargetObj.transform.position, origin.position);
        if (distance > lockOnRange)
        {
            DisableLockOn();
        }
    }

    // ロックオン起動
    void EnableLockOn(GameObject target)
    {
        isLockOn = true;
        TargetObj = target;
        playerCamera.ActiveLockOnCamera(TargetObj);
        if (lockOnCursor) lockOnCursor.SetActive(true);
    }

    // ロックオン解除
    void DisableLockOn()
    {
        isLockOn = false;
        TargetObj = null;
        playerCamera.InactiveLockOnCamera();
        if (lockOnCursor) lockOnCursor.SetActive(false);
    }

    // 正面付近の最適な敵を探す
    GameObject GetLockOnTarget()
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin.position, lockOnRange, Vector3.up, 0, lockOnLayers);

        if (hits == null || hits.Length == 0) return null;

        List<GameObject> hitObjects = MakeListRaycastHit(hits);
        if (hitObjects.Count == 0) return null;

        var tupleData = GetOptimalEnemy(hitObjects);
        float minDegree = tupleData.Item1;
        GameObject target = tupleData.Item2;

        if (Mathf.Abs(minDegree) <= lockOnThreshold)
        {
            return target;
        }
        return null;
    }

    // 左右入力によるターゲット検索
    GameObject GetLockOnTargetLeftOrRight(string direction)
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin.position, lockOnRange, Vector3.up, 0, lockOnLayers);
        if (hits == null || hits.Length == 0) return null;

        List<GameObject> hitObjects = MakeListRaycastHit(hits);

        var tupleData = GetEnemyLeftOrRight(hitObjects, direction);
        return tupleData.Item2;
    }

    // 障害物判定を行い、射線が通る敵だけのリストを作る
    List<GameObject> MakeListRaycastHit(RaycastHit[] hits)
    {
        List<GameObject> hitObjects = new List<GameObject>();
        RaycastHit hit;

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject checkObj = hits[i].collider.gameObject;
            Vector3 direction = checkObj.transform.position - origin.position;

            // ここでは簡易的にoriginから飛ばす
            if (Physics.Raycast(origin.position, direction, out hit, lockOnRange, lockOnObstacleLayers | lockOnLayers))
            {
                // レイが最初に当たったのがその敵自身であれば「壁に隠れていない」と判断
                if (hit.collider.gameObject == checkObj)
                {
                    hitObjects.Add(checkObj);
                }
            }
        }
        return hitObjects;
    }

    // リストの中から画面中央（カメラ正面）に最も近い敵を探す
    (float, GameObject) GetOptimalEnemy(List<GameObject> hitObjects)
    {
        float degreep = Mathf.Atan2(cameraTransform.forward.x, cameraTransform.forward.z);
        float minDegree = Mathf.PI * 2.0f;
        GameObject target = null;

        foreach (var enemy in hitObjects)
        {
            Vector3 enemyToCameraPos = cameraTransform.position - enemy.transform.position;
            Vector3 cameraToEnemyPos = enemy.transform.position - cameraTransform.position;
            cameraToEnemyPos.y = 0.0f;
            cameraToEnemyPos.Normalize();

            float degree = Mathf.Atan2(cameraToEnemyPos.x, cameraToEnemyPos.z);
            degree = DegreeNormalize(degree, degreep);

            // 距離が近いほど優先度を上げる（角度補正）
            degree = degree + degree * (enemyToCameraPos.magnitude / 500.0f) * lockOnFactor;

            if (Mathf.Abs(minDegree) >= Mathf.Abs(degree))
            {
                minDegree = degree;
                target = enemy;
            }
        }
        return (minDegree, target);
    }

    // 左右切り替えの候補を探す(タプルでターゲットと角度を返す)
    (float, GameObject) GetEnemyLeftOrRight(List<GameObject> hitObjects, string direction)
    {
        float degreep = Mathf.Atan2(cameraTransform.forward.x, cameraTransform.forward.z);
        float minDegree = Mathf.PI * 2.0f;
        GameObject target = null;

        foreach (var enemy in hitObjects)
        {
            if (enemy == TargetObj) continue; // 現在のターゲットは除外

            Vector3 enemyToCameraPos = cameraTransform.position - enemy.transform.position;
            Vector3 cameraToEnemyPos = enemy.transform.position - cameraTransform.position;
            cameraToEnemyPos.y = 0.0f;
            cameraToEnemyPos.Normalize();

            float degree = Mathf.Atan2(cameraToEnemyPos.x, cameraToEnemyPos.z);
            degree = DegreeNormalize(degree, degreep);

            // 左右の選別
            if (direction == "left")
            {
                // 左入力時、右側にある敵（角度が正）は無視
                if (degree > 0) continue;
            }
            else // right
            {
                // 右入力時、左側にある敵（角度が負）は無視
                if (degree < 0) continue;
            }

            // 距離による重みづけ
            degree = degree + degree * (enemyToCameraPos.magnitude / 1.0f) * lockOnFactor;

            if (Mathf.Abs(minDegree) >= Mathf.Abs(degree))
            {
                minDegree = degree;
                target = enemy;
            }
        }
        return (minDegree, target);
    }

    // 角度の正規化 (-PI ~ PI)
    float DegreeNormalize(float degree, float degreep)
    {
        float diff = degreep - degree;

        if (diff >= Mathf.PI)
        {
            diff -= Mathf.PI * 2.0f;
        }
        else if (diff <= -Mathf.PI)
        {
            diff += Mathf.PI * 2.0f;
        }
        return diff;
    }
}