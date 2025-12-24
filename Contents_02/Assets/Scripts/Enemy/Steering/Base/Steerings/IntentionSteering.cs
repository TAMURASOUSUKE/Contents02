using UnityEngine;

public abstract class IntentionSteering:SteeringBase
{
    public abstract Vector3 SteeringCalc(EnemyBlackBoardBase _bb);
}
