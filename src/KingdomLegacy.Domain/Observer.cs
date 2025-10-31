namespace KingdomLegacy.Domain;
public class Unsubscriber<T>(ICollection<IObserver<T>> observers,
    IObserver<T> observer) : IDisposable
{
    public void Dispose() => observers.Remove(observer);
}

public class Observer<T>(Action<T> onNext) : IObserver<T>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }
    public void OnNext(T value) =>
        onNext(value);
}

public class Observable<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    public IDisposable Subscribe(Action<T> onNext) =>
        Subscribe(GetObserver(onNext));

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber<T>(_observers, observer);
    }

    protected void Notify(T item)
    {
        foreach (var observer in _observers.ToArray())
            observer.OnNext(item);
    }

    private Observer<T> GetObserver(Action<T> onNext) =>
        new(onNext);
}