using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    protected virtual void OnDestroy()
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
        SceneState.GameClear => "GameClearScene",
        _ => null, // デフォルトの代わり
    };


    
    // floatの値が欲しいが綺麗なフェードインやアウトを作ろうとするとIEnumeratorを返す必要があるためAction<>(関数ポインタのようなもの)で対応する
    // フェードイン関数 : 第一引数にフェードが完了するまでの時間を入れる, 第二引数はフェードアウトさせるかどうか(falseならInになります) ,第三引数はフェードさせたい値を操作する関数を入れる(0～1で値を返します)
    protected IEnumerator Fade(float fadeInTime, bool isOut ,Action<float> fadeValue)
    {
        float elapsedTime = 0.0f; // 経過時間
        fadeValue(0.0f); // 初期化

        // 設定時間を超えるまでループする
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime; // 時間を経過させる
            float t =  Mathf.Clamp01(elapsedTime / fadeInTime);  // 現在の時間 / 設定時間で進行度を計算する

            // フラグがtrueならフェードアウトさせる
            // 1から引くことで値を逆転させている
            if (isOut)
            {
                 t = 1.0f - t;
            } 
            fadeValue(t); // 引数で受け取った関数に結果を渡す
            yield return null; // 1フレーム待機
        }

        fadeValue(isOut ? 0.0f : 1.0f); // 保険としてループを抜けた後0になるようにする
    }

    // materialの複製を作る関数第一引数にマテリアルを適用したいコンポーネントの変数
    protected void CreateCloneMaterial(Image image)
    {
        Material mat = Instantiate(image.material); // マテリアルを複製する
        image.material = mat; // 複製したマテリアルを代入する
    }

    protected IEnumerator TransitionSequence(SceneState nextScene)
    {
        actions.Disable();
        yield return StartCoroutine(Fade(1.0f, false, ChangeMaterialValue));

        ChangeScene(nextScene); // 次のシーンへ
    }

    // materialのプロパティを変更する関数
    protected abstract void ChangeMaterialValue(float val);


    // ゲームの終了
    protected void EndGame()
    {
        Application.Quit(); // ゲームの終了
    }
}
