using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementTest : MonoBehaviour
{
    // ========================================================================
    // ★重要★: 生成したInput Systemのクラス名に書き換えてください
    // 例: private PlayerControls inputActions;
    private InputSystem_Actions inputActions;
    // ========================================================================

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 0.1f; // 回転のスムーズさ（秒）
    [SerializeField] private float gravity = -9.81f;

    // 内部変数
    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 playerVelocity; // 重力落下用
    private float turnSmoothVelocity; // 回転計算用の一時変数
    private bool isGrounded;

    private void Awake()
    {
        // インスタンス生成（クラス名を合わせる）
        inputActions = new InputSystem_Actions();

        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        HandleGravity();
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Input Systemから入力を取得 (Vector2)
        // "Player" や "Move" はInput Actionsの設定名に合わせてください
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        // 入力がある場合のみ移動処理を行う
        if (input.sqrMagnitude >= 0.01f)
        {
            // 1. 入力値を3Dベクトルに変換（Yは0）
            // Normalizeしないと斜め移動が速くなる可能性があるが、InputSystemの設定次第
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

            // 2. カメラの向きを考慮した進行方向の角度を計算
            // Atan2(x, z) で入力の角度を求め、カメラのY軸回転を加算する
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // 3. キャラクターの向きをスムーズに回転させる
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotateSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 4. 計算した角度の方向に移動ベクトルを作成
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // 5. CharacterControllerで移動
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        // 接地判定
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // 接地時は少しだけ下向きの力を残して浮き上がりを防ぐ
        }

        // 重力加算
        playerVelocity.y += gravity * Time.deltaTime;

        // 落下移動
        controller.Move(playerVelocity * Time.deltaTime);
    }
}