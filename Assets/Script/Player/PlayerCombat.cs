using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
   private static readonly int Attack = Animator.StringToHash("Attack");
   private static readonly int SuperAttack = Animator.StringToHash("DashAttack");
   [SerializeField] private Animator animator;
   [SerializeField] private int attackDamage = 2;
   [SerializeField] private int superAttackDamage = 4;
   private int _currentDamage;
   private bool _hasDealtDamage;
   private PolygonCollider2D _polygonCollider2D;

   private void Awake()
   {
      _polygonCollider2D = GetComponent<PolygonCollider2D>();
   }
   private void Start()
   {
      AttackColliderTurnOff();
      GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
      GameInput.Instance.OnPlayerDashAttack += GameInput_OnPlayerDashAttack;
   }
   

   private void GameInput_OnPlayerAttack(object sender, EventArgs e)
   {
      _currentDamage = attackDamage;
      _hasDealtDamage = false;
      animator.SetTrigger(Attack);
   }

   private void GameInput_OnPlayerDashAttack(object sender, EventArgs e)
   {
      _currentDamage = superAttackDamage;
      _hasDealtDamage = false;
      animator.SetTrigger(SuperAttack);
   }

   public void AttackColliderTurnOff()
   {
      _polygonCollider2D.enabled = false;
   }

   public void AttackColliderTurnOn()
   {
      _polygonCollider2D.enabled = true;
   }

   public void TriggerEndAttackAnimation()
   {
      AttackColliderTurnOff();
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (_hasDealtDamage)
         return;
      if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
      {
         enemyEntity.TakeDamage(_currentDamage);
         _hasDealtDamage = true;
      }
   }
   private void OnDestroy()
   {
      GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
      GameInput.Instance.OnPlayerDashAttack -= GameInput_OnPlayerDashAttack;
   }
}
