using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class StrikerEnemy : Enemy
{
    private IProjectileFactory _projectileFactory;
    private IDimensions _dimensions;
    private IGameCycle _gameCycle;
    private IProfile _profile;
    
    public override void Init(IEntryPoint entryPoint, string id, UnityAction<string> onDefeated)
    {
        _projectileFactory = entryPoint.projectileFactory;
        _dimensions = entryPoint.dimensions;
        _gameCycle = entryPoint.gameCycle;
        _profile = entryPoint.profile;
        base.Init(entryPoint, id, onDefeated);
    }

    public override void SetActive(bool value)
    {
        if (value) StartCoroutine(hitProjectile());
        else StopAllCoroutines();
        base.SetActive(value);
    }

    public override int Defeat()
    {
        return base.Defeat() + 1;
    }
    
    
    private IEnumerator hitProjectile()
    {
        yield return new WaitForSecondsRealtime(Random.Range(0F, 3F));
        var t = 2F;

        while (gameObject.activeSelf)
        {
            var projectile = _projectileFactory.GetProjectile();
            var a = transform.position;
            var b = new Vector2(a.x, _dimensions.GetDimension(DimensionType.Bottom).y);
            projectile.Run(a, b, .75F, onReact);
            while (t != 0F)
            {
                if (!_gameCycle.Paused) t = Mathf.Clamp(t - Time.deltaTime, 0F, 2F);
                yield return new WaitForEndOfFrame();
            }
            t = 2F;
        }
    }
    
    private void onReact(IProjectile projectile, GameObject reacted)
    {
        if (!reacted.CompareTag("Player")) return;
        projectile.Stop();
        _profile.SetHealth(-1);
    }
}
