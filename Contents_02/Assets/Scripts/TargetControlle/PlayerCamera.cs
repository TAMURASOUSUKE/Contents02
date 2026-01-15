using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera freeLookCamera;
    [SerializeField] CinemachineCamera lockOnCamera;
    [SerializeField] Transform playerTransform; // プレイヤーのTransform
    [SerializeField] CinemachineOrbitalFollow freeLookOrbital;
    [SerializeField] CinemachineOrbitalFollow lockOnOrbital;
    [SerializeField] float cameraOffSet = 40.0f;

    readonly int LockOnCameraActivePriority = 11;
    readonly int LockOnCameraInactivePriority = 0;

    private void LateUpdate()
    {
        if(lockOnCamera.Priority == LockOnCameraActivePriority && lockOnCamera.LookAt != null)
        {
            UpdateLockOnOrbitalPosition();
        }

    }

    public void ResetFreeLookCamera()
    {
        if(freeLookCamera == null || playerTransform == null) return;

        // プレイヤーのY軸回転を取得する
        float playerAngleY = playerTransform.rotation.eulerAngles.y;

        // 補正する
        float targetAngle = playerAngleY;

        // 代入する
        freeLookOrbital.HorizontalAxis.Value = targetAngle;
        // 高さも補正する
        freeLookOrbital.VerticalAxis.Value = 20;

        // ダンピングを無視する
        freeLookCamera.PreviousStateIsValid = false;

    }

    // 常に「敵 -> プレイヤー -> カメラ」の並びになるよう角度を更新し続ける
    void UpdateLockOnOrbitalPosition()
    {
        if (lockOnOrbital == null || playerTransform == null) return;

        Transform enemy = lockOnCamera.LookAt;
        if(enemy == null) return;

        // プレイヤーから敵へのベクトルを出す
        Vector3 direction = enemy.position - playerTransform.position;

        //　水平方向だけとりだす
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);

        // 水平方向の距離が近すぎると計算せずに返す
        if(horizontalDirection.magnitude < 1.0f)
        {
            return;
        }

        // それをもとに角度を出す
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // カメラの位置に補正を行いちょうどいい位置に持っていく
        targetAngle += cameraOffSet;

        // 現在のシネマシーンの角度を取得
        float currentAngle = lockOnOrbital.HorizontalAxis.Value;
        // 現在地から目的地への差分計算
        float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

        float smoothT = Time.deltaTime * 5.0f;
        // 毎フレームの更新
        lockOnOrbital.HorizontalAxis.Value = Mathf.Lerp(currentAngle, currentAngle + deltaAngle, smoothT);
    }

    public void ActiveLockOnCamera(GameObject target)
    {
        lockOnCamera.Priority = LockOnCameraActivePriority;

        // LookAt（注視点）は敵のまま
        lockOnCamera.LookAt = target.transform;
        lockOnCamera.Follow = playerTransform;
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