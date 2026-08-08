using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]

public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;
    
    [SerializeField]private int maxHealth;
    private int _currentHealth;
    
    private CapsuleCollider2D _capsuleCollider2D;
    private PolygonCollider2D _polygonCollider2D;
    private EnemyAi _enemyAi;
    private ArcherAi _archerAi;
    
    private bool _isAttacking;

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _enemyAi = GetComponent<EnemyAi>();
        _archerAi = GetComponent<ArcherAi>();
    }

    private void Start()
    {
        _currentHealth = enemySO.enemyHealth;
        PolygonColliderTurnOff();
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isAttacking)
        {
            return;
        }

        if (!Player.Instance.isAlive)
        {
            return;
        }
        if (collision.transform.TryGetComponent(out Player player))
        {
            player.TakeDamage(transform, enemySO. enemyDamageAmount);
        }
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }
    public void PolygonColliderTurnOff()
    {
        _isAttacking = false;
        _polygonCollider2D.enabled = false;
    }
    public void PolygonColliderTurnOn()
    {
        _isAttacking = true;
        _polygonCollider2D.enabled = true;
    }
    
    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            _capsuleCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;
            if (_enemyAi != null)
            {
                _enemyAi.SetDeathState();
            }

            if (_archerAi != null)
            {
                _archerAi.SetDeathState();
            }
            
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
  
}
