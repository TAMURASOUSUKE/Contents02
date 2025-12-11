using UnityEngine;


/*
    ゲームシーンの切り替え等を行う
*/

public class GameSceneManager : SceneManagerBase
{
    /*
        現状プレイヤー等がないためシーン遷移の条件はボタンにしておく
    */

    void Update()
    {
        // 条件が作れるまでボタンでクリアシーンに飛ぶようにしておく
        if (actions.UI.Next.WasPressedThisFrame())
        {
            ChangeScene(SceneState.GameClear);
        }

        /*
            ポーズ画面等の処理は今後行う
        */
        
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }
}
