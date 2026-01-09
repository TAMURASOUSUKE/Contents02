using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera freeLookCamera;
    [SerializeField] CinemachineCamera lockOnCamera;
    [SerializeField] Transform playerTransform; // プレイヤーのTransform
    [SerializeField] CinemachineOrbitalFollow freeLookOrbital;

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
        // 角度を合わせる
        SynchronizeFreeLookAngles();

        lockOnCamera.Priority = LockOnCameraInactivePriority;
        lockOnCamera.LookAt = null;
    }

    // 現在のカメラの角度をFreeLookカメラのコントローラーに入れる
    void SynchronizeFreeLookAngles()
    {
        if (freeLookCamera == null) return;

        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 playerPos = playerTransform.position;

        float currentDistance = Vector3.Distance(cameraPos, playerPos);
        freeLookOrbital.Radius = currentDistance;

        // カメラから見てプレイヤーはどこにいるかを知る
        Vector3 direction = playerPos - cameraPos;

        // 座標から角度を度数法で出す
        float locationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        freeLookOrbital.HorizontalAxis.Value = locationAngle;

        float heightDiff = cameraPos.y - playerPos.y;

        float horizontalDist = new Vector3(direction.x, 0, direction.z).magnitude;
        float heightAngle = Mathf.Atan2(heightDiff, horizontalDist) * Mathf.Rad2Deg;

        freeLookOrbital.VerticalAxis.Value = heightAngle;



        // 切り替えの瞬間はダンピングを無視する
        freeLookCamera.PreviousStateIsValid = false;
        
    }

    public Transform GetLookAtTransform()
    {
        return lockOnCamera.LookAt;
    }
}