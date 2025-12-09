using Unity.VisualScripting;
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

public abstract class SceneManagerBase : MonoBehaviour
{


    protected InputSystem_Actions actions; // インプットシステムの変数
    protected SceneState state = SceneState.None; // シーン状態のインスタンス 
    string nextSceneName = null; // 次シーンの名前  

    protected virtual void Awake()
    {
        Application.targetFrameRate = 60;
        actions = new InputSystem_Actions(); // インプットシステムのインスタンスを作る
    }


    protected virtual void OnEnable()
    {
        actions.Enable(); // インプットシステムの有効化
    }

    protected virtual void OnDisable()
    {
        actions.Disable(); // インプットシステムの無効化
    }

    protected virtual void OnDestory()
    {
        actions.Dispose(); // インプットシステムの解放
    }

    // 引数に入れたシーンに飛ぶようにする
    protected virtual void ChangeScene(SceneState scene)
    {
        nextSceneName = ConvertToSceneName(scene); // 次のシーンの名前取得
        SceneManager.LoadScene(nextSceneName); // 次のシーンに移動
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
