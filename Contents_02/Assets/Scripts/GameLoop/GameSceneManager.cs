using UnityEngine;


/*
    ゲームシーンの切り替え等を行う
*/

public class GameSceneManager : MonoBehaviour
{

    [SerializeField] InputSystem_Actions action; // インプットシステムの自動作成クラスを使う

    void Start()
    {
        Application.targetFrameRate = 60; // FPSを60に固定
        action.Enable(); // インプットシステムの有効化
    }

    void Update()
    {
        
    }

    void ChangeScene()
    {

    }
}
