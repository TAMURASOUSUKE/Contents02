using UnityEditor;
using UnityEngine;


// FieldDataのSOを外から見たときにわかりやすくするために説明文を追加する
// typeof = 型そのものの情報を取得
[CustomEditor(typeof(SO_FieldData))]
public class Cst_FieldData : Editor
{
    public override void OnInspectorGUI()
    {
        // タイトル + 説明を書く(文字スタイルをboldに設定)
        EditorGUILayout.LabelField("フィールドデータ設定", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "このデータはフィールド生成を行う際に使うデータ群です\n" +
            "・Level01 : 高解像度のフィールドプレファブを設定\n" +
            "・Level02 : 低解像度のフィールドプレファブを設定\n" +
            "・Width : フィールドの横幅\n" +
            "・Depth : フィールドの奥行\n" +
            "Width × Depthがフィールを構成するサイズになります。\n\n\n" +
            "各グリッド一つ一つの縦幅と横幅は統一してください。\n" +
            "{(0,0) (1,0) (2,0) (3,0) (4,0) (5,0)...}\n" +
            "{(0,1) (1,1) (2,1) (3,1) (4,1) (5,1)...}\n" +
            "{(0,2) (1,2) (2,2) (3,2) (4,2) (5,2)...}\n" +
            "{(0,3) (1,3) (2,3) (3,3) (4,3) (5,3)...}\n" +
            "...\n" +
            "としたときに、オブジェクトを入れる順番は\n" +
            "(0,0)\n" +
            "(1,0)\n" +
            "(2,0)\n" +
            "(3,0)\n" +
            "(4,0)\n" +
            "(5,0)\n" +
            "(0,1)\n" +
            "...\n" +
            "のような流れで入れてください\n\n\n" +
            "下のボタンから『Auto Resize & LabelArrays』を実行すると配列データを初期化できます。\n",
            MessageType.Info
            );


        // 基底クラスのOnInspectorGUIを呼び出しUnity標準のインスペクターを描画する
        base.OnInspectorGUI(); // base = 基底クラスを意味する


        EditorGUILayout.Space(10); // 区切り線を付ける(10ピクセル分)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // 横線の描画

        // ボタンを押したらSetUp関数が実行できるようにする
        SO_FieldData data = (SO_FieldData)target; // 編集中のオブジェクトを指定の型にキャストして実際の型として扱えるようにする
        if (GUILayout.Button("Auto Resize & LabelArrays 実行")) // GUILayout.Button = 押されるとtrueを返すボタンを生成する
        {
            // dataから文字列を使って関数を取得しボタンが押されたときに実行する
            // SetUpはprivate関数のため、ReflectionとBindingFlagsを使って取得する
            // Reflection.BindingFlags.NonPublic = private,protected関数まで探せるようにするもの
            // Reflection.BindingFlags.Instance = staticではなくインスタンスメソッドを探す
            data.GetType().GetMethod("SetUp",
                // OR演算を行いNonPublicとInstanceを同時に探索できるようにする
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                // Invokeを使って見つけた関数を実際に呼ぶ(第一引数に呼び出す対象, 代入にその関数の引数を入れる今回は引数なしの関数なのでnull)
                ?.Invoke(data, null);
        }
    }
}
