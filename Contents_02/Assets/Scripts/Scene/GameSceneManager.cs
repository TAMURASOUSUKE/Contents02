using UnityEngine;
using UnityEngine.UI;


/*
    ゲームシーンの切り替え等を行う
*/

public class GameSceneManager : SceneManagerBase
{
    [SerializeField] Image image;
    /*
        現状プレイヤー等がないためシーン遷移の条件はボタンにしておく
    */

    protected override void Awake()
    {
        base.Awake();
        CreateCloneMaterial(image);
    }

    // 最初にフェードアウトさせる
    private void Start()
    {
        StartCoroutine(Fade(1.0f, true, ChangeMaterialValue));
    }

    void Update()
    {
        // 条件が作れるまでボタンでクリアシーンに飛ぶようにしておく
        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.GameClear));
        }
        /*
            ポーズ画面等の処理は今後行う
        */

    }

    // materialのプロパティを変更する関数
    protected override void ChangeMaterialValue(float val)
    {
        image.material.SetFloat("_Threshold", val);
    }

    protected override void OnDestroy()
    {
        // ベースの破棄用関数を呼ぶ
        base.OnDestroy();
    }
}
