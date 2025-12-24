using UnityEngine;

public abstract class AvoidanceSteering:SteeringBase
{
    public abstract bool TryGetAvoidance(EnemyBlackBoardBase _bb, out AvoidInfo _info);
}
