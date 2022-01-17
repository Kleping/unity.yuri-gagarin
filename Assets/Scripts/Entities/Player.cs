using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour, IInit
{
    [Space(5)]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private IGameCycle _gameCycle;
    private IProjectileFactory _projectileFactory;
    private IDimensions _dimensions;
    private IProfile _profile;

    private Vector3 _initPosition;
    
    private const float MOVEMENT_SPEED = 4F;

    public void Init(IEntryPoint entryPoint)
    {
        _projectileFactory = entryPoint.projectileFactory;
        _dimensions = entryPoint.dimensions;
        _gameCycle = entryPoint.gameCycle;
        _profile = entryPoint.profile;

        _gameCycle.OnStartedGame += onStartedGame;
        _initPosition = transform.position;
    }
    
    public void Update()
    {
        if (_gameCycle.Paused || !_gameCycle.Started) return;
        processInput();
        clampTranslation();
    }

    private static Vector3 calculateMovementSpeed(Vector3 direction)
    {
        return direction * Time.deltaTime * MOVEMENT_SPEED;
    }

    private void processInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(calculateMovementSpeed(Vector3.left));
            _spriteRenderer.flipX = true;
        }
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(calculateMovementSpeed(Vector3.right));
            _spriteRenderer.flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var a = transform.position;
            var dimTop = _dimensions.GetDimension(DimensionType.Top);
            var projectile = _projectileFactory.GetProjectile();
            projectile.Run(a, new Vector2(a.x, dimTop.y), .75F, onReact);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        _profile.SetScore(-3);
    }

    private void onStartedGame()
    {
        transform.position = _initPosition;
    }
    
    private void onReact(IProjectile projectile, GameObject reacted)
    {
        if (!reacted.CompareTag("Enemy")) return;
        projectile.Stop();
        _profile.SetScore(reacted.GetComponent<Enemy>().Defeat());
    }
    
    private void clampTranslation()
    {
        var cam = Camera.main;
        if (cam == null) return;
        var screenPos = new Vector3(Screen.width, Screen.height, cam.transform.position.z);
        var screenBounds = cam.ScreenToWorldPoint(screenPos);
        var pos = transform.position;
        var x = Mathf.Clamp(pos.x, -screenBounds.x + 1, screenBounds.x - 1);
        transform.position = new Vector3(x, pos.y, pos.z);
    }
}
