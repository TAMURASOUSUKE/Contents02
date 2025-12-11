using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyData", menuName = "Scriptable Objects/SO_EnemyData")]
public class SO_EnemyData : ScriptableObject
{
    //Å‘å‘¬“x
    public float maxSpeed;
    //Å‘å‰Á‘¬“x
    public float maxAcc = 20.0f;
    
    //Arrive‚Å’x‚­‚È‚é”¼Œa
    public float slowRadius = 10.0f;
}
