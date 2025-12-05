using UnityEngine;

[CreateAssetMenu(fileName = "SO_FieldData", menuName = "Scriptable Objects/SO_FieldData")]
public class SO_FieldData : ScriptableObject
{
    [Header("解像度が高いオブジェクトを入れてください。")]
    public GameObject[] level01; // 解像度が高いFieldPrefab
    [Header("解像度が低いオブジェクトを入れてください。")]
    public GameObject[] level02; // 解像度が低いFieldPrefab
    [Header("フィールドの横幅になります。")]
    public int width = 10;
    [Header("フィールドの奥行きになります。")]
    public int depth = 10;

    // インデックの計算責任を持たせる
    int ToIndex(int x_, int z_) => z_ * width + x_; // インデックス計算


    // 配列の自動セットアップを行う関数
    // ContextMenu = コンポーネント右上の三点リーダーから関数を実行できるようにするもの
    [ContextMenu("Auto Resize & LabelArrays")]
    void SetUp()
    {
        int size = width * depth; // 配列の長さ定義
        level01 = new GameObject[size]; // 配列の長さをマップサイズに整える
        level02 = new GameObject[size]; // 配列の長さをマップサイズに整える
        Debug.Log($"配列の初期化完了({width} × {depth} = {size})");
        for (int z = 0; z < depth; z++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                int index = ToIndex(x, z);
                row += $"({x}, {z})";
            }
            Debug.Log(row);
        }
    }

    // Level01オブジェクトを返す関数(配列以上の長さ若しくはインデックスを負の値で指定するとnullを返します)
    public GameObject GetLevel01(int x_, int z_)
    {
        int index = ToIndex(x_, z_);
        if (index < 0 || index >= level01.Length) return null;
        return level01[index];
    }

    // Level02オブジェクトを返す関数(配列以上の長さ若しくはインデックスを負の値で指定するとnullを返します)
    public GameObject GetLevel02(int x_, int z_)
    {
        int index = ToIndex(x_, z_);
        if (index < 0 || index >= level02.Length) return null;
        return level02[index];
    }


    // スケールはすべて統一するので0で返しても問題はない
    public Vector3 GetScaleLevel01()
    {
        return level01[0].transform.localScale;
    }
    // スケールはすべて統一するので0で返しても問題はない
    public Vector3 GetScaleLevel02()
    {
        return level02[0].transform.localScale;
    }
}
