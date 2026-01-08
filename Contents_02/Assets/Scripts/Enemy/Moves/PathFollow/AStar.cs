using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AStar
{
    public Node[] Calc(Node _start, Node _goal)
    {
        //結果代入用
        List<Node> result = new List<Node>();

        //スタート位置の評価計算
        ANode currentANode =
            new ANode
            (
                _start,
                0,
                Vector3.Distance(_goal.pos, _start.pos),
                Vector3.Distance(_goal.pos, _start.pos)  //移動コストが0なので、distと同じ値
            );

        //仮計算結果
        Dictionary<int, ANode> calcResults = new Dictionary<int, ANode>();
        //計算済みノード(親ノードになったやつら)
        Dictionary<int, ANode> closeNodes = new Dictionary<int, ANode>();
        //周囲計算
        while(currentANode.node.id != _goal.id)
        {
            //隣接ノードの計算
            foreach(Node next in currentANode.node.nextNodes)
            {
                //エラー防止
                if (next.id == -1)
                {
                    continue;
                }

                //次のノードの計算
                ANode nextANode =
                    new ANode
                    (
                        next,
                        currentANode.cost + Vector3.Distance(currentANode.node.pos, next.pos),
                        Vector3.Distance(_goal.pos, next.pos)
                    );

                //計算済みリストにあるか
                if(closeNodes.ContainsKey(next.id))
                {
                    continue;
                }

                //計算結果代入用リストにあるか
                //あるなら
                if (calcResults.TryGetValue(next.id, out ANode calcNode))
                {
                    //スコアが既存のモノより軽いなら
                    if (calcNode.score > nextANode.score)
                    {
                        //入れ替え
                        calcNode = nextANode;
                    }
                }
                else
                {
                    calcResults.Add(next.id, nextANode);
                }
            }

            //計算が終わったので、クローズリストに追加
            closeNodes.Add(currentANode.node.id, currentANode);
            //次の親ノードの決定
            //最小スコアの探索
            ANode min = calcResults[0];
            foreach(ANode node in calcResults.Values)
            {
                if(min.score > node.score)
                {
                    min = node;
                }
            }

            //代入
            currentANode = min;
        }

        //追加されるノード
        ANode addANode = calcResults[_goal.id];
        //スタート地点は親がnullなのでそこまで。
        while(addANode.parent != null)
        {
            //ノードの追加
            result.Add(addANode.node);
            //親から次の追加ノードの計算
            addANode = addANode.parent;
        }

        //ゴールから先に追加されているので、反転
        result.Reverse();

        return result.ToArray();
    }
}
