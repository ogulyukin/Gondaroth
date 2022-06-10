using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float flySpeed = 1;
        [SerializeField] private bool isHomed;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private float maxLifeTime = 10;
        [SerializeField] private GameObject[] toDestroy;
        [SerializeField] private float lifeAfterImpact = 0.2f;
        [SerializeField] private UnityEvent onHit;
        private Vector3 _startPoint;
        private float _range;
        private float _damage = 0;
        private Health _target;
        private GameObject _instigator;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (Vector3.Distance(_startPoint, transform.position) > _range) GetComponent<Rigidbody>().useGravity = true;
            if (_target == null) return;
            if(isHomed && _target.IsAlive()) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * Time.deltaTime * flySpeed);
        }

        public void SetRange(float range)
        {
            _range = range;
        }
        private Vector3 GetAimLocation()
        {
            var targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return _target.transform.position;
            }

            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _startPoint = transform.position;
            _instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            var hitTarget = other.GetComponent<Health>();
            if (hitTarget != null)
            {
                if(!hitTarget.IsAlive()) return;
                hitTarget.GetComponent<Defence>().GetAttack(1000, _damage, _instigator);
            }

            flySpeed = 0;
            if (hitEffect != null) Instantiate(hitEffect, transform.position, transform.rotation);
            onHit?.Invoke();
            foreach (var i in toDestroy)
            {
                Destroy(i);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
