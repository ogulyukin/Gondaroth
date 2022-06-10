using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour , IAction, ISaveable
    {
        [FormerlySerializedAs("Target")] [SerializeField] private Transform target;
        [SerializeField] private float maxNavMeshPathLength = 40f;
        [SerializeField] private float maxSpeed = 6.74f;
        [SerializeField] private float walkSpeed = 1.385f;
        [SerializeField] private GameObject runIcon;
        [SerializeField] private float runPrice = 1;
        private bool _isRunning;
        private NavMeshAgent _navMeshAgent;
        private static readonly int ForwardSpeed = Animator.StringToHash("Locomotion");
        private Health _health;
        private Stamina _stamina;
        private Vector3 _previonusPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _stamina = GetComponent<Stamina>();
            if(runIcon != null) runIcon.SetActive(false);
            _navMeshAgent.speed = walkSpeed;
            _previonusPosition = transform.position;
        }

        private void Update()
        {
            _navMeshAgent.enabled = _health.IsAlive();
            UpdateAnimator();
            if (_isRunning && _previonusPosition != transform.position)
            {
                if(!_stamina.UseSkill(runPrice)) SetNavMeshSpeed(false);
            }

            _previonusPosition = transform.position;
        }
        
        public void ToggleNavMeshSpeed()
        {
            _isRunning = !_isRunning;
            _navMeshAgent.speed = _isRunning ? maxSpeed : walkSpeed;
            if (runIcon != null)
            {
                runIcon.SetActive(_isRunning);
            }
        }

        public void SetNavMeshSpeed(bool flag)
        {
            _isRunning = flag;
            _navMeshAgent.speed = _isRunning ? maxSpeed : walkSpeed;
            if (runIcon != null)
            {
                runIcon.SetActive(_isRunning);
            }
        }
        public bool CanMoveTo(Vector3 destination)
        {
            var path = new NavMeshPath();
            if (!NavMesh.CalculatePath(transform.position,destination, NavMesh.AllAreas, path)) return false;
            if(path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavMeshPathLength) return false;
            return true;
        }
        
        private float GetPathLength(NavMeshPath path)
        {
            var total = 0f;
            if (path.corners.Length < 2) return total;
            for(var i = 0;  i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public void StartMoving(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            Moveto(destination);
        }

        public void StartRotation(bool direction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            var rotation = new Vector3(0, direction ? 5 : -5, 0);
            transform.Rotate(rotation);
        }

        public void Moveto(Vector3 destination)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.isStopped = false;
        }
        
        private void UpdateAnimator()
        {
            Vector3 velocity = transform.InverseTransformDirection(_navMeshAgent.velocity);
            GetComponent<Animator>().SetFloat("Locomotion", velocity.z);
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            var transform1 = transform;
            var data = new Dictionary<string, object>
            {
                { "position", new SerializableVector3(transform1.position) },
                { "rotation", new SerializableVector3(transform1.eulerAngles) }
            };
            return data;
        }

        public void RestoreState(object state)
        {
            //var data = (Dictionary<string, object>)state;
            if (state is Dictionary<string, object> data)
            {
                _navMeshAgent.enabled = false;
                transform.position = ((SerializableVector3)data["position"]).ToVector();
                transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
                _navMeshAgent.enabled = true;
            }
        }
    }
}
