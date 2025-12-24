using UnityEngine;

// これをつけると自動でCharacterControllerも追加されます
[RequireComponent(typeof(CharacterController))]
public class TestMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] float moveSpeed = 6.0f; // 移動速度
    [SerializeField] float turnSpeed = 10.0f; // 回転速度

    CharacterController characterController;
    Transform cameraTransform;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // メインカメラの位置情報を取得
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("メインカメラが見つかりません！タグがMainCameraになっているか確認してください");
        }
    }

    void Update()
    {
        // WASD入力 (または矢印キー)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 入力がある時だけ処理する
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            // 1. カメラの向きを基準にする
            // カメラの前方ベクトルを取得（Y軸=高さ情報は捨てる）
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            // カメラの右方向ベクトルを取得
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // 2. 進む方向を合成する
            // (カメラの前 * 縦入力) + (カメラの右 * 横入力)
            Vector3 moveDirection = (camForward * v + camRight * h).normalized;

            // 3. キャラクターを移動させる
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 4. キャラクターの向きを進む方向に向ける
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 簡易的な重力（床から浮かないようにするためだけ）
        characterController.Move(Physics.gravity * Time.deltaTime);
    }
}