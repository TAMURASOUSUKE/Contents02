public abstract class EnemyStateBase<T> where T : EnemyBlackBoardBase
{
    public abstract EnemyStateBase<T> StateUpdate(T _bb);
}
