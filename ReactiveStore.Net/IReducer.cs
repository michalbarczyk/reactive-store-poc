namespace ReactiveStore.Net;

public interface IReducer<TState, in TAction>
{
    TState Reduce(TState state, TAction action);
}

