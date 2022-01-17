using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour, IProjectileFactory, IInit
{
    [Space(5)]
    [SerializeField] private GameObject _reference;
    
    private readonly List<IProjectile> _projectiles = new List<IProjectile>();
    private IEntryPoint _entryPoint;
    

    public void Init(IEntryPoint entryPoint)
    {
        _entryPoint = entryPoint;
    }
    
    public IProjectile GetProjectile()
    {
        var projectiles = _projectiles.Where(p => p.isAvailable).ToList();
        IProjectile projectile;
        if (projectiles.Count == 0)
        {
            var projectileBase = Instantiate(_reference, transform).GetComponent<ProjectileBase>();
            projectileBase.Init(_entryPoint);
            projectile = projectileBase;
            _projectiles.Add(projectile);
        }
        else projectile = projectiles[0];
        return projectile;
    }
}

public interface IProjectileFactory
{
    IProjectile GetProjectile();
}
