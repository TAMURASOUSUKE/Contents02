using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Node", menuName = "Scriptable Objects/SO_Node")]
public class SO_Node : ScriptableObject
{
    public int id;
    public Vector3 pos;
    public List<SO_Node> nextNodes;
}
