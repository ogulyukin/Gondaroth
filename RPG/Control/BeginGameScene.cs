using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Magic;
using RPG.Movement;
using RPG.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

namespace RPG.Control
{
    public class BeginGameScene : MonoBehaviour
    {
        [SerializeField] private GameObject director;
        [SerializeField] private GameObject director2;
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private GameObject pauseCanvas;
            
        [SerializeField] private float maxSpeed = 4.66f;
           
        private Vector3 m_GuardPosition;
            
        private int m_CurrentWaypointIndex;
           
        private float m_TimeSinceLastSawPlayer = Mathf.Infinity;
        private float m_EndScript = 10f;
            
        private float m_TimeSinceArriveAtWaypoint = Mathf.Infinity;
        private const float WaypointTolerance = 1f;
        private GameObject m_FirstEnemy;
        private CombatTarget m_Target;
        private Fighter m_Fighter;
        private Mover m_Mover;
        private Health m_Health;
        private NavMeshAgent m_NavMeshAgent;
        private bool _spellCasted = false;
        private void Start()
        {
            m_FirstEnemy = GameObject.FindWithTag("DarkElf");
            m_Target = m_FirstEnemy.GetComponent<CombatTarget>();
            m_Fighter = GetComponent<Fighter>();
            m_Mover = GetComponent<Mover>();
            m_Health = GetComponent<Health>();
            if (m_Health == null) print("DE Health component == null");
            m_CurrentWaypointIndex = 0;
            m_GuardPosition = (patrolPath != null) ? GetCurrentWaypoint() : transform.position;
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            GetComponent<Animator>().SetFloat("Locomotion", 0);
            GetComponent<Animator>().SetFloat("CombatStyle", 0);
            pauseCanvas.GetComponent<PauseCanvas>().TurnOffAvatarCamera();
        }
        private void Update()
        {
            if (m_EndScript < 8f)
            {
                m_EndScript += Time.deltaTime;
                if(m_EndScript > 3f)  GetComponent<PlayerController>().enabled = true;
            }
            if(!m_Health.IsAlive()) return;
            
            if (GetIsInChaseRange())
            {
                AttackBehaviour();
            }else if (m_TimeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehaviour();
            }
        }
    
        
        private void PatrolBehaviour()
        {
            //Vector3 nextPosition = m_GuardPosition;
            if (patrolPath == null)
            {
                m_Mover.StartMoving(m_GuardPosition);
                return;
            }
            if (AtWaypoint())
            {
                CycleWayPoint();
                GetCurrentWaypoint();
            }
                
            if (m_TimeSinceArriveAtWaypoint > waypointDwellTime)
            {
                if (m_CurrentWaypointIndex == 2 && !_spellCasted)
                {
                    GetComponent<MagicControl>().TriggerCast();
                    _spellCasted = true;
                    m_TimeSinceArriveAtWaypoint = 0;
                    return;
                }
                m_Mover.StartMoving(m_GuardPosition);
            }
            else
            {
                m_TimeSinceArriveAtWaypoint += Time.deltaTime;
            }
        }
    
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(m_CurrentWaypointIndex);
        }
    
        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < WaypointTolerance;
        }
    
        private void CycleWayPoint()
        {
            m_NavMeshAgent.speed = maxSpeed / 4;
            m_CurrentWaypointIndex++;
            if (m_CurrentWaypointIndex == 1)
            {
                pauseCanvas.GetComponent<PauseCanvas>().SetPauseText("<i>Elende</i>, the yong elven huntress, meet her first challenge after years of training. " +
                                                                     "She was send to the  wilderness near her native villadge <i>Gondgaroth</i> scouting for " +
                                                                     "present of wild wolves. Her perсeption and spear was enough for such task. " +
                                                                     "At least her trainer <i>Aurion</i> thinks so.\n" +
                                                                     "\nUnfortunately, no one could have predicted what happen soon.\n" + 
                                                                     "\nShe woke up early in the morning with a sharp premonition of danger. It was to quiet. " +
                                                                     "No birds or animals around. Suddenly she notice moving near the bridge...");
                pauseCanvas.GetComponent<PauseCanvas>().PauseCanvasHandler();
                GetComponent<IdleBreak>().StartIdleBreak();
            }
            
            if (m_CurrentWaypointIndex == 2)
            {
                transform.LookAt(m_FirstEnemy.transform);
                director.GetComponent<PlayableDirector>().Play();
                GetComponent<Animator>().SetFloat("CombatStyle", 1f);
                pauseCanvas.GetComponent<PauseCanvas>().SetPauseText("Damn, what is this? Dark elves here, near <i>Gondaroth</i>? She read about dark elves in old books. " +
                                                                     "They are cruel and ruthless to their enemies. And they considered the forest " +
                                                                     "elves to be their worst enemies. And...\nThey are great warriors - very, very dangerous.");
                pauseCanvas.GetComponent<PauseCanvas>().PauseCanvasHandler();
                GetComponent<Taunt>().StartTaunting();
                m_NavMeshAgent.speed = maxSpeed;
            }

            if (m_CurrentWaypointIndex == 3)
            {
                GetComponent<Animator>().SetFloat("CombatStyle", 0f);
                transform.LookAt(m_FirstEnemy.transform);
                //GetComponent<Taunt>().StartTaunting();
                pauseCanvas.GetComponent<PauseCanvas>().SetPauseText("She need to warn guard about this new danger. <i>Aurion</i> wait her on north in the forest, and she have to " +
                                                                     "report to him of cause. But somewhere here have to be elven guards patrol to the west by the road.");
                pauseCanvas.GetComponent<PauseCanvas>().PauseCanvasHandler();
                director2.GetComponent<PlayableDirector>().Play();
                m_EndScript = 0;
            }
            if (m_CurrentWaypointIndex == patrolPath.transform.childCount)
            {
                m_CurrentWaypointIndex = 0;
            }
            
            m_GuardPosition = GetCurrentWaypoint();
            m_TimeSinceArriveAtWaypoint = 0f;
        }
    
        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            m_TimeSinceLastSawPlayer += Time.deltaTime;
            GetComponent<IdleBreak>().StartIdleBreak();
        }
    
        private void AttackBehaviour()
        {
            m_TimeSinceLastSawPlayer = 0;
            m_NavMeshAgent.speed = maxSpeed;
            if(m_Fighter.CanAttack(m_Target))
                m_Fighter.Attack(m_Target);
        }
            
            
        private bool GetIsInChaseRange()
        {
            return Vector3.Distance(transform.position, m_FirstEnemy.transform.position) < chaseDistance;
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
