using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;



public class CreateField : MonoBehaviour
{
    [SerializeField] SO_FieldData fieldData; // フィールドのデータ
    [SerializeField] Transform playerTransform; // プレイヤーの位置情報を参照する


    [Header("LOD設定")]
    [Header("高解像度用のプレファブを表示する距離")]
    [SerializeField] float disp01Distance = 300.0f;
    [Header("低解像度用のプレファブを表除する距離")]
    [SerializeField] float disp02Distance = 4200.0f;
    [Header("プレイヤーが高い位置に行ったときに周囲がどのくらい見えるようになるかを調整する")]
    [SerializeField, Range(0.1f, 1.0f)] float heightFactorADJ = 0.1f;
    [Header("指定したフレーム数で全体の状態を更新し描画残しを防ぐ")]
    [Header("単位 : f(フレーム)")]
    [SerializeField] int refreshFrame = 30;
    [Header("生成間隔(グリッドサイズ)")]
    [SerializeField] int generationInterval = 50;

    // オブジェクト管理配列
    GameObject[,] cellHigh; // fieldDataから取り出したハイポリを格納する変数
    GameObject[,] cellLow; // fieldDataから取り出したローポリを格納する変数

    // Transformへのアクセスを減らし高速化をする目的でキャッシュ配列を用意する
    Vector3[,] positionCache;
    Vector3 scaleCache = Vector3.one; // fieldDataから取り出したスケールをキャッシュする
    Vector2Int playerStartPosCache = Vector2Int.zero; // プレイヤーのスタート位置のキャッシュ(Vector2Int型に丸める)
   
    // 前フレームの計算キャッシュ
    int prevCenterX = int.MinValue; // 前フレームの中心X
    int prevCenterZ = int.MinValue; // 前フレームの中心Z
    int prevRadius = 0; // 前フレームの描画半径

    // Enemy探索用のコスト設定
    int edgeCost = -1;
    int fillerCost = 1;
    int[,] costMap; // コストをキャッシュする二次元配列




    void Awake()
    {
        // デバッグ用(毎回同じ配列でデバッグしたいならコメントを外す)
        // Random.InitState(12345);


        if(fieldData == null)
        {
            Debug.LogError("FieldDataが設定されていません");
        }

        // キャッシュにためる
        scaleCache = fieldData.baseScale;

        //プレイヤーがいる位置をマスに変換する
        int playerStartPosIntX = Mathf.RoundToInt(playerTransform.position.x / scaleCache.x);
        int playerStartPosIntZ = Mathf.RoundToInt(playerTransform.position.z / scaleCache.z);
        playerStartPosCache = new Vector2Int(playerStartPosIntX, playerStartPosIntZ);

        // 初期化
        SetUp();
    }


    void Update()
    {
        if (cellHigh == null) return;

        DrawAlgorithm();

        // 指定したフレーム間で全体を更新する
        if (Time.frameCount % refreshFrame == 0)
        {
            FullRefresh();
        }        
    }


