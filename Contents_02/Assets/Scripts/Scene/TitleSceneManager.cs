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
        StartCoroutine(Fade(1.0f, true, ChangeMaterialValue));
    }

    void Update()
    {
        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Game));
        }
        else if (actions.UI.Exit.WasPressedThisFrame())
        {
            EndGame(); // Escボタンでゲーム終了()今後ボタンにするかも
        }
    }

    // materialのプロパティを変更する関数
    protected override void ChangeMaterialValue(float val)
    {
        imagePlanel.material.SetFloat("_Threshold", val);
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }

}
