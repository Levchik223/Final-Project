using System;
using UnityEngine;

public class ArcherVisual : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    
   [SerializeField] private ArcherAi archerAi;
   [SerializeField] private EnemyEntity enemyEntity;
   
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private AudioClip attackSound;
   
   private Animator _animator;
   private SpriteRenderer _spriteRenderer;
   private bool _isDead;

   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _spriteRenderer = GetComponent<SpriteRenderer>();
   }

   private void Start()
   {
       enemyEntity.OnTakeHit += EnemyEntity_OnTakeHit;
       enemyEntity.OnDeath += EnemyEntity_OnDeath;
   }

   private void EnemyEntity_OnTakeHit(object sender, EventArgs e)
   {
       if (_isDead)
           return;
       _animator.SetTrigger("TakeHit");
   }

   private void EnemyEntity_OnDeath(object sender, EventArgs e)
   {
       _isDead = true;
       _animator.SetBool("IsDead", true);
   }

  

   private void Update()
   {
     _animator.SetBool("IsMoving", archerAi.IsMoving);
     if (archerAi.IsRetreating)
     {
         if (Player.Instance.transform.position.x < archerAi.transform.position.x)
         {
             _spriteRenderer.flipX = true;
         }
         else
         {
             _spriteRenderer.flipX = false;
         }
     }
     else if (archerAi.IsMoving)
     {
         if (archerAi.MoveDirectionX < 0)
         {
             _spriteRenderer.flipX = true;
         }
         else if (archerAi.MoveDirectionX > 0)
         {
             _spriteRenderer.flipX = false;
         }
     }

     if (archerAi.AttackRequested)
     {
         _animator.SetTrigger("Attack");
         archerAi.ResetAttackRequest();
     }
   }

   public void ShootArrow()
   {
       Vector2 direction = (Player.Instance.transform.position - archerAi.transform.position).normalized;
       GameObject arrowObject = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
       Arrow arrow = arrowObject.GetComponent<Arrow>();
       arrow.SetDirection(direction);
   }

   public void PlayTakeHit()
   {
       _animator.SetTrigger("TakeHit");
   }

   public void SetDead()
   {
       _animator.SetBool("IsDead", true);
   }

   public void StartTakeHit()
   {
       archerAi.StartTakeHit();
   }

   public void EndTakeHit()
   {
       archerAi.EndTakeHit();
   }

   public void PlayAttackSound()
   {
       audioSource.PlayOneShot(attackSound);
   }

   private void OnDestroy()
   {
       enemyEntity.OnTakeHit -= EnemyEntity_OnTakeHit;
       enemyEntity.OnDeath += EnemyEntity_OnDeath;
   }
}
