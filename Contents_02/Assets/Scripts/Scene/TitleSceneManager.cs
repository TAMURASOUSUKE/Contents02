using UnityEngine;

/*
    タイトルシーンの遷移を作る
*/

public class TitleSceneManager : SceneManagerBase
{
    
    void Update()
    {
        if (actions.UI.Next.WasPressedThisFrame())
        {
            ChangeScene(SceneState.Game); // ゲームシーンへ
        }
        else if (actions.UI.Exit.WasPressedThisFrame())
        {
            EndGame(); // Escボタンでゲーム終了()今後ボタンにするかも
        }
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }
}
