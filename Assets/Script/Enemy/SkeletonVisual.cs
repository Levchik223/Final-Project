using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SkeletonVisual : MonoBehaviour
{
  private static readonly int IsDying = Animator.StringToHash("IsDying");
  private static readonly int TakeHit = Animator.StringToHash("TakeHit");
  private static readonly int IsRunning = Animator.StringToHash("IsRunning");
  private static readonly int ChasingSpeedMultiplier = Animator.StringToHash("ChasingSpeedMultiplier");
  private static readonly int Attacking = Animator.StringToHash("Attacking");
  
  [SerializeField] private EnemyAi enemyAi;
  [SerializeField] private EnemyEntity enemyEntity;
  [SerializeField] private GameObject enemyShadow;
  
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip attackSound;
  
  SpriteRenderer _spriteRenderer;
  private Animator _animator;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void Start()
  {
    enemyAi.OnEnemyAttack += enemyAi_OnEnemyAttack;
    enemyEntity.OnTakeHit += enemyEntity_OnTakeHit;
    enemyEntity.OnDeath += enemyEntity_OnDeath;
  }

  private void enemyEntity_OnDeath(object sender, EventArgs e)
  {
    _animator.SetBool(IsDying, true);
    _spriteRenderer.sortingOrder = -1;
    enemyShadow.SetActive(false);
  }
  private void enemyEntity_OnTakeHit(object sender, System.EventArgs e)
  {
    _animator.SetTrigger(TakeHit);
  }
  
  private void Update()
  {
    _animator.SetBool(IsRunning, enemyAi.IsRunning);
    _animator.SetFloat(ChasingSpeedMultiplier, enemyAi.GetRoamingAnimationSpeed());
  }

  public void TriggerAttackAnimationTurnOff()
  {
    enemyEntity.PolygonColliderTurnOff();
  }
  public void TriggerAttackAnimationTurnOn()
  {
    enemyEntity.PolygonColliderTurnOn();
  }
  private void enemyAi_OnEnemyAttack(object sender, System.EventArgs e)
  {
    _animator.SetTrigger(Attacking);
  }

  public void PlayAttackSound()
  {
    audioSource.PlayOneShot(attackSound);
  }

  private void OnDestroy()
  {
    enemyAi.OnEnemyAttack -= enemyAi_OnEnemyAttack;
    enemyEntity.OnTakeHit += enemyEntity_OnTakeHit;
    enemyEntity.OnDeath += enemyEntity_OnDeath;
  }
}
