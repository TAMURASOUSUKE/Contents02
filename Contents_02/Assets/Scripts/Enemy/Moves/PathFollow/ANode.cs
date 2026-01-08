public class ANode
{
    public Node node;          //どのノードか
    public float cost;  //移動コスト
    public float dist;  //ゴールとの距離
    public float score; //スコア
    public ANode parent;//計算時親ノード
    //-------------コンストラクタ------------------------
    public ANode(Node _node, float _cost, float _dist, float _score, ANode _parent)
    {
        this.node = _node;
        this.cost = _cost;
        this.dist = _dist;
        this.score = _score;
        this.parent = _parent;
    }
    public ANode(Node _node, float _cost, float _dist, float _score)
    {
        this.node = _node;
        this.cost = _cost;
        this.dist = _dist;
        this.score = _score;
        this.parent = null;
    }
    public ANode(Node _node, float _cost, float _dist)
    {
        this.node = _node;
        this.cost = _cost;
        this.dist = _dist;
        this.score = _cost + _dist; //コストと、距離の合計なので自動で計算
        this.parent = null;
    }

    public ANode()
    {
        cost = 0;
        dist = 0;
        score = 0;
        parent = null;
    }
}
