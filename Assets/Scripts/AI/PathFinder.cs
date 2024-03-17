using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public abstract class PathFinder : MonoBehaviour
{
    public bool CanMove = true;
    public float StoppingRadius = 1f;
    public float SmoothingValue = 0.05f;
    public float ShouldMoveValue = 0.3f;
    public NavMeshAgent agent;
    public Animator _animator;
    private Vector3 _targetPos;
    private Vector2 _velocity;
    private Vector2 _smoothDeltaPosition;
    private bool _pathCompleted;
    private bool _reset;
    protected bool _initialized;
    protected bool _invalidPath;

    protected virtual void SetDestination(Vector3 pos, NavMeshPath path = null)
    {
        if (!CanMove || !agent.enabled) return;
        if (path == null)
        {
            path = new NavMeshPath();
            agent.CalculatePath(pos, path);
        }
        switch (path.status)
        {
            case NavMeshPathStatus.PathComplete:
                //Debug.Log("Path is valid! Destination point: " + agent.destination);
                agent.SetPath(path);
                _pathCompleted = false;
                _targetPos = pos;
                agent.updateRotation = true;
                _reset = false;
                _invalidPath = false;
                break;
            case NavMeshPathStatus.PathInvalid:
                _invalidPath = true;
                break;
            case NavMeshPathStatus.PathPartial:
                _invalidPath = true;
                break;
        }
    }
    protected virtual void Initialize()
    {
        agent.enabled = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        if (NavMesh.SamplePosition(gameObject.transform.position, out NavMeshHit closestHit, 500f, NavMesh.AllAreas))
        {
            transform.position = closestHit.position;
            agent.enabled = true;
        }
        _initialized = true;
    }
    protected virtual void PathFindingCycle()
    {
        if (_pathCompleted || !agent.enabled || !agent.isOnNavMesh) return;
        if (!CanMove || _reset) { OnMove(false, Vector2.zero); return; }
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new(dx, dy);
        float smooth = Mathf.Min(1, Time.deltaTime / SmoothingValue);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);
        _velocity = _smoothDeltaPosition / Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            _velocity = Vector2.Lerp(Vector2.zero, _velocity, agent.remainingDistance / agent.stoppingDistance);
            if (Vector3.Distance(transform.position, _targetPos) <= StoppingRadius)
            {
                PathCompleted();
                ResetPathFinding();
                return;
            }
        }
        bool shouldMove = (_velocity.magnitude > ShouldMoveValue && agent.remainingDistance > agent.stoppingDistance);
        OnMove(shouldMove, _velocity);
        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / 2)
        {
            transform.position = Vector3.Lerp(_animator.rootPosition, agent.nextPosition, smooth);
        }

    }
    public void SetAgent(bool value) { agent.enabled = value; _reset = !value; OnMove(false, Vector2.zero); }
    public virtual void ResetPathFinding() { _pathCompleted = true; agent.updateRotation = false; _reset = true; OnMove(false, Vector2.zero); }
    public abstract void PathCompleted();
    protected Vector3 GetNavMeshPoint(Vector3 pos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 100f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            Debug.LogWarning("Current position is not on the NavMesh.");
            return transform.position;
        }
    }
    public abstract void OnMove(bool ShouldMove, Vector2 Motion);
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.DrawSphere(agent.destination, 0.5f);
    }
#endif
    private void OnAnimatorMove()
    {
        Vector3 rootPosition = _animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }
}
