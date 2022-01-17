using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IEnemy
{
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    
    private UnityAction<string> _onDefeated;
    private string _id;

    public virtual void Init(IEntryPoint entryPoint, string id, UnityAction<string> onDefeated)
    {
        _onDefeated = onDefeated;
        _id = id;
    }

    public virtual int Defeat()
    {
        StopAllCoroutines();
        SetRenderer(false);
        _onDefeated(_id);
        return 1;
    }

    public virtual void SetActive(bool value) { }

    public void SetRenderer(bool value)
    {
        boxCollider.enabled = spriteRenderer.enabled = value;
    }
    

    private BoxCollider2D boxCollider
    {
        get
        {
            if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
            return _boxCollider;
        }
    }
    
    private SpriteRenderer spriteRenderer
    {
        get
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }
    
    public bool defeated => !gameObject.activeSelf;
}

public interface IEnemy
{
    void SetRenderer(bool value);
    void SetActive(bool value);
    int Defeat();
}