using RPG.Attributes;
using RPG.Core;
using RPG.Spells;
using UnityEngine;

namespace RPG.Combat
{
    public class MagicControl : MonoBehaviour, IAction
    {
        [SerializeField] private float manaCostForSpell = 60.0f;
        [SerializeField] private Material magicFogMaterial;
        [SerializeField] private float fogStartSize = 5;
        [SerializeField] private Color fogColor = Color.white;
        [SerializeField] private Color fogLining = Color.gray;
        [SerializeField] private float fogTime = 10f;
        private Mana m_Mana;
        private Health m_Health;
        private GameObject m_MagicFog;
        private float m_MagicFogTime = 0;

        private void Start()
        {
            m_Health = GetComponent<Health>();
            m_Mana = GetComponent<Mana>();
        }
        private void Update()
        {
            if (m_MagicFog != null)
            {
                m_MagicFogTime += Time.deltaTime;
                if (m_MagicFogTime > (fogTime + 1)) Destroy(m_MagicFog);
            }
        }
        public bool CanCastSpell()
        {
            return (m_Health.IsAlive() && m_Mana.CheckManaAvaible(manaCostForSpell));
        }
        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("Cast01");
            GetComponent<Animator>().ResetTrigger("Cast2Start");
        }
    
        public void TriggerCast()
        {
            m_Mana.SpendMana(manaCostForSpell);
            GetComponent<ActionScheduler>().StartAction(this);
            GetComponent<Animator>().SetTrigger("Cast01");
            //GenerateMagicFog();
        }
        public void TriggerCast2()
        {
            m_Mana.SpendMana(manaCostForSpell);
            GetComponent<ActionScheduler>().StartAction(this);
            GetComponent<Animator>().SetTrigger("Cast2Start");
            //GetComponent<Health>().RestoreHeath(20);
        }

        private void GenerateMagicFog()
        {
            var cloudGO = new GameObject()
            {
                name = $"MassSpell",
                transform =
                {
                    rotation = GetComponent<CombatTarget>().transform.rotation,
                    position = GetComponent<CombatTarget>().transform.position,
                }
            };
            var fogSystem = cloudGO.AddComponent<ParticleSystem>();
            var fogRend = cloudGO.GetComponent<Renderer>();
            fogRend.material = magicFogMaterial;
            fogRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            fogRend.receiveShadows = false;
            
            var main = fogSystem.main;
            main.loop = false;
            main.startLifetime = fogTime;
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
            m_MagicFog = cloudGO;
            m_MagicFogTime = 0;
            var spellCollider = cloudGO.AddComponent<CapsuleCollider>();
            spellCollider.radius = 8f;
            spellCollider.height = 4f;
            spellCollider.isTrigger = true;
            spellCollider.enabled = true;
            
            var spellType = cloudGO.AddComponent<Spell>();
            spellType.spellType = Core.Spells.MagicFog;
            spellType.duration = 10;
            spellType.caster = UnitTypes.Elves;
        }
    }
}
