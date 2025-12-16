using UnityEngine;
using UnityEngine.Scripting.APIUpdating;


public class PlayerContoroller : MonoBehaviour
{
    //----------------------------定数------------------------------
    [Header("プレイヤーの速度上限")]
    //歩く速度の上限
    [SerializeField] private const float WALK_SPEED = 2.0f;
    //走る速度の上限
    [SerializeField] private const float DASH_SPEED = 3.5f;
    //しゃがんだ時の速度上限
    [SerializeField] private const float CROUCH_SPEED = 1.0f;
    //スライディング時の速度上限
    [SerializeField] private const float SLIDING_SPEED = 4.0f;

    [Header("プレイヤーに加える力")]
    //加速度
    [SerializeField] private const float ACCELERATION = 3.0f;
    //ジャンプ力
    [SerializeField] private const float JUMP_FORCE = 3.0f;


    Rigidbody rb;

    //----------------------------変数------------------------------
    private float speed;
    private Vector3 moveVec = Vector3.zero;

    //ダッシュのフラグ
    private bool isSprint;
    //しゃがみのフラグ
    private bool isCrouch;
    //ジャンプのフラグ
    private bool isJump;
    //スライディングのフラグ
    private bool isSliding;

    protected InputSystem_Actions moveAcions;

    //----------------------------状態------------------------------

    enum moveState {
    None,
    Walk,
    Dash,
    Crouch,
    Sliding
    }

    //--------------------------------------------------------------

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAcions = new InputSystem_Actions();
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



    public void Move()
    {
        // 入力の受け取り
        var vec = moveAcions.Player.Move.ReadValue<Vector2>();
        var spr = moveAcions.Player.Sprint.ReadValue<bool>();
        var crch = moveAcions.Player.Crouch.ReadValue<bool>();
        var jmp = moveAcions.Player.Jump.ReadValue<bool>();

        if (spr)
        {
            isSprint = !isSprint;
        }
        if (crch)
        {
            isCrouch = !isCrouch;
        }

        // ベクトルの生成
        moveVec = vec;

        // 正規化
        float length = Mathf.Sqrt((moveVec.x * moveVec.x) + (moveVec.y * moveVec.y));
        moveVec.x = moveVec.x / length;
        moveVec.y = moveVec.y / length;

        // 単位ベクトルに速度をかける
        if (spr)
        {
            moveVec = moveVec * DASH_SPEED;
        }
        else if (crch)
        {

        }
        // 位置に値を追加していく

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
