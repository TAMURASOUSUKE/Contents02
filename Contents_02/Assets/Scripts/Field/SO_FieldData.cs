using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_FieldData", menuName = "Scriptable Objects/SO_FieldData")]
public class SO_FieldData : ScriptableObject
{

    [Header("フィールド設計")]
    public int width = 31; // 幅
    public int depth = 31; // 奥行
    public Vector3 baseScale = Vector3.one;

    [Header("フィールドの端になるオブジェクト群")]
    public GameObject[] edgePrefabHigh; // ハイモデル
    public GameObject[] edgePrefabLow; // ローモデル

    [Header("1×1の埋め尽くし用")]
    public GameObject[] fillerPrefabHigh;
    public GameObject[] fillerPrefabLow;

    [Header("パターンの定義")]
    public List<MapPattern> patterns;

    [Header("ランダム生成のルール(サイズが大きいものから入れてください)")]
    public List<RandomSpawnRule> spawnRules;



    // ==============================以下はクラス定義やヘルパー関数==========================================

    /// <summary>
    /// マップのパターン(2 * 2や 3 * 3のフィールド塊データを作る)
    /// </summary>
    [System.Serializable]
    public class MapPattern
    {
        [Header("識別用のID")]
        public string id; // それぞれのデータを識別できるようにするID
        [Header("パターンのサイズ(size×size分作られます)")]
        [Range(1, 10)] public int size; // ここで設定した値の塊を作る(必ずsize * sizeになることに留意)
        [Header("パターンを構成する各ハイモデル")]
        public GameObject[] partsHigh; // それぞれのハイモデル
        [Header("パターンを構成する各ローモデル")]
        public GameObject[] partsLow; // それぞれのローモデル
    }


    /// <summary>
    /// ランダム生成する際のルールを決定する
    /// </summary>
    [System.Serializable]
    public class RandomSpawnRule
    {
        [Header("識別用のID")]
        public string patternId; // パターンの識別ID
        [Header("生成個数")]
        public int count; // 何個まで生成するのか
        [Header("生成を行う際の最大試行回数")]
        public int maxAttempts = 100; // 生成を何回まで試行するのか
        [Header("このルールで配置されたオブジェクトどうしの最低距離(0なら制限なし)")]
        public float minDistance = 0.0f; // 生成の際の距離制限
    }

    /// <summary>
    /// マップパターンのIDを取得するGetter
    /// </summary>
    /// <param name="id">パターンを識別するID</param>
    /// <returns>IDに応じたパターンを返す</returns>
    public MapPattern GetMapPatternByID(string id) => patterns.Find(p => p.id == id);

    /*
        タプルを使って外枠や中身を埋めつくす用のプレファブを
        ハイモデルローモデル同時に取得する
     */

    /// <summary>
    /// ランダムに取り出した外枠を取得する
    /// </summary>
    /// <returns>ハイモデルのオブジェクトとローモデルのオブジェクトを同時に返す</returns>
    public (GameObject high, GameObject low) GetRandomEdgePrefab()
    {
        if (edgePrefabHigh == null || edgePrefabLow == null) return (null, null); // 外枠用のプレファブが入っていない時はnullを返す
        int index = Random.Range(0, edgePrefabHigh.Length); // 配列からランダムに取り出す
        return (edgePrefabHigh[index], edgePrefabLow[index]);
    }

    /// <summary>
    /// ランダムに取り出した地面を取得する
    /// </summary>
    /// <returns>ハイモデルのオブジェクトとローモデルのオブジェクトを同時に返す</returns>
    public (GameObject high, GameObject low) GetRandomFillerPrafab()
    {
        if (fillerPrefabHigh == null || fillerPrefabLow == null) return (null, null); // 地面用のプレファブが入っていないならnullを返す
        int index = Random.Range(0, fillerPrefabHigh.Length); // 配列からランダムに取り出す
        return (fillerPrefabHigh[index], fillerPrefabLow[index]);
    }
}
