using UnityEngine;

public class GameClearSceneManager : SceneManagerBase
{
   
    // Update is called once per frame
    void Update()
    {
        if (actions.UI.Next.WasPressedThisFrame())
        {
            ChangeScene(SceneState.Game); // ゲームシーンへ
        }
        else if (actions.UI.Back.WasPressedThisFrame())
        {
            ChangeScene(SceneState.Titel); // タイトルシーンへ
        }
        else if (actions.UI.Exit.WasPressedThisFrame())
        {
            EndGame(); // タイトルでEscボタンでゲーム終了()今後ボタンにするかも
        }
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }
}
