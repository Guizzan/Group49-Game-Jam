using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : PathFinder
{
    public EnemyAIState State;
    public float MinDamage;
    public float MaxDamage;
    public bool Run;
    public List<Transform> PatrolPoints;
    public List<int> _usedPatrolPoints;
    public float fieldOfViewAngle = 90f;
    public float detectionRange = 10f;
    public float TimeOut;
    private int _curPatrolPoint = -1;
    private IEnumerator _attackingRoutine;
    [SerializeField]
    private AttackTrigger attackTrigger;
    private IEnumerator _timeOutRoutine;

    public enum EnemyAIState
    {
        Patrolling,
        Following,
        Attacking
    }
    private void Start() => Initialize();

    private void Update()
    {
        PathFindingCycle();
        StateMachine();
    }

    private void StateMachine()
    {
        switch (State)
        {
            case EnemyAIState.Patrolling:
                CanMove = true;
                Run = false;
                if (IsPlayerInFOV())
                    State = EnemyAIState.Following;
                if (_curPatrolPoint == -1)
                {
                    SetDestination(GetNavMeshPoint(GetPatrolPoint().position));
                }
                break;
            case EnemyAIState.Following:
                CanMove = true;
                Run = true;
                if (attackTrigger.IsPlayerInRange())
                {
                    State = EnemyAIState.Attacking;
                    break;
                }
                else
                {
                    if (_timeOutRoutine == null)
                    {
                        _timeOutRoutine = TimeOutRoutine(EnemyAIState.Patrolling);
                        StartCoroutine(_timeOutRoutine);
                    }
                }
                SetDestination(GetNavMeshPoint(PlayerController.Instance.transform.position));
                if (_timeOutRoutine == null && _invalidPath)
                {
                    _timeOutRoutine = TimeOutRoutine(EnemyAIState.Patrolling);
                    StartCoroutine(_timeOutRoutine);
                }
                break;
            case EnemyAIState.Attacking:
                CanMove = false;
                if (!attackTrigger.IsPlayerInRange())
                {
                    State = EnemyAIState.Following;
                    break;
                }
                if (_attackingRoutine == null)
                {
                    _attackingRoutine = AttackingRoutine();
                    StartCoroutine(_attackingRoutine);
                }
                break;
        }
    }



    public override void OnMove(bool ShouldMove, Vector2 Motion)
    {
        _animator.SetBool("Walking", ShouldMove);
        _animator.SetBool("Running", Run);
        _animator.SetFloat("PosX", Motion.x);
        _animator.SetFloat("PosY", Motion.y);
    }

    public override void PathCompleted()
    {
        print("ReachedPath!");
        switch (State)
        {
            case EnemyAIState.Patrolling:
                _usedPatrolPoints.Add(_curPatrolPoint);
                if (_usedPatrolPoints.Count == PatrolPoints.Count)
                {
                    _usedPatrolPoints.Clear();
                }
                _curPatrolPoint = -1;
                break;
        }
    }

    private Transform GetPatrolPoint()
    {
        if (PatrolPoints.Count == 0)
        {
            throw new System.Exception("Patrol list is empty!");
        }
        int num = Random.Range(0, PatrolPoints.Count);
        while (_usedPatrolPoints.Contains(num))
        {
            num = Random.Range(0, PatrolPoints.Count);
        }
        _curPatrolPoint = num;
        return PatrolPoints[_curPatrolPoint];
    }

    IEnumerator AttackingRoutine()
    {
        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        if (attackTrigger.IsPlayerInRange())
        {
            //print("Succesfull Attack!");
            SoundManager.PlaySound("ZombieAttack",transform.position);
            float amount = Random.Range(MinDamage, MaxDamage);
            if (PlayerController.Instance != null)
                PlayerController.Instance.GetComponent<LivingEntity>().GetDamage(amount);
        }
        _attackingRoutine = null;
    }

    IEnumerator TimeOutRoutine(EnemyAIState state)
    {
        float timer = 0;
        while (timer <= TimeOut)
        {
            if (!_invalidPath)
            {
                _timeOutRoutine = null;
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        _timeOutRoutine = null;
        State = state;
    }


    public bool IsPlayerInFOV()
    {
        Vector3 directionToPlayer = PlayerController.Instance.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle <= fieldOfViewAngle * 0.5f)
        {
            if (directionToPlayer.magnitude <= detectionRange)
            {
                return true;
            }
        }

        return false;
    }
}
