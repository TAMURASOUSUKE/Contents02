using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLockOn : MonoBehaviour
{

    private InputSystem_Actions inputActions;

    [Header("References")]
    [SerializeField] private PlayerCamera playerCamera; // カメラ制御スクリプト
    [SerializeField] private Transform origin;          // プレイヤーの中心座標
    [SerializeField] private GameObject lockOnCursor;   // ロックオンマーカーUI

    [Header("Settings")]
    [SerializeField] private float lockOnRange = 20.0f;     // ロックオン可能距離
    [SerializeField] private LayerMask lockOnLayers;        // 敵のレイヤー
    [SerializeField] private LayerMask lockOnObstacleLayers;// 壁などの障害物レイヤー

    // 内部パラメータ
    private float lockOnFactor = 0.3f;    // 距離による優先度重みづけ
    private float lockOnThreshold = 0.5f; // 正面判定の閾値

    // 状態管理
    public bool isLockOn = false;
    private bool stickReturnFlag = true; // スティックが中央に戻ったかどうかのフラグ
    private GameObject targetObj;
    private Camera mainCamera;
    private Transform cameraTransform;

    // --- Input System 初期化処理 ---
    private void Awake()
    {
        // クラスのインスタンス生成
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
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
        // 1. ロックオンボタン入力の処理
        HandleLockOnInput();

        // 2. ターゲット切り替え入力の処理（右スティック）
        HandleTargetSwitching();

        // 3. ロックオン中の状態更新（距離判定、カーソル移動）
        UpdateLockOnState();
    }

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
                targetObj = GetLockOnTarget();
                if (targetObj != null)
                {
                    EnableLockOn(targetObj);
                }
                else
                {
                    // 敵がいない場合はカメラリセット（正面を向く等）
                    playerCamera.ResetFreeLookCamera();
                }
            }
        }
    }

    void HandleTargetSwitching()
    {
        if (!isLockOn) return;

        Vector2 inputVal = inputActions.Player.Look.ReadValue<Vector2>();
        float inputX = inputVal.x;

        // スティックを大きく倒した時
        if (Mathf.Abs(inputX) > 0.8f)
        {
            if (stickReturnFlag)
            {
                stickReturnFlag = false; // 連続切り替え防止
                GameObject nextTarget = null;

                if (inputX > 0)
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
        else if (Mathf.Abs(inputX) < 0.2f)
        {
            stickReturnFlag = true;
        }
    }

    void UpdateLockOnState()
    {
        if (!isLockOn) return;

        // ターゲットが存在しない、または非アクティブになった場合
        if (targetObj == null || !targetObj.activeInHierarchy)
        {
            DisableLockOn();
            return;
        }

        // カーソル位置の更新
        if (lockOnCursor != null)
        {
            lockOnCursor.transform.position = mainCamera.WorldToScreenPoint(targetObj.transform.position);
        }

        // 距離による解除判定
        float distance = Vector3.Distance(targetObj.transform.position, origin.position);
        if (distance > lockOnRange)
        {
            DisableLockOn();
        }
    }

    void EnableLockOn(GameObject target)
    {
        isLockOn = true;
        targetObj = target;
        playerCamera.ActiveLockOnCamera(targetObj);
        if (lockOnCursor) lockOnCursor.SetActive(true);
    }

    void DisableLockOn()
    {
        isLockOn = false;
        targetObj = null;
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
            if (enemy == targetObj) continue; // 現在のターゲットは除外

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
            degree = degree + degree * (enemyToCameraPos.magnitude / 500.0f) * lockOnFactor;

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