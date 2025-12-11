using UnityEngine;
using UnityEngine.UI;

public class GameClearSceneManager : SceneManagerBase
{
    [SerializeField] Image image;

    protected override void Awake()
    {
        base.Awake();
        CreateCloneMaterial(image);
    }

    // 最初にフェードアウトさせる
    private void Start()
    {
        StartCoroutine(Fade(fadeTime, true, ChangeMaterialValue));
    }
    void Update()
    {
        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Game));
        }
        if (actions.UI.Back.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Titel));
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
    // materialのプロパティを変更する関数
    protected override void ChangeMaterialValue(float val)
    {
        image.material.SetFloat("_Threshold", val);
    }
}
