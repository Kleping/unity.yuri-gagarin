using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour, IInit
{
    [Space(5)]
    [SerializeField] private GameObject _objIntroduction;
    [SerializeField] private GameObject _objFinishedRound;
    [SerializeField] private GameObject _objTimer;

    [Space(5)]
    [SerializeField] private Sprite _spriteVictory;
    [SerializeField] private Sprite _spriteFailed;
    
    [Space(5)]
    [SerializeField] private Image _finishedRoundBackground;
    
    [Space(5)]
    [SerializeField] private Sprite _spriteLife;
    [SerializeField] private Sprite _spriteLifeDead;
    
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _textScore;
    [SerializeField] private TextMeshProUGUI _textFinalScore;
    [SerializeField] private TextMeshProUGUI _textTimer;
    
    [Space(5)]
    [SerializeField] private List<Image> _healthIcons;

    private IGameCycle _gameCycle;
    
    public void Init(IEntryPoint entryPoint)
    {
        _objIntroduction.gameObject.SetActive(true);
        _objTimer.gameObject.SetActive(false);
        
        entryPoint.profile.OnHealthChanged += onHealthChanged;
        entryPoint.profile.OnScoreChanged += onScoreChanged;
        entryPoint.OnUpdate += onUpdate;
        _gameCycle = entryPoint.gameCycle;
        _gameCycle.OnFinishedRound += onFinishedRound;
    }


    private void onUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_gameCycle.Paused && !_gameCycle.Started && !_objTimer.activeSelf)
            {
                _objFinishedRound.gameObject.SetActive(false);
                _objIntroduction.gameObject.SetActive(false);
                _objTimer.gameObject.SetActive(true);
                StartCoroutine(introduction());
            }
        }
    }

    private void onFinishedRound(bool playerDead)
    {
        _finishedRoundBackground.sprite = playerDead ? _spriteFailed : _spriteVictory;
        _objFinishedRound.gameObject.SetActive(true);
        _textFinalScore.text = _textScore.text;
    }

    private IEnumerator introduction()
    {
        _textTimer.text = "3";
        yield return new WaitForSecondsRealtime(1);
        _textTimer.text = "2";
        yield return new WaitForSecondsRealtime(1);
        _textTimer.text = "1";
        yield return new WaitForSecondsRealtime(1);
        _textTimer.text = "GO!";
        yield return new WaitForSecondsRealtime(.5F);
        _objTimer.gameObject.SetActive(false);
        _gameCycle.StartGame();
    }
    

    private void onHealthChanged(int value)
    {
        for (var i = 0; i < _healthIcons.Count; i++)
            _healthIcons[i].sprite = i < value ? _spriteLife : _spriteLifeDead;
    }
    
    private void onScoreChanged(int value)
    {
        _textScore.text = value.ToString();
    }
}
