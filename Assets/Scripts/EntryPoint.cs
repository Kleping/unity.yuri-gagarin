using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EntryPoint : MonoBehaviour, IEntryPoint
{
    [Space(5)]
    [SerializeField] private ProjectileFactory _projectileFactory;
    [SerializeField] private Dimensions _dimensions;
    [SerializeField] private Profile _profile;
    [SerializeField] private GameCycle _gameCycle;

    public void Start()
    {
        foreach (var init in FindObjectsOfType<MonoBehaviour>().OfType<IInit>())
        {
            init.Init(this);
        }
    }

    public void Update()
    {
        OnUpdate();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    
    
    public IEnumerator Moving(Transform _transform, Vector3 a, Vector3 b, float speed, UnityAction callback)
    {
        var t = 0F;
        while (Math.Abs(t - 1F) > float.Epsilon)
        {
            if (_gameCycle.Paused)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            t = Mathf.Clamp01(t + Time.deltaTime * speed);
            _transform.position = Vector3.Lerp(a, b, t);
            yield return new WaitForEndOfFrame();
        }
        callback();
    }
    
    public IProjectileFactory projectileFactory => _projectileFactory;
    public IDimensions dimensions => _dimensions;
    public IGameCycle gameCycle => _gameCycle;
    public IProfile profile => _profile;
    
    public event UnityAction OnUpdate = delegate {};
}

public interface IEntryPoint : IAuxiliary
{
    IProjectileFactory projectileFactory { get; }
    IDimensions               dimensions { get; }
    IProfile                     profile { get; }
    IGameCycle                 gameCycle { get; }

    event UnityAction OnUpdate;
}

public interface IAuxiliary
{
    IEnumerator Moving(Transform _transform, Vector3 a, Vector3 b, float speed, UnityAction callback);   
}

public interface IInit
{
    void Init(IEntryPoint entryPoint);
}
