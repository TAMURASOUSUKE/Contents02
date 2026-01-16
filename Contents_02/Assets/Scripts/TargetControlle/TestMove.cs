using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class PlayerMovementTest : MonoBehaviour, ISkillReceiver
{
    private InputSystem_Actions inputActions;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 0.1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float speedUpTime = 10.0f;
    [SerializeField] SkillManager skillManager;
    [SerializeField] PlayerLockOn lockOn;
    [SerializeField] PlayerCamera playerCamera; // スピード変更時のカメラ制御を行う

    StatusController statusController;
    CharacterController controller;
    Transform cameraTransform;
    Vector3 verticalVelocity; // 垂直方向（重力）の速度
    float turnSmoothVelocity;
    bool isGrounded;
    bool selectButtonReturnFlag = true; // スキル選択ボタンの連打防止フラグ

    private void Awake()
    {
        statusController = new StatusController();
        inputActions = new InputSystem_Actions();
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        // 1. 移動計算（横方向）
        Vector3 moveVector = CalculateMovement();

        // 2. 重力計算（縦方向）
        CalculateGravity();

        // 3. 最終的な移動（合成して1回だけMoveする）
        // (横移動 * スピード) + (縦移動)
        Vector3 finalMovement = (moveVector * (statusController.Has(SkillMasks.SpeedUp) ? moveSpeed * 10.0f : moveSpeed)) + verticalVelocity;
        controller.Move(finalMovement * Time.deltaTime);

        float inputSelectValue = inputActions.Player.SelectCommand.ReadValue<float>(); // 入力を受け取る 
        // デバイスによって入力方法を分ける
        bool isMouseSelection = false;
        if (inputActions.Player.SelectCommand.activeControl != null)
        {
            isMouseSelection = inputActions.Player.SelectCommand.activeControl.device is Mouse;
        }

        // 入力がある場合
        if(Mathf.Abs(inputSelectValue) > 0.1f)
        {
            // マウスの場合
            if (isMouseSelection)
            {
                skillManager.MoveSelection((int)inputSelectValue);
            }
            // パッドの場合
            else
            {
                // 連続入力されていなければ
                if (selectButtonReturnFlag)
                {
                    skillManager.MoveSelection((int)inputSelectValue);
                    selectButtonReturnFlag = false; // ロックをかける
                }
            }
        }
        // 入力がない場合は
        else
        {
            selectButtonReturnFlag = true; // ロック解除
        }


        // 発動したら通知を送る
        if (inputActions.Player.InteractCommand.WasPressedThisFrame())
        {
            SkillContext skillContext;
            skillContext.user = this.gameObject;
            skillContext.target = lockOn.TargetObj;
            if (lockOn.TargetObj != null)
            {
                skillContext.hitPosition = lockOn.TargetObj.transform.position;
            }
            else
            {
                skillContext.hitPosition = this.gameObject.transform.position;
            }
            skillContext.condition = skillManager.GetCurrentSkill();
            skillManager.InputSkillContext(skillContext);
        }

        if (statusController.Has(SkillMasks.SpeedUp))
        {
            Debug.Log("スピードアップ中");
            playerCamera.SpeedUPFOV();
        }
        else
        {
            Debug.Log("通常速度");
            playerCamera.ResetFOV();
        }
    }

    // 重力計算（Moveはしない）
    void CalculateGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // 接地時のリセット
        }

        verticalVelocity.y += gravity * Time.deltaTime;
    }

    // 移動方向の計算をしてベクトルを返す
    private Vector3 CalculateMovement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        if (input.sqrMagnitude >= 0.01f)
        {
            Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotateSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // normalizedされた方向ベクトルを返す
            return moveDir.normalized;
        }

        return Vector3.zero; // 入力がないときは動かない
    }

    // スキルを受け取る
    public void OnReceiveSkill(SkillContext skillContext_)
    {
        if (skillContext_.target != gameObject) return;

        if(skillContext_.condition == SkillMasks.SpeedUp)
        {
            statusController.AddTimedEffect(skillContext_.condition, speedUpTime);
        }
    }
}