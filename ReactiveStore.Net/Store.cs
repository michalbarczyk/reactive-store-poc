using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveStore.Net;

public class Store<TState, TAction, TReducer> : IDisposable where TReducer : IReducer<TState, TAction>, new()
{
    private readonly BehaviorSubject<TState> _state;
    private readonly Subject<TAction> _action;
    private readonly CompositeDisposable _compositeDisposable;

    public Store(TState initialState)
    {
        _state = new BehaviorSubject<TState>(initialState);
        _action = new Subject<TAction>();
        var reducer = new TReducer();
        _compositeDisposable = new CompositeDisposable(
            _action.Subscribe(action => _state.OnNext(reducer.Reduce(_state.Value, action)))
        );
    }

    public IObservable<TState> State => _state.ObserveOn(SynchronizationContext.Current);
    
    public void Dispatch(TAction action)
    {
        _action.OnNext(action);
    }

    public void Effect<T>(Func<IObservable<TAction>, IObservable<T>> effect, bool dispatch = true)
    {
        if (effect is Func<IObservable<TAction>, IObservable<TAction>> cast)
        {
            _compositeDisposable.Add(dispatch 
                ? cast(_action.ObserveOn(SynchronizationContext.Current)).Subscribe(Dispatch) 
                : cast(_action.ObserveOn(SynchronizationContext.Current)).Subscribe());
        }
        else
        {
            _compositeDisposable.Add(dispatch 
                ? throw new NotSupportedException($"Unable to dispatch action [{typeof(T)}] in store [{GetType()}]") 
                : effect(_action.ObserveOn(SynchronizationContext.Current)).Subscribe());
        }
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}