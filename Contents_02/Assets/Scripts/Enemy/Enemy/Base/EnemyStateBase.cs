public abstract class EnemyStateBase<T> where T : EnemyBlackBoardBase
{
    public abstract void StateUpdate(T _bb);
}