    /// <summary>
    /// // 初期生成フロー(ここでランダム生成を行いフィールドを構築する)
    /// </summary>
    void SetUp()
    {
        GameObject fieldHighParent = new GameObject("FieldHighParent"); // ハイポリを格納する親
        GameObject fieldLowParent = new GameObject("FieldLowParent"); // ローポリを格納する親
        // 各親を生成用オブジェクトの子に設定する
        fieldHighParent.transform.parent = transform;
        fieldLowParent.transform.parent = transform;

        // 配列を初期化する
        cellHigh = new GameObject[fieldData.width, fieldData.depth];
        cellLow = new GameObject[fieldData.width, fieldData.depth];
        positionCache = new Vector3[fieldData.width, fieldData.depth]; // Transfromへのアクセスを防ぐ
        // 埋めつくすときに判定するbool型のデータをフィールド分用意する
        bool[,] isOccupied = new bool[fieldData.width, fieldData.depth];
        // コスト用も用意する
        costMap = new int[fieldData.width, fieldData.depth];

        // 最初に固定配置を置く
        if(fieldData.fixedRules != null)
        {
            foreach (var rule in fieldData.fixedRules)
            {
                // IDから必要なパターン情報を引っ張ってくる
                var pattern = fieldData.GetMapPatternByID(rule.patternID);

                if(pattern == null)
                {
                    Debug.LogWarning($"固定配置エラー : ID{rule.patternID}が見つかりません");
                }


                // 範囲外チェックを行う
                if (rule.position.x < 0 || rule.position.x + pattern.size > fieldData.width ||
                   rule.position.y < 0 || rule.position.y + pattern.size > fieldData.depth)
                {
                    Debug.LogWarning($"固定配置エラー : {rule.patternID}が外側に置こうとしています");
                }

                // 置こうとしている範囲がすでに置いた場所に重なっていないか若しくは壁に重なっていないかを判定する
                if(CanPlace(rule.position, pattern.size, isOccupied))
                {
                    // 分解して設置する
                    PlacePattern(rule.position, pattern, isOccupied, fieldHighParent.transform, fieldLowParent.transform); 
                }
                else
                {
                    // エラーを出してどこでミスったかを明確にする
                    Debug.LogError($"固定配置エラー : {rule.patternID}がすでに置かれている場所か壁のある場所に置こうとしています。\n" +
                        $"サイズ : {pattern.size}\n" +
                        $"場所 : {rule.position}");
                }
            }
        }


        // 壁の配置
        for (int z = 0; z < fieldData.depth; z++)
        {
            for (int x = 0; x < fieldData.width; x++)
            {
                // 四方の辺しか判定しない
                if (x == 0 || x == fieldData.width -1 || z == 0 || z == fieldData.depth - 1)
                {
                    var (pHigh, pLow) = fieldData.GetRandomEdgePrefab(); // 端に来たときにランダムに端のプレファブを取得する
                    SpawnObject(new Vector2Int(x, z), pHigh, pLow, fieldHighParent.transform, fieldLowParent.transform);
                    isOccupied[x, z] = true;
                    costMap[x, z] = edgeCost; // 壁コスト設定
                }
            }
        }

        // 地面をランダムに配置していく(重要なものから先にルールを適用していく)
        if(fieldData.spawnRules != null)
        {
            foreach (var rule in fieldData.spawnRules)
            {
                // そのルールを取得していく
                var pattern = fieldData.GetMapPatternByID(rule.patternId);
                if(pattern == null) continue;

                int size = pattern.size;
                // 壁の内側を抽選範囲とする
                // Random.Rangeのmaxは排他のため端 - sizeでちょうど壁の手前になる
                int minX = 1;
                int maxX = fieldData.width - size;
                int minZ = 1;
                int maxZ = fieldData.depth - size;

                if (minX >= maxX || minZ >= maxZ) continue;

                // このルールですでに配置した座標を記録する
                List<Vector2Int> placedPositions = new List<Vector2Int>();

                // count分だけ配置を試みる
                for (int i = 0; i < rule.count; i++)
                {
                    // 1つの配置につき最大試行回数分だけ試す
                    for (int attempt = 0; attempt < rule.maxAttempts; attempt++)
                    {
                        int rX = Random.Range(minX, maxX);
                        int rZ = Random.Range(minZ, maxZ);
                        Vector2Int candidatePos = new Vector2Int(rX, rZ); // 行こうとしている位置

                        // 同じもの同士の距離チェックを行う
                        if(rule.minGenerateDistance > 0 && IsTooClose(candidatePos, placedPositions, rule.minGenerateDistance))
                        {
                            continue; // 近いやつがいるのでやり直し
                        }

                        // 特定のオブジェクトとの距離チェックを行う
                        if(rule.minIsolationDistance > 0 && IsTooClose(candidatePos, playerStartPosCache, rule.minIsolationDistance))
                        {
                            continue; // オブジェクトと近すぎるのでやり直し
                        }

                        // ランダムに出た値が置けるかどうかをチェックする
                        if (CanPlace(candidatePos, size, isOccupied))
                        {
                            // 置けるならLODに使えるように一つ一つに分解して配置する
                            PlacePattern(new Vector2Int(rX, rZ), pattern, isOccupied, fieldHighParent.transform, fieldLowParent.transform);
                           
                            placedPositions.Add(candidatePos); // 配置したらリストにも登録する
                            
                            break; // 成功したら次の個体に行く
                        }
                    }
                }
            }
        }

        // 隙間を埋める
        for(int z = 0; z < fieldData.depth; z++)
        {
            for (int x = 0; x < fieldData.width; x++)
            {
                // 何も置かれていないなら地面を置く
                if (!isOccupied[x, z])
                {
                    var (pHigh, pLow) = fieldData.GetRandomFillerPrafab();
                    SpawnObject(new Vector2Int(x, z), pHigh, pLow, fieldHighParent.transform, fieldLowParent.transform);

                    costMap[x, z] = fillerCost;
                
                }
            }
        }
    }

 


