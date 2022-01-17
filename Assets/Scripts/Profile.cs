using UnityEngine;
using UnityEngine.Events;

public class Profile : MonoBehaviour, IProfile, IInit
{
    private IGameCycle _gameCycle;
    private int _health;

    
    public void Init(IEntryPoint entryPoint)
    {
        _gameCycle = entryPoint.gameCycle;
        _gameCycle.OnStartedGame += onStartGame;
        resetData();
    }

    
    public void SetHealth(int value)
    {
        _health = Mathf.Clamp(_health + value, 0, 3);
        OnHealthChanged?.Invoke(_health);

        if (_health == 0)
        {
            _gameCycle.FinishRound(true);
        }
    }

    private void onStartGame()
    {
        resetData();
    }
    
    public void SetScore(int value)
    {
        Score += value;
        OnScoreChanged?.Invoke(Score);
    }

    private void resetData()
    {
        SetScore(-Score);
        SetHealth(3);
    }
    
    
    public event UnityAction<int> OnHealthChanged;
    public event UnityAction<int> OnScoreChanged;

    public int Score { get; private set; }
}

public interface IProfile
{
    void SetHealth(int value);
    void SetScore(int value);
    
    event UnityAction<int> OnHealthChanged;
    event UnityAction<int> OnScoreChanged;
}
