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
        StartCoroutine(StartSceneFade(fadeTime, true, image ,ChangeMaterialValue));
    }
    void Update()
    {
        if (isFading) return; // フェード中は操作を受け付けない

        if (actions.UI.Next.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Game, image));
        }
        if (actions.UI.Back.WasPressedThisFrame())
        {
            StartCoroutine(TransitionSequence(SceneState.Titel, image));
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
}
