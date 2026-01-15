using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;




public class PlayerContoroller : MonoBehaviour
{
    //----------------------------定数------------------------------
    [Header("プレイヤーの速度上限")]
    //歩く速度の上限
    [SerializeField] private float WALK_SPEED = 1.0f;
    //走る速度の上限
    [SerializeField] private float DASH_SPEED = 2.0f;
    //しゃがんだ時の速度上限
    [SerializeField] private float CROUCH_SPEED = 0.5f;
    //スライディング時の速度上限
    [SerializeField] private float SLIDING_SPEED = 1.5f;

    [Header("プレイヤーに加える力")]
    //ジャンプ力
    [SerializeField] private float JUMP_FORCE = 20.0f;

    [Header("プレイヤーの各姿勢時の高さ")]
    //通常時
    [SerializeField] private float NORMAL_HIGHT = 1.8f;
    //しゃがみ時
    [SerializeField] private float CROUCH_HIGHT = 1.2f;
    //スライディング時
    [SerializeField] private float SLIDING_HIGHT = 1.0f;

    [Header("レイの各種設定")]
    //接地判定するレイキャストの長さ
    [SerializeField] private float CEARCH_GROUND_REYCAST;
    //接地判定するレイキャストのオフセット
    [SerializeField] private Vector3 GROUND_REYCAST_OFFSET;
    //立った時に天井にぶつからないかチェックするレイキャストの長さ
    [SerializeField] private float CEARCH_CEILING_REYCAST;
    //立った時に天井にぶつからないかチェックするレイキャストのオフセット
    [SerializeField] private Vector3 CEILING_REYCAST_OFFSET;
    //リジッドボディのインスタンス
    Rigidbody rb;

    [SerializeField] private float duration;

    [SerializeField] private CinemachineCameraOffset cinemachineOffset;
    CameraState state;

    //----------------------------変数------------------------------
    private Vector3 currentPos;
    private float speed;
    private Vector3 moveVec = Vector3.zero;
    private Vector3 velocity;

    private Vector3 lookVec = Vector3.zero;

    //ダッシュのフラグ
    private bool isSprint;
    //しゃがみのフラグ
    private bool isCrouch;
    //ジャンプのフラグ
    private bool isJump;
    //スライディングのフラグ
    private bool isSliding;
    //着地判定のフラグ
    private bool isGround;
    //天井の有無の判定フラグ
    private bool isCeiling;
    //
    private bool isForward;

    protected InputSystem_Actions moveAcions;

    private bool isCrouchAvailable = true;
    private Transform cameraRootTransform;
    private float standingCameraHeight;
    private Vector3 standingCenter;

    private float crouchingCameraHeight;
    private Vector3 crouchingCenter;

    private float activeSlidingTime; 


    private CapsuleCollider capsuleCollider;


    //----------------------------状態------------------------------
    public enum moveState {
    None,
    Walk,
    Sprint,
    Crouch,
    Sliding
    };


    moveState mState;
    //--------------------------------------------------------------

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAcions = new InputSystem_Actions();

        if (cinemachineOffset != null)
        {
            Vector3 finalCamPosition = state.PositionCorrection;
            Debug.Log("Final Camera World Position: " + finalCamPosition);
        }


        cameraRootTransform = cinemachineOffset.transform;
        standingCameraHeight = cameraRootTransform.localPosition.y;

