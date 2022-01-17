using UnityEngine;
using UnityEngine.Events;

public class Projectile : ProjectileBase
{
    private Coroutine _coroutineRun;
    private UnityAction<IProjectile, GameObject> _onReact;
    private IAuxiliary _auxiliary;

    public override void Init(IEntryPoint entryPoint)
    {
        _auxiliary = entryPoint;
        isAvailable = true;
        entryPoint.gameCycle.OnFinishedRound += onFinishedRound;
    }

    private void onFinishedRound(bool playerDead)
    {
        Stop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAvailable) return;
        _onReact?.Invoke(this, other.gameObject);
    }

    public override void Run(Vector2 a, Vector2 b, float speed, UnityAction<IProjectile, GameObject> onReact)
    {
        if (!isAvailable) return;
        isAvailable = false;
        gameObject.SetActive(true);
        _onReact = onReact;
        _coroutineRun = StartCoroutine(_auxiliary.Moving(transform, a, b, speed, () =>
        {
            gameObject.SetActive(false);
            isAvailable = true;
        }));
    }

    public override void Stop()
    {
        if (isAvailable) return;
        StopCoroutine(_coroutineRun);
        gameObject.SetActive(false);
        isAvailable = true;
    }
}

public abstract class ProjectileBase : MonoBehaviour, IProjectile, IInit
{
    public virtual void Init(IEntryPoint entryPoint) { }
    public virtual void Run(Vector2 a, Vector2 b, float speed, UnityAction<IProjectile, GameObject> onReact) {}
    public virtual void Stop() {}
    
    public bool isAvailable { get; protected set; }
}

public interface IProjectile
{
    void Run(Vector2 a, Vector2 b, float speed, UnityAction<IProjectile, GameObject> onReact);
    void Stop();
    bool isAvailable { get; }
}
