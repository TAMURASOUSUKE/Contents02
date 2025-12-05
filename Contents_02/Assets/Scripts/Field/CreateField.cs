using UnityEngine;
using UnityEngine.Assertions.Must;



public class CreateField : MonoBehaviour
{
    [SerializeField] SO_FieldData fieldData;
    [SerializeField] Transform playerTransform; // プレイヤーの位置情報を参照する
    [Header("高解像度用のプレファブを表示する距離です")]
    [SerializeField] float disp01Distance = 300.0f;
    [Header("低解像度用のプレファブを表除する距離です")]
    [SerializeField] float disp02Distance = 4200.0f;
    [Header("プレイヤーが高い位置に行ったときに周囲がどのくらい見えるようになるかを調整するものです")]
    [SerializeField, Range(0.1f, 1.0f)] float heightFactorADJ = 0.1f;
    [Header("指定したフレーム数で全体の状態を更新し描画残しを防ぎます")]
    [Header("単位 : f(フレーム)")]
    [SerializeField] int refreshFrame = 30;
    [Header("生成間隔")]
    [SerializeField] int generationInterval = 50;
    GameObject[,] cell01; // fileDataから取り出したlevel01を格納する変数
    GameObject[,] cell02; // fileDataから取り出したlevel02を格納する変数
    Vector3 scaleCash = Vector3.one; // fieldDataから取り出したスケールをキャッシュする
    int prevCenterX = 0; // 前フレームの中心X
    int prevCenterZ = 0; // 前フレームの中心Z
    int prevRadius = 0; // 前フレームの描画半径
    void Start()
    {
        SetUp();
        scaleCash = fieldData.GetScaleLevel01();
    }


    void Update()
    {
        DrawAlgorithm();

        // 指定したフレーム間で全体を更新する
        if (Time.frameCount % refreshFrame == 0)
        {
            FullRefresh();
        }
    }


    // プレイヤーとの距離を参照し描画する範囲を決定する
    void DrawAlgorithm()
    {
        // それぞれの距離を成分ごとに計算
        float heightY = Mathf.Max(1.0f, playerTransform.position.y); // プレイヤーの高さを出す。最低でも1以上の値になるようにする
        float heightFactor = Mathf.Log(heightY); // 対数を使いy軸の値が増えるほどゆるやかに増加するもの変換
        float totalDisp02Distance = disp02Distance * (1.0f + heightFactor * heightFactorADJ); // 調整値をかけて広がり具合を決めるheightFactorが0でも安全なように+1
        float sqDisp01 = disp01Distance * disp01Distance;
        float sqDisp02 = totalDisp02Distance * totalDisp02Distance;

        // プレイヤーがいる位置をグリッドに変換(一つ一つのオブジェクトのサイズは同じなので01だけで作る)
        int centerX = Mathf.RoundToInt(playerTransform.position.x / scaleCash.x);
        int centerZ = Mathf.RoundToInt(playerTransform.position.z / scaleCash.z);

        // 最大表示距離をマスに変換
        // 距離が小数点を含んでいた場合に小数点分のマスも見れるように切り上げる
        // xとzが同じ大きさであることが前提
        int radius = Mathf.CeilToInt(totalDisp02Distance / scaleCash.x);

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

                float distance_x = playerTransform.position.x - cell01[x, z].transform.position.x;
                float distance_y = playerTransform.position.y - cell01[x, z].transform.position.y;
                float distance_z = playerTransform.position.z - cell01[x, z].transform.position.z;
                float distance = distance_x * distance_x + distance_y * distance_y + distance_z * distance_z; // 距離計算

                // 一定の距離以内にいるならtrue
                bool should01Active = distance < sqDisp01;
                if (cell01[x, z].activeSelf != should01Active)
                {
                    cell01[x, z].SetActive(should01Active);
                }

                // ハイポリの範囲外かつ低ポリ範囲内ならtrue
                bool should02Active = distance > sqDisp01 && distance < sqDisp02;
                if (cell02[x, z].activeSelf != should02Active)
                {
                    cell02[x, z].SetActive(should02Active);
                }
            }
        }

        // 前フレームの値を記録する
        prevCenterX = centerX;
        prevCenterZ = centerZ;
        prevRadius = radius;
    }


    // 万が一取りこぼしがあった時のために一定時間で全体を検索しオブジェクトの状態を切り替える関数
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
                float distance_x = playerTransform.position.x - cell01[x, z].transform.position.x;
                float distance_y = playerTransform.position.y - cell01[x, z].transform.position.y;
                float distance_z = playerTransform.position.z - cell01[x, z].transform.position.z;
                float distance = distance_x * distance_x + distance_y * distance_y + distance_z * distance_z; // 距離計算

                // 一定の距離以内にいるならtrue
                bool should01Active = distance < sqDisp01;
                if (cell01[x, z].activeSelf != should01Active)
                {
                    cell01[x, z].SetActive(should01Active);
                }

                // ハイポリの範囲外かつ低ポリ範囲内ならtrue
                bool should02Active = distance > sqDisp01 && distance < sqDisp02;
                if (cell02[x, z].activeSelf != should02Active)
                {
                    cell02[x, z].SetActive(should02Active);
                }
            }
        }
    }


    // 最初に地形生成を行う
    void SetUp()
    {
        GameObject filed01Parent = new GameObject("Field01Parent"); // field01の親となるオブジェクトの生成
        GameObject filed02Parent = new GameObject("Field02Parent"); // field02の親となるオブジェクトの生成
        filed01Parent.transform.parent = transform; // Field01の親オブジェクトを自身の子オブジェクトにする
        filed02Parent.transform.parent = transform; // Field02の親オブジェクトを自身の子オブジェクトにする

        // 配列の長さをDataに合わせる
        cell01 = new GameObject[fieldData.width, fieldData.depth];
        cell02 = new GameObject[fieldData.width, fieldData.depth];

        for (int z = 0; z < fieldData.depth; z++)
        {
            for (int x = 0; x < fieldData.width; x++)
            {
                var cell01Prefab = fieldData.GetLevel01(x, z); // level01の情報を取得
                var cell02Prefab = fieldData.GetLevel02(x, z); // level02の情報の取得

                if (cell01Prefab == null || cell02Prefab == null)
                {
                    Debug.LogWarning($"FieldDataが不足しています。 (x, z) = ({x}, {z})");
                }

                cell01[x, z] = Instantiate(cell01Prefab, new Vector3(x * generationInterval, 0.0f, z * generationInterval), Quaternion.identity); // level01の生成
                cell02[x, z] = Instantiate(cell02Prefab, new Vector3(x * generationInterval, 0.0f, z * generationInterval), Quaternion.identity); // level02の生成
                cell01[x, z].transform.parent = filed01Parent.transform; // 生成されたlevel01を子オブジェクトに
                cell02[x, z].transform.parent = filed02Parent.transform; // 生成されたlevel02を子オブジェクトに
                cell01[x, z].SetActive(false); // 最初は見えない状態にする
                cell02[x, z].SetActive(false); // 最初は見えない状態にする
                cell02[x, z].GetComponent<Collider>().enabled = false; // 02のところに触れるわけではないのであたり判定は消す
            }
        }
    }
}
