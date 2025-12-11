using UnityEngine;

public class GameOverSceneManager : SceneManagerBase
{

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
}
