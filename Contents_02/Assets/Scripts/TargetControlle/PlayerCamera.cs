using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineCamera freeLookCamera;
    [SerializeField] CinemachineCamera lockOnCamera;
    [SerializeField] Transform playerTransform; // プレイヤーのTransform

    // 内部で作る「カメラ用回転軸」
    private GameObject lockOnPivot;

    readonly int LockOnCameraActivePriority = 11;
    readonly int LockOnCameraInactivePriority = 0;

    private void Start()
    {
        // ピボット用の空オブジェクトを動的に生成
        lockOnPivot = new GameObject("LockOnCameraPivot");
        // 最初はプレイヤーと同じ場所に置いておく
        lockOnPivot.transform.position = playerTransform.position;
        lockOnPivot.transform.rotation = playerTransform.rotation;
    }

    private void LateUpdate()
    {
        // ピボットは常にプレイヤーの座標に同期させる
        if (lockOnPivot != null && playerTransform != null)
        {
            lockOnPivot.transform.position = playerTransform.position;

            // ロックオン中は、ピボットを「敵の方向」に向ける
            if (lockOnCamera.Priority == LockOnCameraActivePriority && lockOnCamera.LookAt != null)
            {
                Transform enemy = lockOnCamera.LookAt;

                // 敵への方向ベクトル（高さYは無視して水平回転のみにする）
                Vector3 dirToEnemy = enemy.position - playerTransform.position;
                dirToEnemy.y = 0; // 高低差でカメラが地面に潜るのを防ぐ

                if (dirToEnemy != Vector3.zero)
                {
                    // ピボットを回転させる
                    // ※Slerpを使うと少し遅れて追従する味付けも可能（今回は即時回転）
                    lockOnPivot.transform.rotation = Quaternion.LookRotation(dirToEnemy);
                }
            }
        }
    }

    public void ResetFreeLookCamera()
    {
        // 任意の実装
    }

    public void ActiveLockOnCamera(GameObject target)
    {
        lockOnCamera.Priority = LockOnCameraActivePriority;


        // Follow（位置基準）はプレイヤーではなく「ピボット」にする
        lockOnCamera.Follow = lockOnPivot.transform;

        // LookAt（注視点）は敵のまま
        lockOnCamera.LookAt = target.transform;
    }

    public void InactiveLockOnCamera()
    {
        lockOnCamera.Priority = LockOnCameraInactivePriority;
        lockOnCamera.LookAt = null;
        // 解除時はFollowをプレイヤーに戻しておいても良いが、FreeLookに切り替わるのでそのままでもOK
    }

    public Transform GetLookAtTransform()
    {
        return lockOnCamera.LookAt;
    }
}