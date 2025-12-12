using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/*
    ゲームシーンの切り替え等を行う
*/

public class GameSceneManager : SceneManagerBase
{
    [Header("シーン遷移")]
    [SerializeField] Image image;
    [Header("ポーズ画面")]
    [SerializeField] Image[] quitButtonImage; // ゲームを終了するボタンの画像
    [SerializeField] Volume volume; // ポーズ時のポストプロセス用
    [SerializeField] int rePauseTime = 0; // ポーズ連打を防ぐための時間(ここに設定した値が次にポーズを開けるまでの時間になります)
    [SerializeField] float pauseFadeTime = 0.2f; // ポーズの際のフェードにかける時間(秒)

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

        // ポーズ画面用のマテリアル複製を作成する
        for (int i = 0; i < quitButtonImage.Length; i++)
        {
            CreateCloneMaterial(quitButtonImage[i]);
        }
    }

    // 最初にフェードアウトさせる
    private void Start()
    {
        StartCoroutine(StartSceneFade(fadeTime, true, image , ChangeMaterialValue));
    }

    void Update()
    {
        if (isFading) return; // フェード中は操作を受け付けない

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

        if (!isOut)
        {
            // すべてのボタンを表示させる
            for (int i = 0; i < quitButtonImage.Length; i++)
            {
                quitButtonImage[i].gameObject.SetActive(true); // ポーズ状態なのでボタン表示
            }

            EventSystem.current.SetSelectedGameObject(null); // 一度選択状態を解除してから
            EventSystem.current.SetSelectedGameObject(quitButtonImage[0].gameObject); // 最初のボタンを選択状態にする
        }

        // すべてのボタンのフェードを開始させる
        for (int i = 0; i < quitButtonImage.Length; i++)
        {
            StartCoroutine(Fade(pauseFadeTime, isOut, quitButtonImage[i], ChangeMaterialValue));
        }

        // ポーズフェード時間待機
        yield return new WaitForSecondsRealtime(pauseFadeTime);

        if (isOut)
        {
            Time.timeScale = 1.0f; // 時間を正常に動かす
            isPause = false; // ポーズ状態を解除する

            for (int i = 0; i < quitButtonImage.Length; i++)
            {
                quitButtonImage[i].gameObject.SetActive(false); // ポーズ状態の解除なのでボタンも消す 
            }

            EventSystem.current.SetSelectedGameObject(null); // 選択状態を解除する
            currentRePauseTime = rePauseTime; // ポーズ連打防止のための時間をセットする
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
