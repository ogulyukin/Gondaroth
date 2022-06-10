using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private float agroCooldownTime = 5f;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float shoutDistance = 5f;
        private Vector3 _guardPosition;
        private int _currentWaypointIndex;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArriveAtWaypoint = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        private const float WaypointTolerance = 1f;
        private GameObject _player;
        private CombatTarget _target;
        private Fighter _fighter;
        private Mover _mover;
        private Health _health;
        private NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _target = _player.GetComponent<CombatTarget>();
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _currentWaypointIndex = 0;
            _guardPosition = (patrolPath != null) ? GetCurrentWaypoint() : transform.position;
        }
        private void Update()
        {
            if(!_health.IsAlive()) return;
            if ( IsAggrevated() && _fighter.CanAttack(_player.GetComponent<CombatTarget>()))
            {
                AttackBehaviour();
            }else if (_timeSinceLastSawPlayer < suspicionTime)
            {
               SuspicionBehavior();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0;
        }

        private void PatrolBehaviour()
        {
            //Vector3 nextPosition = m_GuardPosition;
            if (patrolPath == null)
            {
                _mover.StartMoving(_guardPosition);
                return;
            }
            if (AtWaypoint())
            {
                CycleWayPoint();
                GetCurrentWaypoint();
            }
            
            if (_timeSinceArriveAtWaypoint > waypointDwellTime)
            {
                _mover.StartMoving(_guardPosition);
            }
            else
            {
                _timeSinceArriveAtWaypoint += Time.deltaTime;
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < WaypointTolerance;
        }

        private void CycleWayPoint()
        {
            _mover.SetNavMeshSpeed(false);
            _currentWaypointIndex++;
            if (_currentWaypointIndex == patrolPath.transform.childCount)
            {
                _currentWaypointIndex = 0;
            }
            _guardPosition = GetCurrentWaypoint();
            _timeSinceArriveAtWaypoint = 0f;
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            _timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _mover.SetNavMeshSpeed(true);
            if(_fighter.CanAttack(_target))
                _fighter.Attack(_target);
            AggrevateNearblyEnemies();
        }

        private void AggrevateNearblyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (var hit in hits)
            {
                hit.transform.GetComponent<AIController>()?.Aggrevate();
            }
        }


        private bool IsAggrevated()
        {
            if (_timeSinceAggrevated < agroCooldownTime)
            {
                _timeSinceAggrevated += Time.deltaTime;
                return true;
            }
            return Vector3.Distance(transform.position, _player.transform.position) < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
