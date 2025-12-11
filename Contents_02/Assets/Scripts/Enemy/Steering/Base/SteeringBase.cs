using UnityEngine;

public abstract class SteeringBase
{
    public abstract Vector3 SteeringCalc(EnemyBlackBoardBase _bb, float weight);
}
