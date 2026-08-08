using System;
using UnityEngine;
using UnityEngine.AI;
using WarriorHunt.Utils;
using Random = UnityEngine.Random;

public class ArcherAi : MonoBehaviour
{
 [SerializeField] private float roamingDistanceMax = 7f;
 [SerializeField] private float roamingDistanceMin = 3f;
 [SerializeField] private float roamingTimerMax = 2f;

 [SerializeField] private float chasingDistance = 7f;
 [SerializeField] private float shootingDistance = 4f;
 [SerializeField] private float attackCooldown = 1.5f;
 [SerializeField] private float retreatDistance = 2f;
 
 [SerializeField] private ArcherVisual archerVisual;

 private float _attackTimer;
 private float _roamingTimer;
 private Vector3 _startingPosition;
 private Vector3 _roamPosition;
 private NavMeshAgent _navMeshAgent;
 private Collider2D[] _colliders;


 private void Awake()
 {
  _navMeshAgent = GetComponent<NavMeshAgent>();
  _navMeshAgent.updateRotation = false;
  _navMeshAgent.updateUpAxis = false;
  _colliders = GetComponents<Collider2D>();

  transform.rotation = Quaternion.identity;
  _startingPosition = transform.position;
  _roamingTimer = roamingTimerMax;

  _attackTimer = 0f;
 }
 
 private void Update()
 {
  if (IsTakingHit)
  {
   return;
  }

  if (!Player.Instance.isAlive)
  {
   _roamingTimer -= Time.deltaTime;
   if (_roamingTimer <= 0f)
   {
    Roaming();
    _roamingTimer = roamingTimerMax;
   }
   return;
  }
  if (_attackTimer > 0f)
  {
   _attackTimer -= Time.deltaTime;
  }
  float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
  if (distanceToPlayer <= chasingDistance)
  {
   ChasePlayer(distanceToPlayer);
  }
  else
  {
   _roamingTimer -= Time.deltaTime;
   if (_roamingTimer <= 0f)
   {
    Roaming();
    _roamingTimer = roamingTimerMax;
   }
  }
 }
 
 
 
 public bool IsAttacking {get; private set;}
 
 public bool AttackRequested {get; private set;}
 
 public bool IsTakingHit {get; private set;}
 
 public bool IsRetreating{get; private set;}

 public void StartTakeHit()
 {
  IsTakingHit = true;
  _navMeshAgent.isStopped = true;
  _navMeshAgent.ResetPath();
 }

 public void EndTakeHit()
 {
  IsTakingHit = false;
  if (_navMeshAgent.isOnNavMesh)
  {
   _navMeshAgent.isStopped = false;
  }
 }

 private void Attack()
 {
  AttackRequested =  true;
 }

 public void ResetAttackRequest()
 {
  AttackRequested =  false;
 }

 private void ChasePlayer(float distanceToPlayer)
 {
  if (distanceToPlayer < retreatDistance)
  {
   RetreatFromPlayer();
  }
  else if (distanceToPlayer > shootingDistance)
  {
   IsRetreating = false;
  // IsAttacking = false;
   _navMeshAgent.isStopped = false;
   _navMeshAgent.SetDestination(Player.Instance.transform.position);
  }
  else
  {
   IsRetreating = false;
   //IsAttacking = true;
   _navMeshAgent.isStopped = true;
   _navMeshAgent.ResetPath();
   if (_attackTimer <= 0f)
   {
    Attack();
    _attackTimer = attackCooldown;
   }
  }
 }

 private void RetreatFromPlayer()
 {
  IsRetreating = true;
  _navMeshAgent.isStopped = false;
  Vector3 directionAway = (transform.position - Player.Instance.transform.position).normalized;
  Vector3 retreatPosition = transform.position + directionAway * 3f;
  _navMeshAgent.SetDestination(retreatPosition);
 }
 public bool IsMoving
 {
  get
  {
   return _navMeshAgent.velocity.sqrMagnitude > 0.01f;
  }
 }

 public float MoveDirectionX
 {
  get
  {
   return _navMeshAgent.velocity.x;
  }
 }
 
 private void Roaming()
 {
  _navMeshAgent.isStopped = false;
  _roamPosition = GetRoamingPosition();
  _navMeshAgent.SetDestination(_roamPosition);
 }

 private Vector3 GetRoamingPosition()
 {
  return _startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
 }

 private void LateUpdate()
  {
   transform.rotation = Quaternion.identity;
  }

  public void SetDeathState()
  {
   if (_navMeshAgent.isOnNavMesh)
   {
    _navMeshAgent.isStopped = true;
    _navMeshAgent.ResetPath();
   }

   foreach (Collider2D col in _colliders)
   {
    col.enabled = false;
   }

   enabled = false;
   //archerVisual.SetDead();
   //enabled = false;
  }
 
}
