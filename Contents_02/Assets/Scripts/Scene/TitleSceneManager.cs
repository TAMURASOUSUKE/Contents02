using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    タイトルシーンの遷移を作る
*/

public class TitleSceneManager : SceneManagerBase
{
    [SerializeField] Image imagePlanel;
    protected override void Awake()
    {
        base.Awake();

        CreateCloneMaterial(imagePlanel); // materialの複製を作る
    }

    // 最初にフェードアウトさせる
    private void Start()
    {
        StartCoroutine(StartSceneFade(fadeTime, true, imagePlanel ,ChangeMaterialValue));
    }

    void Update()
    {
        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Game, imagePlanel));
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
