using UnityEngine;

public abstract class SteeringBase
{
    protected float weight = 1.0f;
    protected int priority;

    public void SetWeight(float _weight)
    {
        this.weight = _weight;
    }

    public int GetPriority()
    {
        return priority;
    }
}
