using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


/*
    ゲームシーンの切り替え等を行う
*/

public class GameSceneManager : SceneManagerBase
{
    [Header("シーン遷移")]
    [SerializeField] Image image;
    [Header("ポーズ画面")]
    [SerializeField] GameObject quitButton; // ゲームを終了する際のボタン
    [SerializeField] Volume volume; // ポーズ時のポストプロセス用
    [SerializeField] Image quitButtonImage; // ゲームを終了するボタンの画像
    [SerializeField] int rePauseTime = 0; // ポーズ連打を防ぐための時間(ここに設定した値が次にポーズを開けるまでの時間になります)

    int currentRePauseTime = 0; // 実際に減らしていく時間
    bool isPause = false; // ポーズ画面かどうかを判断するフラグ
    bool isProcessing = false; // フェード中か同かを判断するフラグ
    /*
        現状プレイヤー等がないためシーン遷移の条件はボタンにしておく
    */

    protected override void Awake()
    {
        base.Awake();
        CreateCloneMaterial(image);
        CreateCloneMaterial(quitButtonImage);
    }

    // 最初にフェードアウトさせる
    private void Start()
    {
        StartCoroutine(StartSceneFade(fadeTime, true, image , ChangeMaterialValue));
    }

    void Update()
    {
        // 条件が作れるまでボタンでクリアシーンに飛ぶようにしておく
        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.GameClear, image));
        }

        // ポーズ画面処理
        if (actions.UI.Pause.WasPressedThisFrame() && !isPause && currentRePauseTime <= 0 && !isProcessing)
        {
            StartCoroutine(SetPause(false));  
        }
        // ポーズ画面解除処理
        else if(actions.UI.Pause.WasPressedThisFrame() && isPause && !isProcessing)
        {
            StartCoroutine(SetPause(true));
        }

        CalcRePauseTime(); // ポーズ解除時の制限時間を計算する
    }

    // ポーズ解除時の制限時間を計算する
    void CalcRePauseTime()
    {
        // ポーズの制限時間が発生している時はマイフレーム減らす
        if (currentRePauseTime > 0)
        {
            currentRePauseTime--;
            // 負の値にならないようにガードする
            if (currentRePauseTime <= 0)
            {
                currentRePauseTime = 0;
            }
        }
    }

    // ポストプロセス用の値を調整する
    protected override void ChangeMaterialValue(float val, Image targetImage)
    {
        if(isProcessing) volume.weight = val;
        base.ChangeMaterialValue(val, targetImage);
    }

    // ポーズ時やポーズ解除時に使う関数引数にフェードアウトさせるかを入れる
    IEnumerator SetPause(bool isOut)
    {
        isProcessing = true; // フェードが始まったらtrue

        if(!isOut) quitButton.SetActive(true); // ポーズ状態なのでボタン表示

        yield return StartCoroutine(Fade(0.2f, isOut, quitButtonImage ,ChangeMaterialValue));


        if (isOut)
        {
            Time.timeScale = 1.0f; // 時間を正常に動かす
            isPause = false; // ポーズ状態を解除する
            quitButton.SetActive(false); // ポーズ状態の解除なのでボタンも消す
            currentRePauseTime = rePauseTime;
        }
        else
        {
            Time.timeScale = 0.0f; // 時間を止める
            isPause = true; // ポーズ状態にする
        }


        isProcessing = false; // フェードが終了したらfalse
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }
}
