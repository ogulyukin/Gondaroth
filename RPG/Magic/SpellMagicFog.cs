using System;
using RPG.Combat;
using RPG.Core;
using UnityEngine;

namespace RPG.Magic
{
    public class SpellMagicFog : MonoBehaviour, ISpell
    {
        [SerializeField] float manaCost;
        [SerializeField] private bool mainAnimation;
        
        [SerializeField] private SpellEffectTargets targets;
        [SerializeField] private float duration;
        [SerializeField] private bool stunningEffect;
        [SerializeField] private bool charmEffect;
        [SerializeField] private bool silenceEffect;
        [SerializeField] private bool sleepEffect;
        [SerializeField] private float healEffect;
        [SerializeField] private float damageEffect;
        
        [SerializeField] private Material magicFogMaterial;
        [SerializeField] private float fogStartSize = 5;
        [SerializeField] private Color fogColor = Color.white;
        [SerializeField] private Color fogLining = Color.gray;
        
        private GameObject _magicFog;
        private float _magicFogTime = 0;
        private SpellEffect _spellEffect;

        private void Awake()
        {
            _spellEffect = GetComponent<SpellEffect>();
        }

        private void Update()
        {
            if (!ReferenceEquals(_magicFog, null))
            {
                _magicFogTime += Time.deltaTime;
                if (_magicFogTime > (_spellEffect.duration + 1)) Destroy(_magicFog);
            }
        }

        public float Cast(Transform target, Transform caster)
        {
            var cloudGO = new GameObject()
            {
                name = "MassSpell",
                transform =
                {
                    rotation = caster.rotation,
                    position = caster.position,
                }
            };
            var fogSystem = cloudGO.AddComponent<ParticleSystem>();
            var fogRend = cloudGO.GetComponent<Renderer>();
            fogRend.material = magicFogMaterial;
            fogRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            fogRend.receiveShadows = false;
            
            var main = fogSystem.main;
            main.loop = false;
            main.startLifetime = duration;
            main.startSpeed = 0;
            main.startSize = fogStartSize;
            main.startColor = fogColor;
            var emission = fogSystem.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0.0f, 100)});
            var shape = fogSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.scale = new Vector3(8, 8, 8);
            var particles = new ParticleSystem.Particle[fogSystem.particleCount];
            fogSystem.GetParticles(particles);
            for (var i = 0; i < particles.Length; i++)
            {
                particles[i].startColor =
                    Color.Lerp(fogLining, fogLining, particles[i].position.y / fogSystem.shape.scale.y);
            }
            fogSystem.SetParticles(particles, particles.Length);
            
            cloudGO.transform.localScale = new Vector3(1, 1, 1);
            _magicFog = cloudGO;
            _magicFogTime = 0;
            var spellCollider = cloudGO.AddComponent<CapsuleCollider>();
            spellCollider.radius = 8f;
            spellCollider.height = 4f;
            spellCollider.isTrigger = true;
            spellCollider.enabled = true;
            
            var spellType = cloudGO.AddComponent<SpellEffect>();
            //spellType.spellType = Core.Spells.MagicFog;
            CreteMagicEffect(spellType, caster.GetComponent<CombatTarget>().GetUnitType());
            return manaCost;
        }
        
        public bool CanCast(float mana)
        {
            return mana >= manaCost;
        }

        public bool GetAnimationWay()
        {
            return mainAnimation;
        }


        private void CreteMagicEffect(SpellEffect spellType, UnitTypes caster)
        {
            spellType.duration = duration;
            spellType.caster = caster;
            spellType.targets = targets;
            spellType.charmEffect = charmEffect;
            spellType.damageEffect = damageEffect;
            spellType.healEffect = healEffect;
            spellType.sleepEffect = sleepEffect;
            spellType.stunningEffect = stunningEffect;
            spellType.silenceEffect = silenceEffect;
        }
    }
}
