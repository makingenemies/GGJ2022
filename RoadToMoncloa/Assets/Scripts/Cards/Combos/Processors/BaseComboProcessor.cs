public abstract class BaseComboProcessor<TComboSO> : IComboProcessor where TComboSO : CardComboSO
{
    public void ProcessComboAfterPlayImpl(CardComboSO combo, PlayStageCard playedCard, CardPlayType playType)
    {
        ProcessComboAfterPlayImpl(combo as TComboSO, playedCard, playType);
    }

    protected abstract void ProcessComboAfterPlayImpl(TComboSO combo, PlayStageCard playedCard, CardPlayType playType);
}

public interface IComboProcessor
{
    void ProcessComboAfterPlayImpl(CardComboSO combo, PlayStageCard playedCard, CardPlayType playType);
}