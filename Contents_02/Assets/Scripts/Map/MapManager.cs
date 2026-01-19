using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MapManager : MonoBehaviour
{
    const float SIDE_LEN = 50.0f;
    Dictionary<int,Node> mapNodes = new Dictionary<int,Node>();
    Dictionary<Node, Cell> exitNodes = new Dictionary<Node, Cell>();

    CreateField field;

    public static MapManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        field = GetComponent<CreateField>();
        //SetUp();
    }

    public void SetUp()
    {
        for (int i = 0; i < field.GetWidthCount(); i++)
        {
            for (int j = 0; j < field.GetDepthCount(); j++)
            {
                Vector2Int cellPos = new Vector2Int(i, j);
                // セル取得関数
                Cell cell = field.GetCost(cellPos);
                // 全セル内ノードをワールド座標に変換して保持。
                foreach(Node node in cell.so_nodes.nodes)
                {
                    Node worldNode = new Node(mapNodes.Count, cell.pos + node.pos);
                    mapNodes[mapNodes.Count] = worldNode;

                    // セル内の出入り口ノードを登録
                    if(node.role == NodeRole.EXIT)
                    {
                        exitNodes[worldNode] = cell;
                    }
                }
                // 接続ができそうなノードがあったら接続
                foreach(Node node in exitNodes.Keys)
                {
                    Vector2Int checkCell = NodeExitDirUtil.GetNeighborCell(exitNodes[node].cellPos, node.exitDir);
                    //if()
                }
            }
        }
    }

    public Node GetShortestPathNextNode(Vector3 _current, Vector3 _goal)
    {
        // 位置をセルに変更
        Cell currentCell = GetCell(_current);
        Cell goalCell = GetCell(_goal);

        // セルでの、A*
        Cell[] cells = AStar.Calc(currentCell, goalCell, field);

        // 次移動すべきノードの決定
        // 最初には、現在地が入っているのでインデックスが1
        Node node = GetNextCellGoNode(currentCell, cells[1]);

        // ノードのA*
        Node[] nodes = AStar.Calc(GetNearNode(_current, currentCell), GetNearNode(_goal, goalCell), mapNodes);

        // 最初には、現在地が入っているのでインデックスが1
        return node;
    }


    /// <summary>
    /// 現在のセルから、隣り合う行きたいセルに行くためのノードの取得関数
    /// </summary>
    /// <param name="_currentCell">
    /// 現在のセル
    /// </param>
    /// <param name="_targetCell">
    /// 次行きたいセル
    /// </param>
    /// <returns>
    /// 目標ノード
    /// </returns>
    public Node GetNextCellGoNode(Cell _currentCell,Cell _targetCell)
    {
        foreach(Node node in _currentCell.so_nodes.nodes)
        {
            if(node.role == NodeRole.NORMAL)
            {
                continue;
            }

            Vector2Int neighborPos = NodeExitDirUtil.GetNeighborCell(_currentCell.cellPos, node.exitDir);

            if(neighborPos == _targetCell.cellPos)
            {
                return node;
            }
        }

        return null;
    }

    /// <summary>
    /// 特定のセル内で、一番近いノードを取得
    /// </summary>
    /// <param name="_pos">
    /// ワールド座標
    /// </param>
    /// <param name="_nowCell">
    /// 探すセル
    /// </param>
    /// <returns></returns>
    public Node GetNearNode(Vector3 _pos,Cell _targetCell)
    {
        SO_Nodes so = _targetCell.so_nodes;
        // 一番近いノードが入る
        Node nearNode = so.nodes[0];
        // 一番近いノードの距離
        float nearDist = Vector3.Distance(_pos, nearNode.pos);
        for (int i = 1; i < _targetCell.so_nodes.nodes.Count; i++)
        {
            Node node = so.nodes[i];
            // 距離
            float dist = Vector3.Distance(_pos, node.pos);
            // 一番近いノードの距離の更新
            if (nearDist > dist)
            {
                nearNode = node;
                nearDist = dist;
            }
        }

        return nearNode;
    }

    /// <summary>
    /// 特定のセル内で、一番近いノードを取得
    /// </summary>
    /// <param name="_pos">
    /// ワールド座標
    /// </param>
    /// <returns></returns>
    public Node GetNearNode(Vector3 _pos)
    {
        Cell currentCell = GetCell(_pos);

        SO_Nodes so = currentCell.so_nodes;
        // 一番近いノードが入る
        Node nearNode = so.nodes[0];
        // 一番近いノードの距離
        float nearDist = Vector3.Distance(_pos, nearNode.pos);
        for (int i = 1; i < currentCell.so_nodes.nodes.Count; i++)
        {
            Node node = so.nodes[i];
            // 距離
            float dist = Vector3.Distance(_pos, node.pos);
            // 一番近いノードの距離の更新
            if (nearDist > dist)
            {
                nearNode = node;
                nearDist = dist;
            }
        }

        return nearNode;
    }

    /// <summary>
    /// 現在の位置から、セルを把握する
    /// </summary>
    /// <param name="_pos">
    /// 現在地
    /// </param>
    /// <returns></returns>
    public Cell GetCell(Vector3 _pos)
    {
        int wide = Mathf.FloorToInt((_pos.x / SIDE_LEN) - SIDE_LEN / 2.0f);
        int depth = Mathf.FloorToInt((_pos.y / SIDE_LEN) - SIDE_LEN / 2.0f);

        // セル取得関数
        Cell nowCell = null;

        return nowCell;
    }
}