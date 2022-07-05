using UnityEngine;

namespace RPG.Magic
{
    public class Elf_ParticleControl : MonoBehaviour {

        public GameObject spearSmash;                                               // Magic spell
        public ParticleSystem[] castParticles;
        public Transform spearBase;
    
        public void SpearSmash()
        {
            GameObject newObject = Instantiate(spearSmash, spearBase.position, Quaternion.identity);
            Destroy(newObject, 5.0f);
        }

        public void CastStart()
        {
            for (int i = 0; i < castParticles.Length; i++)
            {
                castParticles[i].Play();
            }
        }

        public void CastEnd()
        {
            for (int i = 0; i < castParticles.Length; i++)
            {
                castParticles[i].Stop();
            }
        }
    }
}
