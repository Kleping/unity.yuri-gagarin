using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGroup : MonoBehaviour, IInit
{
    [Space(5)]
    [SerializeField] private float _initialSpeed = .25F;
    [SerializeField] private float _speedCoefficient = 4F;
    
    [Space(5)]
    [SerializeField] private List<EnemyGroupContainer> _enemyContainer;
    
    private Vector2Int _defeatedOffset;
    private List<IEnemy> _flatEnemies;
    private float _accumulatedDeltaSpeed, _calculatedSpeed;
    private bool _isMovingRight = true, _isMovingVertical;
    private Coroutine _coroutineMoving;

    private int _defeatedCount;
    private IDimensions _dimensions;
    private IGameCycle _gameCycle;
    private IAuxiliary _auxiliary;
    
    public void Init(IEntryPoint entryPoint)
    {
        _flatEnemies = _enemyContainer.SelectMany(i => i.vertical.Cast<IEnemy>()).ToList();
        _defeatedOffset = new Vector2Int(0, _enemyContainer.Count-1);
        _calculatedSpeed = _initialSpeed;
        
        _dimensions = entryPoint.dimensions;
        _gameCycle = entryPoint.gameCycle;
        _auxiliary = entryPoint;

        for (var i = 0; i < _enemyContainer.Count; i++)
        {
            for (var i1 = 0; i1 < _enemyContainer[i].vertical.Count; i1++)
            {
                _enemyContainer[i].vertical[i1].Init(entryPoint, packId(i, i1), onDefeated);
            }
        }

        _flatEnemies.ForEach(e => e.SetRenderer(false));
        _gameCycle.OnStartedGame += onStartedGame;
        _gameCycle.OnFinishedRound += onFinishedRound;
    }
    

    private void move()
    {
        var position = _dimensions.GetDimension(DimensionType.Top);
        position += Vector3.up*4 + (Vector3.left * _enemyContainer.Count / 2);
        transform.position = position;
        
        var a = position;
        var b = _dimensions.GetDimension(_isMovingRight ? DimensionType.RightTop : DimensionType.LeftTop);
        b = applyOffset(b + Vector3.up*4 + Vector3.left);
        _coroutineMoving = StartCoroutine(_auxiliary.Moving(transform, a, b, _calculatedSpeed, movingCallback));
    }

    private void nextMove(bool switchable)
    {
        if (switchable) _isMovingVertical = !_isMovingVertical;
        var transformComponent = transform;
        var position = transformComponent.position;
        var a = position;
        
        if (_isMovingVertical)
        {
            var b = position + Vector3.down;
            var speed = _calculatedSpeed * 2;
            _coroutineMoving = StartCoroutine(_auxiliary.Moving(transformComponent, a, b, speed, movingCallback));
        }
        else
        {
            if (switchable) _isMovingRight = !_isMovingRight;
            var dimType = _isMovingRight ? DimensionType.RightMiddle : DimensionType.LeftMiddle;
            var dimX = _dimensions.GetDimension(dimType).x;
            dimX += _isMovingRight ? -1 : 1;
            var b = applyOffset(new Vector3(dimX, position.y));
            _accumulatedDeltaSpeed += Time.deltaTime;
            _calculatedSpeed = calculateSpeed(_initialSpeed, _accumulatedDeltaSpeed, _speedCoefficient);
            _coroutineMoving =
                StartCoroutine(_auxiliary.Moving(transformComponent, a, b, _calculatedSpeed, movingCallback));
        }
    }
    
    
    private void onStartedGame()
    {
        _flatEnemies.ForEach(e =>
        {
            e.SetRenderer(true);
            e.SetActive(true);
        });
        _defeatedCount = 0;
        move();
    }

    private void onFinishedRound(bool playerDead)
    {
        _flatEnemies.ForEach(e => e.SetActive(false));
        _calculatedSpeed = _initialSpeed;
        _accumulatedDeltaSpeed = 0F;
        _isMovingVertical = false;
        _isMovingRight = true;
        StopAllCoroutines();
    }

    private void onDefeated(string id)
    {
        unpackId(id, out var columnIndex, out _);
        var container = _enemyContainer[columnIndex];
        if (container.vertical.All(e => e.defeated))
        {
            int x = _defeatedOffset.x, y = 0;
            var clamped = false;
            for (var i = _defeatedOffset.x; i <= _defeatedOffset.y; i++)
            {
                if (clamped)
                {
                    if (_enemyContainer[i].vertical.Any(e => !e.defeated)) y = i;
                }
                else
                {
                    if (_enemyContainer[i].vertical.All(e => e.defeated)) x++;
                    else
                    {
                        y = x;
                        clamped = true;
                    }
                }
            }

            var defeatedOffset = new Vector2Int(x, y);
            if (_defeatedOffset != defeatedOffset)
            {
                _defeatedOffset = new Vector2Int(x, y);
                StopCoroutine(_coroutineMoving);
                nextMove(false);
            }
        }

        _defeatedCount++;
        if (_defeatedCount == _flatEnemies.Count) StartCoroutine(finishRound());
    }

    private IEnumerator finishRound()
    {
        yield return new WaitForSecondsRealtime(1F);
        _gameCycle.FinishRound(false);
    }

    
    private void movingCallback()
    {
        nextMove(true);
    }
    
    private static float calculateSpeed(float initialSpeed, float accumulatedDelta, float coefficient)
    {
        return initialSpeed + accumulatedDelta * coefficient;
    }
    
    private static string packId(int i, int i1)
    {
        return $"{i} {i1}";
    }

    private static void unpackId(string id, out int i, out int i1)
    {
        try
        {
            var result = id.Split(' ');
            i = int.Parse(result[0]);
            i1 = int.Parse(result[1]);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            i = -1;
            i1 = -1;
        }
    }

    private Vector3 applyOffset(Vector3 b)
    {
        return b - Vector3.right * (_isMovingRight?_defeatedOffset.y:_defeatedOffset.x);
    }
}

[Serializable]
public class EnemyGroupContainer
{
    public List<Enemy> vertical;
}