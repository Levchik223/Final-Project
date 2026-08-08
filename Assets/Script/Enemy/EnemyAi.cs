using System;
using UnityEngine;
using UnityEngine.AI;
using WarriorHunt.Utils;
public class EnemyAi : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDinstanceMax = 7f;
    [SerializeField] private float roamingDinstanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;
    [SerializeField] private bool isChasingEnemy;

    [SerializeField] float chasingDistance = 4f;
    [SerializeField] float chasingSpeedMultiplier = 2f;
    [SerializeField] float attackRate = 1f;
    
    [SerializeField] private bool isAttackingEnemy;
    [SerializeField] float attackingDistance = 2f;
    
    private float _nextAttackTime;
    private float _nextCheckDirectionTime;
    private readonly float _checkDirectDuration = 0.1f;
    private Vector3 _lastPosition;

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    public event EventHandler OnEnemyAttack;
    
    public bool IsRunning
    {
        get 
        {
            return _currentState == State.ROAMING || 
                   _currentState == State.CHASING;
        }
    }

    private enum State
    {
        IDLE,
        ROAMING,
        CHASING,
        ATTACKING,
        DEATH
    }
    private void Awake()
    {
       _navMeshAgent = GetComponent<NavMeshAgent>(); 
       _navMeshAgent.updateRotation = false;
       _navMeshAgent.updateUpAxis = false;
       _currentState = startingState;
       
       _roamingSpeed = _navMeshAgent.speed;
       _chasingSpeed = _navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
       StateHandler();
       MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        _currentState = State.DEATH;
        
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;
        
    }
    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.ROAMING:
                _roamingTimer -= Time.deltaTime;
                if (_roamingTimer < 0)
                {
                    Roaming();
                    _roamingTimer = roamingTimerMax;
                }
                CheckCurrentState();
                break;
            case State.CHASING:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.ATTACKING:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.DEATH:
                break;
            default:
            case State.IDLE:
                break;
            
        }
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }
    private void AttackingTarget()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + attackRate;
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime)
            if (IsRunning)
            {
                ChangeFacingDirection(_lastPosition, transform.position);
            }
            else if (_currentState == State.ATTACKING)
            {
               ChangeFacingDirection(transform.position, Player.Instance.transform.position); 
            }
        _lastPosition = transform.position;
        _nextCheckDirectionTime = Time.time * _checkDirectDuration;
    }
    private void ChasingTarget()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (distanceToPlayer > attackingDistance)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(Player.Instance.transform.position);
        }
        else
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
         
    }
    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.ROAMING;
        if (isChasingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.CHASING;
            }
        }
        if (isAttackingEnemy)
        {
            if (distanceToPlayer <= attackingDistance)
            {
                if (Player.Instance.isAlive)
                    newState = State.ATTACKING;
                else 
                    newState = State.ROAMING;
                
            }
        }

        if (newState != _currentState)
        {
            if (newState == State.CHASING)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.ROAMING)
            {
                _navMeshAgent.isStopped = false;
                _roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
            }
            else if(newState == State.ATTACKING)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
            }
            _currentState = newState;
        }
    }
    
    private void Roaming()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDinstanceMin, roamingDinstanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
