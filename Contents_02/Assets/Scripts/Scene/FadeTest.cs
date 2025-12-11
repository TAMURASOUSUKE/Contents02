using UnityEngine;
using UnityEngine.UI;

public class FadeTest : MonoBehaviour
{
    // テスト用にインスペクターからスライダーで操作できるようにする
    [Range(0, 1)] public float testCutoff = 0.0f;

    private Image targetImage;
    private Material runtimeMaterial;

    void Start()
    {
        targetImage = GetComponent<Image>();

        // 【重要】ここでマテリアルの「複製」を作る
        // これにより、このImage専用のマテリアルになり、操作が確実に反映されるようになる
        runtimeMaterial = Instantiate(targetImage.material);
        targetImage.material = runtimeMaterial;
    }

    void Update()
    {
        if (runtimeMaterial != null)
        {
            // シェーダーのプロパティ名を指定して数値を送る
            // ※Shader Graphで作ったプロパティ名（Reference）に合わせてください
            // "_Cutoff" ではなく "Cutoff" かもしれません。GraphのBlackboardを確認！
            runtimeMaterial.SetFloat("_Threshold", testCutoff);

            // 【念押し】もしこれでも動かなければ、これをコメントアウト解除してください
            targetImage.SetMaterialDirty(); 
        }
    }

    // 終わったらメモリ解放（コピーを作った責任）
    void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}