        float crouchingHeightRatio = CROUCH_HIGHT / NORMAL_HIGHT;
        crouchingCameraHeight = cameraRootTransform.transform.localPosition.y * crouchingHeightRatio;
        crouchingCenter = standingCenter * crouchingHeightRatio;
    }



    protected virtual void OnEnable()
    {
        moveAcions.Enable(); // インプットシステムの有効化
    }

    protected virtual void OnDisable()
    {
        moveAcions.Disable(); // インプットシステムの無効化
    }

    protected virtual void OnDestroy()
    {
        moveAcions.Dispose(); // インプットシステムの解放
    }

    public void GetMoveState()
    {

        if (!isSliding)
        {

            if (moveVec.sqrMagnitude > 0)
            {
                mState = moveState.Walk;
            }

            if (moveAcions.Player.Sprint.triggered)
            {
                isSprint = !isSprint;
            }

            if (moveAcions.Player.Crouch.triggered)
            {
                if (isCrouchAvailable)
                {
                    isCrouch = !isCrouch;
                }
            }

            if (isSprint && isCrouch)
            {
                isSliding = true;
            }
            else
            {
                isSliding = false;
            }

            if (isSprint)
            {
                mState = moveState.Sprint;
            }

            if (isSliding)
            {
                mState = moveState.Sliding;
            }

            if (moveVec.sqrMagnitude == 0)
            {
                mState = moveState.None;
            }

            if (moveAcions.Player.Jump.triggered)
            {
                isJump = true;
            }

            if (isCrouch)
            {
                mState = moveState.Crouch;
            }
        }

        Debug.Log(mState);
    }

    public void Move()
    {
        //入力の受け取り
        var vec = moveAcions.Player.Move.ReadValue<Vector2>();
        var rot = moveAcions.Player.Look.ReadValue<Vector2>();

        
        // ベクトルの生成
        moveVec = new Vector3 (vec.x, 0, vec.y);
        lookVec = new Vector3(0, rot.x, 0);

        // 正規化
        if(moveVec.sqrMagnitude > 1f)
        {
            moveVec = moveVec.normalized;
        }


        // 状態によってスピードを変更する
        switch (mState)
        {
            case moveState.None:
                break;
            case moveState.Walk:

                moveVec = moveVec * WALK_SPEED;
                break;
            case moveState.Sprint:

                moveVec = moveVec * DASH_SPEED;
                break;
            case moveState.Crouch:

                moveVec =  moveVec * CROUCH_SPEED;
                break;
            case moveState.Sliding:
                
                // スライディングの場合はフラグを立てる
                isSliding = true;
                break;
        }

        if (isSliding) 
        {
            SlidingAction();
            return;
        }

        // 位置に値を追加していく
        //velocity = rb.linearVelocity;
        //velocity.x = moveVec.x;
        //velocity.z = moveVec.z;

        //rb.linearVelocity = velocity;

        transform.Translate(moveVec);
        GROUND_REYCAST_OFFSET.y = (-capsuleCollider.height / 2) + 0.1f;

        // 着地判定
        Vector3 groundRayPos = transform.position + capsuleCollider.center + GROUND_REYCAST_OFFSET;
        isGround = Physics.Raycast(groundRayPos, Vector3.down, CEARCH_GROUND_REYCAST);
        Debug.DrawRay(groundRayPos, Vector3.down * CEARCH_GROUND_REYCAST, Color.red, 0.1f);

        transform.Rotate(lookVec);

        // ジャンプ処理
        if (isJump)
        {
            if(isGround)
            {
                rb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
                isJump = false;
            }
            else
            {
                isJump = false;
            }
        }




        if (isCrouch)
        {
            CrouchAction(true);
        }
        else
        {
            CrouchAction(false);
        }
    }

    private void CrouchAction(bool isCrouch)
    {
        // 多重呼び出しの防止
        isCrouchAvailable = false;
        
        // 現在の高さの取得と切り替える高さの選択(コライダー)
        float currentHeight = capsuleCollider.height;
        float targetHeight = isCrouch ? CROUCH_HIGHT : NORMAL_HIGHT;

        // 現在の高さの取得と切り替える高さの選択(カメラ)
        float currentCameraHeight = cameraRootTransform.localPosition.y;
        float targetCameraHeight = isCrouch ? crouchingCameraHeight : standingCameraHeight;

        // 頭上にオブジェクトがあるかの判定する用の変数
        float headClearance = 0.1f; 
        float rayLength = NORMAL_HIGHT - CROUCH_HIGHT + headClearance;
        Vector3 ceilingRayPos = transform.position + capsuleCollider.center;
        int exclude = LayerMask.GetMask("Camera");

        if (currentHeight == CROUCH_HIGHT)
        {
            // 天井判定
            isCeiling = Physics.Raycast(ceilingRayPos, Vector3.up, rayLength, ~exclude, QueryTriggerInteraction.Ignore);
            Color rayColor = isCeiling ? Color.red : Color.green;
            Debug.DrawRay(ceilingRayPos, Vector3.up * rayLength, rayColor, 0.1f);
        }

        if (isCeiling)
        {
            targetHeight = CROUCH_HIGHT;
            targetCameraHeight = crouchingCameraHeight;
        }

        // コライダーの高さの切り替え
        capsuleCollider.height = targetHeight;

        // カメラ座標の変更
        cameraRootTransform.localPosition =
            new Vector3(
                cameraRootTransform.localPosition.x,
                targetCameraHeight,
                cameraRootTransform.localPosition.z
            );

        isCrouchAvailable = true;
    }

    private void SlidingAction()
    {
        activeSlidingTime += Time.deltaTime;

        // 頭上にオブジェクトがあるかの判定する用の変数
        float headClearance = 0.1f;
        float rayLength = NORMAL_HIGHT - CROUCH_HIGHT + headClearance;
        Vector3 ceilingRayPos = transform.position + capsuleCollider.center;
        int exclude = LayerMask.GetMask("Camera");

        // 天井判定
        isCeiling = Physics.Raycast(ceilingRayPos, Vector3.up, rayLength, ~exclude, QueryTriggerInteraction.Ignore);
        Color rayColor = isCeiling ? Color.red : Color.green;
        Debug.DrawRay(ceilingRayPos, Vector3.up * rayLength, rayColor, 0.1f);

        // 前にオブジェクトがあるかの判定する用の変数
        float rayLengthForward = 0.7f;
        Vector3 forwardRayPos = transform.position + capsuleCollider.center;
        forwardRayPos.y += 0.1f;

        // 正面判定
        isForward = Physics.Raycast(forwardRayPos, Vector3.forward, rayLengthForward, ~exclude, QueryTriggerInteraction.Ignore);
        rayColor = isForward ? Color.red : Color.green;
        Debug.DrawRay(forwardRayPos, Vector3.back * rayLengthForward, rayColor, 0.1f);

        if(isForward || activeSlidingTime >= 2.0f)
        {
            activeSlidingTime = 0.0f;
            if (isCeiling)
            {
                isSprint = false;

            }
            else
            {
                isCrouch = false;

            }

            isSliding = false;
            return; ;
        }

        capsuleCollider.height = SLIDING_HIGHT;

        transform.Translate(Vector3.forward * SLIDING_SPEED, Space.Self);


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GameObjectにアタッチされているCapsuleColliderコンポーネントを取得
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        GetMoveState();
        

    }
}
