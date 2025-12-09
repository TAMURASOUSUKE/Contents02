using UnityEngine;


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
    private bool isDash;
    //しゃがみのフラグ
    private bool isCrouch;
    //ジャンプのフラグ
    private bool isJump;
    //スライディングのフラグ
    private bool isSliding;

    //----------------------------状態------------------------------

    enum moveState {
    None,
    Walk,
    Dash,
    Crouch,
    Sliding
    }

    //--------------------------------------------------------------

    private void Move()
    {

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
