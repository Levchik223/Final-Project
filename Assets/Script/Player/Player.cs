using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{ 
  public static Player Instance { get; private set; }
  public event EventHandler OnPlayerDeath;
  public event EventHandler OnFlashBlink;
  [SerializeField] float movingSpeed = 10f;
  [SerializeField] private int maxHealth = 10;
  [SerializeField] private float damageRecoveryTime = 0.5f;
  [SerializeField] private PlayerVisual playerVisual;
  [SerializeField] private int dashSpeed = 4;
  [SerializeField] private float dashTime = 0.2f;
  [SerializeField] private TrailRenderer trailRenderer;
  [SerializeField] private float dashCoolDownTime = 0.25f;
  
  private Rigidbody2D _rb;
  private KnockBack _knockBack;
  
  Vector2 _inputVector;

  private readonly float _minMovingSpeed = 0.1f;
  private bool _isRunning;
  
  private int _currentHealth;
  private bool _canTakeDamage;
  
  public bool isAlive;
  public int CurrentHealth => _currentHealth;
  public int MaxHealth => maxHealth;
  
  private bool _isDashing;
  private float _initialMovingSpeed;
  
  
  private Camera _mainCamera;

  private void Awake()
  {
    Instance = this;
    _rb = GetComponent<Rigidbody2D>();
    _knockBack = GetComponent<KnockBack>();
    _mainCamera = Camera.main;
    _initialMovingSpeed = movingSpeed;
  }

  private void Start()
  {
    _canTakeDamage = true;
    _currentHealth = maxHealth;
    isAlive = true;

    GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
  }

  private void Update()
  {
    _inputVector = GameInput.Instance.GetMovementVector();
  }

  private void FixedUpdate()
    {
      if (_knockBack.IsGettingKnockedBack)
        return;
      HandleMovement();
    }

  public void TakeDamage(Transform damageSource, int damage)
  {
    if (_canTakeDamage && isAlive)
    {
      _canTakeDamage = false;
      _currentHealth  = Mathf.Max(0, _currentHealth -= damage);
      Debug.Log(_currentHealth);
      _knockBack.GetKnockedBack(damageSource);
      
      OnFlashBlink?.Invoke(this, EventArgs.Empty);

      StartCoroutine(DamageRecoveryRoutine());
    }
    DetectDeath();
  }

  public void Heal(int amount)
  {
    if (!isAlive)
      return;
    _currentHealth += amount;
    if (_currentHealth > maxHealth)
    {
      _currentHealth = maxHealth;
    }
    Debug.Log("current heaal = " + _currentHealth);
  }

  private void DetectDeath()
  {
    if (_currentHealth == 0 && isAlive)
    {
      isAlive = false;
      _canTakeDamage = false;
      
      StopAllCoroutines();
      _knockBack.StopKnockBackMovement();
      
      OnPlayerDeath?.Invoke(this, EventArgs.Empty);
      GameInput.Instance.DisableMovement();
    }
  }

  private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
  {
    Dash();
  }

  private void Dash()
  {
    if (!_isDashing)
     StartCoroutine(DashRoutine());
  }

  private IEnumerator DashRoutine()
  {
    _isDashing = true;
    movingSpeed *= dashSpeed;
    trailRenderer.emitting = true;
    yield return new WaitForSeconds(dashTime);
    
    trailRenderer.emitting = false;
    movingSpeed = _initialMovingSpeed;
    
    yield return new WaitForSeconds(dashCoolDownTime);
    _isDashing = false;
  }

  private IEnumerator DamageRecoveryRoutine()
  {
    yield return new WaitForSeconds(damageRecoveryTime);
    _canTakeDamage = true;
  }

  private void HandleMovement()
  {
    if (playerVisual.IsAttacking())
    {
      _rb.linearVelocity = Vector2.zero;
      return;
    }
    _inputVector = _inputVector.normalized;
    _rb.MovePosition(_rb.position + _inputVector* (movingSpeed * Time.fixedDeltaTime));
    if (MathF.Abs(_inputVector.x) > _minMovingSpeed || Mathf.Abs(_inputVector.y) > _minMovingSpeed)
    {
      _isRunning = true;
    }
    else
    {
      _isRunning = false;
    }
  }

  public bool IsRunning()
  {
    return _isRunning;
  }

  public Vector3 GetPlayerScreenPosition()
  {
    Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
    return playerScreenPosition;
  }
}
