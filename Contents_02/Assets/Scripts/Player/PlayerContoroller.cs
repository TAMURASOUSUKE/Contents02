using System.Collections;
using Unity.Cinemachine;
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

    //接地判定するレイキャストの長さ
    [SerializeField] private float CEARCH_GROUND_REYCAST = 0.2f;
    //接地判定するレイキャストのオフセット
    [SerializeField] private Vector3 GROUND_REYCAST_OFFSET;
    //立った時に天井にぶつからないかチェックするレイキャストの長さ
    [SerializeField] private float CEARCH_CEILING_REYCAST = 0.2f;
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

    protected InputSystem_Actions moveAcions;

    private bool isCrouchAvailable = true;
    private Transform cameraRootTransform;
    private float standingCameraHeight;
    private Vector3 standingCenter;

    private float crouchingCameraHeight;
    private Vector3 crouchingCenter;


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

        standingCenter = currentPos;

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


        if(moveVec.sqrMagnitude > 0)
        {
            mState = moveState.Walk;
        }

        if (moveAcions.Player.Sprint.triggered)
        {
            isSprint = !isSprint;
        }

        if (moveAcions.Player.Crouch.triggered)
        {
            if(isCrouchAvailable)
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

        if (moveAcions.Player.Jump.triggered)
        {
            isJump = true;
        }

        if (moveVec.sqrMagnitude == 0)
        {
            mState = moveState.None;
        }

        if(isCrouch)
        {
            mState = moveState.Crouch;
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

                moveVec = moveVec * CROUCH_SPEED;
                break;
            case moveState.Sliding:
                
                // スライディングの場合はフラグを立てる
                isSliding = true;
                break;
        }

        // 位置に値を追加していく
        //velocity = rb.linearVelocity;
        //velocity.x = moveVec.x;
        //velocity.z = moveVec.z;

        //rb.linearVelocity = velocity;

        transform.Translate(moveVec);



        currentPos = transform.position;


        // 着地判定
        Ray ray = new Ray(currentPos + GROUND_REYCAST_OFFSET, Vector3.down);
        bool isGround = Physics.Raycast(ray, CEARCH_GROUND_REYCAST);
        Debug.DrawRay(GROUND_REYCAST_OFFSET, Vector3.down * CEARCH_GROUND_REYCAST, Color.red);


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


        transform.Rotate(lookVec);


        if (isCrouch && isCrouchAvailable)
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

        // コライダーの中心
        standingCenter = new Vector3( 0.0f, 0.9f, 0.0f );
        crouchingCenter = new Vector3( 0.0f, 0.6f, 0.0f );

        // 現在の高さの取得と切り替える高さの選択(カメラ)
        float currentCameraHeight = cameraRootTransform.localPosition.y;
        float targetCameraHeight = isCrouch ? crouchingCameraHeight : standingCameraHeight;

        // コライダーの中心の切り替え
        capsuleCollider.center = isCrouch ? crouchingCenter : standingCenter;

        // コライダーの高さの切り替え
        capsuleCollider.height = targetHeight;

        // カメラ座標の変更
        cameraRootTransform.localPosition =
            new Vector3(
                cameraRootTransform.localPosition.x,
                targetCameraHeight,
                cameraRootTransform.localPosition.z
            );

        Debug.Log("実行中");


        isCrouchAvailable = true;
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
