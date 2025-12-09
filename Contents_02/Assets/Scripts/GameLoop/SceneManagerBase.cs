using UnityEngine;
using UnityEngine.SceneManagement;

/*
    各シーンマネージャースクリプトのベースを作る
*/

public enum SceneState
{
    None = -1, // 登録なし
    Titel, // タイトル
    Game, // ゲームシーン
    GameOver, // ゲームオーバーシーン
    GameClear, // ゲームクリアシーン
}

public class SceneManagerBase
{


    protected InputSystem_Actions actions; // インプットシステムの変数
    protected SceneState state = SceneState.None; // シーン状態のインスタンス 
    string nextSceneName = null; // 次シーンの名前
    string backSceneName = null; // 戻るシーンの名前

    // コンストラクタ
    SceneManagerBase()
    {
        actions = new InputSystem_Actions(); // インプットシステムのインスタンスを作る
        actions.Enable(); // インプットシステムの有効化
    }
    // デストラクタ
    ~SceneManagerBase()
    {
        actions.Disable(); // インプットシステムの無効化
    }



    // 引数に入れたシーンに飛ぶようにする
    protected virtual void ChangeScene(SceneState next, SceneState back = SceneState.None)
    {
        nextSceneName = ConvertToSceneName(next); // 次のシーンの名前取得
        backSceneName = ConvertToSceneName(back); // 戻るシーンの名前取得


        // 次のシーンのボタンが押されたら次のシーンへ移動
        if (actions.UI.Next.WasPressedThisFrame())
        {
            SceneManager.LoadScene(nextSceneName);
        }

        // 戻るシーンのボタンが押されたら戻るシーンへ移動
        if (actions.UI.Next.WasPressedThisFrame())
        {
            SceneManager.LoadScene(backSceneName);
        }
    }

    // 入力されたステートをシーン名に変換する(ラムダ + switch式を使う)
    string ConvertToSceneName(SceneState name) => name switch
    {
        SceneState.None => null,
        SceneState.Titel => "TitleScene",
        SceneState.Game => "GameScene",
        SceneState.GameOver => "GameOverScene",
        SceneState.GameClear => "GameClear",
        _ => null, // デフォルトの代わり
    };



    // ゲームの終了
    protected void EndGame()
    {
        Application.Quit(); // ゲームの終了
    }
}
