using UnityEngine;
// using UnityEngine.InputSystem; // ※エラーが出る場合はコメント外してください

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class TestMove : MonoBehaviour
{
    InputSystem_Actions inputActions;

    [Header("移動パラメータ")]
    [SerializeField] float moveSpeed = 6.0f;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float jumpForce = 5.0f;

    [Header("接地判定")]
    [SerializeField] LayerMask groundLayer; // 地面のレイヤーを指定すること！
    [SerializeField] float groundCheckDistance = 0.1f;

    Rigidbody rb;
    CapsuleCollider capsuleCollider;
    Transform cameraTransform;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // メインカメラを取得
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 1. 生成されたInputクラスをインスタンス化
        inputActions = new InputSystem_Actions();
    }

    // 2. 必ず Enable / Disable を呼ぶのがルール
    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    [System.Obsolete]
    void Update()
    {
        // 接地チェック
        CheckGround();

        // ジャンプ処理 (Updateで入力を拾う)
        if (inputActions.Player.Jump.triggered && isGrounded)
        {
            // Y軸の速度をリセットしてから跳ぶ（挙動安定のため）
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 入力値を取得
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        MoveAndRotate(input);
    }

    void MoveAndRotate(Vector2 input)
    {
        if (cameraTransform == null) return;

        // 入力がなければ停止（慣性を殺してピタッと止める）
        if (input.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        // --- カメラ基準のベクトル変換 ---
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // 水平方向のみにする
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 進行方向の決定
        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        // --- 移動 (Velocity書き換え) ---
        Vector3 targetVelocity = moveDir * moveSpeed;
        // 重力(Y)は今の値を維持する
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // --- 回転 ---
        // 少しでも移動していたら向きを変える
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void CheckGround()
    {
        // カプセルの底より少し下をチェック
        float checkDist = (capsuleCollider.height * 0.5f) - capsuleCollider.radius + groundCheckDistance;
        // ※シンプルなCheckSphere方式
        isGrounded = Physics.CheckSphere(transform.position + Vector3.up * capsuleCollider.radius,
                                         capsuleCollider.radius + groundCheckDistance,
                                         groundLayer);
    }
}