    //  ================================================ ヘルパー関数 ==============================
    
    /// <summary>
    /// 選択した範囲がすべて空いているかをチェックする
    /// </summary>
    /// <param name="start">選択したい範囲の最初の位置座標</param>
    /// <param name="size">選択したい範囲のサイズ</param>
    /// <param name="occupiedMap">boolで管理しているマップ</param>
    /// <returns>すべて空いていたらtrueそうでなければfalse</returns>
    bool CanPlace(Vector2Int start, int size, bool[,] occupiedMap)
    {
        for(int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                // 調べ用としている座標がtrueだったら失敗とみなしfalseを返す
                if (occupiedMap[start.x + x, start.y + z]) return false;
            }
        }
        return true;
    }



    /// <summary>
    /// SOで作ったパターンを一つ一つに分解して
    /// </summary>
    /// <param name="start">設定されたパターンの最初の位置</param>
    /// <param name="pattern">SOで作ったパターンそのもの</param>
    /// <param name="occupiedMap">bool型マップ</param>
    /// <param name="pHigh">ハイモデルの親オブジェクトになるもの</param>
    /// <param name="pLow">ローモデルの親オブジェクトになるもの</param>
    void PlacePattern(Vector2Int start, SO_FieldData.MapPattern pattern, bool[,] occupiedMap, Transform pHigh, Transform pLow)
    {
        int size  = pattern.size;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                int currentX = start.x + x;
                int currentZ = start.y + z;

                // 左下から順にインデックスを計算する
                int pIndex = z * size + x;

                if (pIndex < pattern.partsHigh.Length)
                {
                    // 生成を行いつつbool座標の場所をtrueにする
                    SpawnObject(new Vector2Int(currentX, currentZ), pattern.partsHigh[pIndex], pattern.partsLow[pIndex], pHigh, pLow);
                    occupiedMap[currentX, currentZ] = true;
                    costMap[currentX, currentZ] = pattern.cost; // コストの設定
                }
            }
        }
    }

   

    /// <summary>
    /// オブジェクトの生成を行う(キャッシュを使っているので若干高速)
    /// </summary>
    /// <param name="generatePos">生成する位置</param>
    /// <param name="prefabHigh">ハイモデルのプレファブ</param>
    /// <param name="prefabLow">ローモデルのプレファブ</param>
    /// <param name="pHigh">ハイモデルを格納する親オブジェクト</param>
    /// <param name="pLow">ローモデルを格納する親オブジェクト</param>
    void SpawnObject(Vector2Int generatePos, GameObject prefabHigh, GameObject prefabLow, Transform pHigh, Transform pLow)
    {
        // プレファブがない場合は何もしない
        if (prefabHigh == null || prefabLow == null) return;

        // x-z平面で座標を決める
        Vector3 pos = new Vector3(generatePos.x * generationInterval, 0.0f, generatePos.y * generationInterval);

        // 負担を減らすためにキャッシュに保存
        positionCache[generatePos.x, generatePos.y] = pos;

        // Highモデルを生成する
        cellHigh[generatePos.x, generatePos.y] = Instantiate(prefabHigh, pos, Quaternion.identity, pHigh);
        // cellHigh[generatePos.x, generatePos.y].transform.localScale = scaleCache; // スケールの統一
        cellHigh[generatePos.x, generatePos.y].SetActive(false); // 最初は映さない

        // Lowモデルを生成する
        cellLow[generatePos.x, generatePos.y] = Instantiate(prefabLow, pos, Quaternion.identity, pLow);
        // cellLow[generatePos.x, generatePos.y].transform.localScale = scaleCache; // スケールの統一
        cellLow[generatePos.x, generatePos.y].SetActive(false); // 最初は映さない

        // Low側があたり判定を持っていれば無効化
        if (cellLow[generatePos.x, generatePos.y].TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }
    }




    /// <summary>
    /// プレイヤーとの距離を参照し描画する範囲を決定する
    /// </summary>
    void DrawAlgorithm()
    {
        // それぞれの距離を成分ごとに計算
        float heightY = Mathf.Max(1.0f, playerTransform.position.y); // プレイヤーの高さを出す。最低でも1以上の値になるようにする
        float heightFactor = Mathf.Log(heightY); // 対数を使いy軸の値が増えるほどゆるやかに増加するもの変換
        float totalDisp02Distance = disp02Distance * (1.0f + heightFactor * heightFactorADJ); // 調整値をかけて広がり具合を決めるheightFactorが0でも安全なように+1
        float sqDisp01 = disp01Distance * disp01Distance;
        float sqDisp02 = totalDisp02Distance * totalDisp02Distance;

        // プレイヤーがいる位置をグリッドに変換(一つ一つのオブジェクトのサイズは同じなので01だけで作る)
        int centerX = Mathf.RoundToInt(playerTransform.position.x / scaleCache.x);
        int centerZ = Mathf.RoundToInt(playerTransform.position.z / scaleCache.z);

        // 最大表示距離をマスに変換
        // 距離が小数点を含んでいた場合に小数点分のマスも見れるように切り上げる
        // xとzが同じ大きさであることが前提
        int radius = Mathf.CeilToInt(totalDisp02Distance / scaleCache.x);

        int useRadius = Mathf.Max(radius, prevRadius); // 実際に使う半径を判断

        // インデックスの範囲をクランプする
        // 取りこぼしがないよう一つ前のフレームの移動経路を考慮した計算にする
        int minX = Mathf.Max(0, Mathf.Min(centerX, prevCenterX) - radius); // プレイヤーの左側が0を下回らないようにする
        int maxX = Mathf.Min(fieldData.width - 1, Mathf.Max(centerX, prevCenterX) + radius); // プレイヤーの右側がwidth - 1を越えないようにする
        int minZ = Mathf.Max(0, Mathf.Min(centerZ, prevCenterZ) - radius); // プレイヤーの手前側が0を下回らないようにする
        int maxZ = Mathf.Min(fieldData.depth - 1, Mathf.Max(centerZ, prevCenterZ) + radius);  // プレイヤーの奥側がdepth - 1を越えないようにする

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {

                if (cellHigh[x, z] == null) continue; // 生成されていない場所はスキップ

                // Transfromではなくキャッシュ座標を使う
                Vector3 targetPos = positionCache[x, z];

                float distance_x = playerTransform.position.x - targetPos.x;
                float distance_y = playerTransform.position.y - targetPos.y;
                float distance_z = playerTransform.position.z - targetPos.z;
                float distance = distance_x * distance_x + distance_y * distance_y + distance_z * distance_z; // 距離計算

                // 一定の距離以内にいるならtrue
                bool should01Active = distance < sqDisp01;
                if (cellHigh[x, z].activeSelf != should01Active)
                {
                    cellHigh[x, z].SetActive(should01Active);
                }

                // ハイポリの範囲外かつ低ポリ範囲内ならtrue
                bool should02Active = distance > sqDisp01 && distance < sqDisp02;
                if (cellLow[x, z].activeSelf != should02Active)
                {
                    cellLow[x, z].SetActive(should02Active);
                }
            }
        }

        // 前フレームの値を記録する
        prevCenterX = centerX;
        prevCenterZ = centerZ;
        prevRadius = radius;
    }

    /// <summary>
    /// 万が一取りこぼしがあった時のために一定時間で全体を検索しオブジェクトの状態を切り替える関数
    /// </summary>
    void FullRefresh()
    {
        // それぞれの距離を成分ごとに計算
        float heightY = Mathf.Max(1.0f, playerTransform.position.y); // プレイヤーの高さを出す。最低でも1以上の値になるようにする
        float heightFactor = Mathf.Log(heightY); // 対数を使いy軸の値が増えるほどゆるやかに増加するもの変換
        float totalDisp02Distance = disp02Distance * (1.0f + heightFactor * heightFactorADJ); // 調整値をかけて広がり具合を決めるheightFactorが0でも安全なように+1
        float sqDisp01 = disp01Distance * disp01Distance;
        float sqDisp02 = totalDisp02Distance * totalDisp02Distance;

        for (int z = 0; z < fieldData.depth; z++)
        {
            for (int x = 0; x < fieldData.width; x++)
            {

                if (cellHigh[x, z] == null) continue;

                Vector3 targetPos = positionCache[x, z];

                float distance_x = playerTransform.position.x - targetPos.x;
                float distance_y = playerTransform.position.y - targetPos.y;
                float distance_z = playerTransform.position.z - targetPos.z;
                float distance = distance_x * distance_x + distance_y * distance_y + distance_z * distance_z; // 距離計算

                // 一定の距離以内にいるならtrue
                bool should01Active = distance < sqDisp01;
                if (cellHigh[x, z].activeSelf != should01Active)
                {
                    cellHigh[x, z].SetActive(should01Active);
                }

                // ハイポリの範囲外かつ低ポリ範囲内ならtrue
                bool should02Active = distance > sqDisp01 && distance < sqDisp02;
                if (cellLow[x, z].activeSelf != should02Active)
                {
                    cellLow[x, z].SetActive(should02Active);
                }
            }
        }
    }

    /// <summary>
    /// 特定のオブジェクトと一定距離離れているかどうかをチェックする
    /// </summary>
    /// <param name="candidate">設置しようとしている位置</param>
    /// <param name="isolationPos">特定のオブジェクトの位置</param>
    /// <param name="minDistance">最低でも離したい距離</param>
    /// <returns></returns>
    bool IsTooClose(Vector2Int candidate, Vector2 isolationPos, float minDistance)
    {
        if(Vector2.Distance(candidate, isolationPos) < minDistance)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 候補地がすでに置かれたリストのいずれかと近すぎないかチェックする
    /// </summary>
    /// <param name="candidate">接地しようとしている位置</param>
    /// <param name="placedList">すでに置かれた座標が格納されたリスト</param>
    /// <param name="minDistance">最小距離</param>
    /// <returns>近いやつがいたらtrue全員と離れているのならfalse</returns>
    bool IsTooClose(Vector2Int candidate, List<Vector2Int> placedList, float minDistance)
    {
        foreach(var pos in placedList)
        {
            // Vector2Distanceで距離を測る
            if(Vector2.Distance(candidate, pos) < minDistance)
            {
                return true; // 近いやつがいたのでtrueを返す
            }
        }
        return false; // 全員と離れていたのでfalseを返す
    }

    // マップの大きさを取得するGetter
    public int GetWidthCount()
    {
        return fieldData.width;
    }

    public int GetDepthCount()
    {
        return fieldData.depth;
    }

    /// <summary>
    /// 指定されたグリッド座標からコストを取得する
    /// </summary>
    /// <param name="position">グリッド座標</param>
    /// <returns>コスト(範囲外等なんらかの理由で範囲外の場合は-1を返す)</returns>
    public int GetCost(Vector2Int position)
    {
        if(position.x >= 1 && position.x < fieldData.width - 1 && position.y >= 1 && position.y < fieldData.depth - 1)
        {
            return costMap[position.x, position.y];
        }
        return -1; // 範囲外などの場合は-1を返す
    }
}
