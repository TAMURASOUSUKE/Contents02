using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{
    static public Node[] Calc(Node _start, Node _goal)
    {
        // 結果代入用
        List<Node> result = new List<Node>();

        // スタート位置の評価計算
        ANode currentANode =
            new ANode
            (
                _start,
                0,
                Vector3.Distance(_goal.pos, _start.pos)
            );

        // 仮計算結果
        Dictionary<int, ANode> calcResults = new Dictionary<int, ANode>();
        // 計算済みノード(親ノードになったやつら)
        Dictionary<int, ANode> closeNodes = new Dictionary<int, ANode>();
        // 周囲計算
        while(currentANode.node.id != _goal.id)
        {
            // 隣接ノードの計算
            foreach(Node next in currentANode.node.nextNodes)
            {
                // エラー防止
                if (next.id == -1)
                {
                    continue;
                }

                // 次のノードの計算
                ANode nextANode =
                    new ANode
                    (
                        next,
                        currentANode.cost + Vector3.Distance(currentANode.node.pos, next.pos),
                        Vector3.Distance(_goal.pos, next.pos)
                    );

                // 計算済みリストにあるか
                if(closeNodes.ContainsKey(next.id))
                {
                    continue;
                }

                // 計算結果代入用リストにあるか
                // あるなら
                if (calcResults.TryGetValue(next.id, out ANode calcNode))
                {
                    // スコアが既存のモノより軽いなら
                    if (calcNode.score > nextANode.score)
                    {
                        // 入れ替え
                        calcNode = nextANode;
                    }
                }
                // ないなら
                else
                {
                    calcResults.Add(next.id, nextANode);
                }
            }

            // 計算が終わったので、クローズリストに追加
            closeNodes.Add(currentANode.node.id, currentANode);
            // 次の親ノードの決定
            // 最小スコアの探索
            ANode min = calcResults.First().Value;
            foreach(ANode node in calcResults.Values)
            {
                if(min.score > node.score)
                {
                    min = node;
                }
            }

            // 代入
            currentANode = min;
        }

        // 追加されるノード
        ANode addANode = calcResults[_goal.id];
        // スタート地点は親がnullなのでそこまで。
        while(addANode.parent != null)
        {
            // ノードの追加
            result.Add(addANode.node);
            // 親から次の追加ノードの計算
            addANode = addANode.parent;
        }

        // ゴールから先に追加されているので、反転
        result.Reverse();

        return result.ToArray();
    }

    static public Cell[] Calc(Cell _start, Cell _goal, CreateField _field)
    {
        // 結果代入用
        List<Cell> result = new List<Cell>();

        // スタート位置の評価計算
        ACell currentACell =
            new ACell
            (
                _start,
                0,
                OctileDist(_start, _goal),
                null
            );

        // 仮計算結果
        Dictionary<Vector2Int, ACell> calcResults = new Dictionary<Vector2Int, ACell>();
        // 計算済みセル(親セルになったやつら)
        Dictionary<Vector2Int, ACell> closeCells = new Dictionary<Vector2Int, ACell>();
        // 周囲計算
        while (currentACell.cell.pos.x != _goal.pos.x
            || currentACell.cell.pos.y != _goal.pos.y
            )
        {
            // 隣接セルの計算
            foreach (Vector2Int dir in Cell.DIR_8)
            {
                // 隣接セルの計算
                Vector2Int nextPos = currentACell.cell.pos + dir;
                Cell nextCell = new Cell
                    (
                    nextPos,
                    _field.GetCost(nextPos)
                    );

                // マップ外の場合スキップ
                if (nextCell.pos.x < 0 || nextCell.pos.x > _field.GetWidthCount() || nextCell.pos.y < 0 || nextCell.pos.y > _field.GetDepthCount())
                {
                    continue;
                }

                // マップ端の場合スキップ
                if (nextCell.cost < 0)
                {
                    continue;
                }

                // 次のセルの計算
                ACell nextACell =
                    new ACell
                    (
                        nextCell,
                        currentACell.cost + nextCell.cost,
                        OctileDist(nextCell, _goal),
                        currentACell
                    );

                //計算済みリストにあるか
                if (closeCells.ContainsKey(nextCell.pos))
                {
                    continue;
                }

                //計算結果代入用リストにあるか
                //あるなら
                if (calcResults.TryGetValue(nextCell.pos, out ACell calcCell))
                {
                    //スコアが既存のモノより軽いなら
                    if (calcCell.score > nextACell.score)
                    {
                        //入れ替え
                        calcCell = nextACell;
                    }
                }
                // ないなら
                else
                {
                    calcResults.Add(nextCell.pos, nextACell);
                }
            }

            //計算が終わったので、クローズリストに追加
            closeCells.Add(currentACell.cell.pos, currentACell);
            //次の親セルの決定
            //最小スコアの探索
            ACell min = calcResults.First().Value;
            foreach (ACell node in calcResults.Values)
            {
                if (min.score > node.score)
                {
                    min = node;
                }
            }

            //代入
            currentACell = min;
        }

        //追加されるノード
        ACell addACell = calcResults[_goal.pos];
        //スタート地点は親がnullなのでそこまで。
        while (addACell.parent != null)
        {
            //ノードの追加
            result.Add(addACell.cell);
            //親から次の追加ノードの計算
            addACell = addACell.parent;
        }

        //ゴールから先に追加されているので、反転
        result.Reverse();

        return result.ToArray();
    }

    /// <summary>
    /// セルのオクタイル距離の計算
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    /// 二つのセルのオクタイル距離
    /// </returns>
    static private float OctileDist(Cell a, Cell b)
    {
        int dx = Mathf.Abs(a.pos.x - b.pos.x);
        int dy = Mathf.Abs(a.pos.y - b.pos.y);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        return max + (Mathf.Sqrt(2f) - 1f) * min;
    }
}
