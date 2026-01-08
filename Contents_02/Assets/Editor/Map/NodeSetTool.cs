using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NodeSetTool: EditorWindow
{
    /// <summary>
    /// 変更するSO
    /// </summary>
    SO_Nodes so;
    /// <summary>
    /// 選択されたノード
    /// </summary>
    Node select;
    [MenuItem("Tools/MapNodePlanter")]
    static void Open()
    {
        GetWindow<NodeSetTool>("MapNodePlanter");
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI()
    {
        GUILayout.Label("マップ上にノードを配置するツール");

        so = (SO_Nodes)EditorGUILayout.ObjectField("ノードを追加したいSO_Nodes", so, typeof(SO_Nodes), false);
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        PlantNode(e);
        DeleteNode(e);

        DrawNode(e);
        DrawConnection(e);
    }

    /// <summary>
    /// ノード設置関数
    /// </summary>
    /// <param name="e_">
    /// 現在のイベント
    /// </param>
    void PlantNode(Event e_)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }

        // マウスからのレイ
        Ray ray = HandleUtility.GUIPointToWorldRay(e_.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitinfo) == false&& e_.type == EventType.MouseMove)
        {
            return;
        }

        if (Handles.Button(hitinfo.point, Quaternion.identity, 0.3f, 0.3f, Handles.SphereHandleCap))
        {
            // Undo作成
            Undo.RecordObject(so, "add node");
            // ノード作成
            Node node = new Node();
            node.pos = hitinfo.point;
            // ノードIDは0から、追加された順
            node.id = so.nodes.Count;
            node.nextNodes = new List<Node>();
            // リストに追加
            so.nodes.Add(node);
            // 通知
            EditorUtility.SetDirty(so);

            // イベント使用
            e_.Use();
        }
    }

    /// <summary>
    /// ノード削除関数
    /// </summary>
    /// <param name="e_">
    /// 現在のイベント
    /// </param>
    void DeleteNode(Event e_)
    {
        // nullチェック
        if (so == null || select == null)
        {
            return;
        }

        // deleteボタンが押されたら、削除
        if(e_.type == EventType.KeyDown && e_.keyCode == KeyCode.Delete)
        {
            // Undo作成
            Undo.RecordObject(so, "delete node");
            // SOのリストから除外
            so.nodes.Remove(select);
            // IDの修正
            for(int id = 0; id < so.nodes.Count; id++)
            {
                if(so.nodes[id].id != id)
                {
                    so.nodes[id].id = id;
                }
            }
            // すべてのノードの移動できるノードリストから除外
            foreach (Node node in so.nodes)
            {
                node.nextNodes.Remove(select);
            }

            // 選択されたノードをnullに戻す
            select = null;

            // 通知
            EditorUtility.SetDirty(so);

            // イベント使用
            e_.Use();
        }
    }

    /// <summary>
    /// ノード接続関数
    /// </summary>
    /// <param name="node01_">
    /// 一つ目のノード
    /// </param>
    /// /// <param name="node02_">
    /// 二つ目のノード
    /// </param>
    void ConnecteNode(Node node01_, Node node02_)
    {
        Undo.RecordObject(so, "connecte node");
        // nullチェック
        if (so == null)
        {
            return;
        }
        // 移動できるノードリストにないなら変更
        if (node01_.nextNodes.Contains(node02_) == false)
        {
            node01_.nextNodes.Add(node02_);
        }

        if (node02_.nextNodes.Contains(node01_) == false)
        {
            node02_.nextNodes.Add(node01_);
        }

        EditorUtility.SetDirty(so);
    }

    /// <summary>
    /// 現在設置されているノード描画関数
    /// </summary>
    /// <param name="e_">
    /// 現在のイベント
    /// </param>
    void DrawNode(Event e_)
    {
        // nullチェック
        if (so == null)
        {
            return;
        }
        foreach(var node in so.nodes)
        {
            //色が変わった時元に戻すよう
            Color prev = Handles.color;

            // nullチェック
            if (select != null)
            {
                // 選択されたノードなら、色を変える
                if (select.id == node.id)
                {
                    Handles.color = Color.red;
                }
            }

            if (Handles.Button(node.pos, Quaternion.identity, 0.3f, 0.3f, Handles.SphereHandleCap))
            {
                // 選択されたノードが埋まってるかどうか
                if(select == null)
                {
                    // 埋まっていないなら、埋める
                    select = node;
                }
                else
                {
                    // 同じノードなら選択から外す
                    if(select.id == node.id)
                    {
                        select = null;
                    }
                    // そうでないなら接続
                    else
                    {
                        // 埋まっているなら、接続
                        ConnecteNode(select, node);
                        // 接続時の親ノードをnullに戻す
                        select = null;
                    }
                }
            }

            // 色を元に戻す
            Handles.color = prev;
        }
    }

    /// <summary>
    /// ノード間の接続の描画関数
    /// </summary>
    /// <param name="e_">
    /// 現在のイベント
    /// </param>
    void DrawConnection(Event e_)
    {
        // nullチェック
        if(so == null)
        {
            return;
        }

        List<Node> drawnNodes = new List<Node>();
        

        foreach(Node node in so.nodes)
        {
            if(node.nextNodes.Count > 0)
            {
                foreach (Node next in node.nextNodes)
                {
                    if (drawnNodes.Contains(next))
                    {
                        continue;
                    }

                    Handles.DrawLine(node.pos, next.pos);
                }
            }

            drawnNodes.Add(node);
        }
    }
}
