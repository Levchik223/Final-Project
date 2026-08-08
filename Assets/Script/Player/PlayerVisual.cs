using System;
using UnityEngine;
using Random = System.Random;

public class PlayerVisual : MonoBehaviour
{
   private static readonly int IsDie = Animator.StringToHash("IsDie");
   private static readonly int IsRunning = Animator.StringToHash("IsRunning");
   private Vector3 _originalScale;
   private Animator _animator;
   private FlashBlink _flashBlink;
   
   [SerializeField] private PlayerCombat playerCombat;
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private AudioClip[] attackSounds;

   private void Awake()
   {
      _animator = GetComponent<Animator>();
      GetComponent<SpriteRenderer>();
      _originalScale = transform.localScale;
      _flashBlink = GetComponent<FlashBlink>();
   }

   private void Start()
   {
      Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
   }

   private void Player_OnPlayerDeath(object sender, EventArgs e)
   {
      _animator.SetBool(IsDie, true);
      _flashBlink.StopBlinking();
   }
   private void Update()
   {
      _animator.SetBool(IsRunning, Player.Instance.IsRunning());
      if (!IsAttacking())
      {
         AdjustPlayerFacingDirection();
      }
   }

   public bool IsAttacking()
   {
      AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
      return stateInfo.IsName("Attack") || stateInfo.IsName("Dash-Attack");
   }

   private void AdjustPlayerFacingDirection()
   {
      Vector2 movementVector = GameInput.Instance.GetMovementVector();
      if (movementVector.x < 0)
      {
         transform.localScale = new Vector3(
            -Mathf.Abs(_originalScale.x),
            _originalScale.y,
            _originalScale.z );
      }
      else if (movementVector.x > 0)
      {
         transform.localScale = new Vector3(
            Mathf.Abs(_originalScale.x),
            _originalScale.y,
            _originalScale.z );
      }
   }
   

   public void TriggerEndAttackAnimaion()
   {
      playerCombat.TriggerEndAttackAnimation();
   }

   public void TurnOnAttackCollider()
   {
      playerCombat.AttackColliderTurnOn();
   }

   public void TurnOffAttackCollider()
   {
      playerCombat.AttackColliderTurnOff();
   }

   public void PlayAttackSound()
   {
      if (attackSounds.Length == 0)
         return;
      int randomIndex = UnityEngine.Random.Range(0, attackSounds.Length);
      audioSource.PlayOneShot(attackSounds[randomIndex]);
   }
   
   private void OnDestroy()
   {
      Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
   }
   
}
