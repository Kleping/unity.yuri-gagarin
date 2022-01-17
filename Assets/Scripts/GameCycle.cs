using UnityEngine;
using UnityEngine.Events;

public class GameCycle : MonoBehaviour, IInit, IGameCycle
{
    public void Init(IEntryPoint entryPoint)
    {
    }


    public void StartGame()
    {
        Started = true;
        OnStartedGame();
    }

    public void FinishRound(bool playerDead)
    {
        Started = false;
        OnFinishedRound(playerDead);
    }

    public void SetPause(bool paused)
    {
        Paused = paused;
        OnPausedGame(Paused);
    }

    
    public event UnityAction OnStartedGame = delegate { };
    public event UnityAction<bool> OnPausedGame = delegate { };
    public event UnityAction<bool> OnFinishedRound = delegate { };
    
    public bool  Paused { get; private set; }
    public bool Started { get; private set; }
}

public interface IGameCycle
{
    public void StartGame();
    public void FinishRound(bool playerDead);
    public void SetPause(bool paused);

    event UnityAction<bool> OnPausedGame;
    event UnityAction OnStartedGame;
    event UnityAction<bool> OnFinishedRound;
    
    public bool  Paused { get; }
    public bool Started { get; }
}
