
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Mothership : StrikerEnemy, IInit
{
    private IAuxiliary _auxiliary;
    private IDimensions _dimensions;
    private bool _directToRight;
    
    public void Init(IEntryPoint entryPoint)
    {
        _dimensions = entryPoint.dimensions;
        _auxiliary = entryPoint;

        entryPoint.gameCycle.OnStartedGame += onStartedGame;
        entryPoint.gameCycle.OnFinishedRound += onFinishedRound;
        SetRenderer(false);
        base.Init(entryPoint, string.Empty, onDefeat);
    }

    private void onDefeat(string id)
    {
        runLoop();
    }

    private void onStartedGame()
    {
        SetRenderer(false);
        runLoop();
    }

    private void runLoop()
    {
        SetActive(true);
        StartCoroutine(move(() =>
        {
            SetActive(false);
            runLoop();
        }));   
    }

    private void onFinishedRound(bool playerDead)
    {
        StopAllCoroutines();
    }

    private IEnumerator move(UnityAction callback)
    {
        // yield return new WaitForSecondsRealtime(4F);
        SetRenderer(true);
        _directToRight = Random.Range(0F, 1F) >= .5F;
        GetComponent<SpriteRenderer>().flipX = _directToRight;
        var a = _dimensions.GetDimension(_directToRight ? DimensionType.LeftTop : DimensionType.RightTop);
        var b = _dimensions.GetDimension(_directToRight ? DimensionType.RightBottom : DimensionType.LeftBottom);
        var y = Random.Range(b.y + 6, a.y - 1);
        a = new Vector3(a.x+ (_directToRight?-1:1), y, a.z);
        b = new Vector3(b.x+(_directToRight?1:-1), y, a.z);
        yield return _auxiliary.Moving(transform, a, b, .1F, callback);
    }
    
    public override int Defeat()
    {
        base.Defeat();
        return 10;
    }
}
