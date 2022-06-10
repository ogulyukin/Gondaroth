using UnityEngine;
using Random = UnityEngine.Random;

public class CloudController : MonoBehaviour
{
    private ParticleSystem m_CloudSystem;
    private bool m_Painted = false;
    private Vector3 m_StartPosition;
    private float m_Speed;
    public Color color;
    public Color lining;
    public float distance = 10;
    public int numberOfParticles;
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.1f;

    private void Start()
    {
        m_CloudSystem = GetComponent<ParticleSystem>();
        m_StartPosition = transform.position;
        Spawn();
    }

    private void Update()
    {
        if (!m_Painted)
        {
            Paint();
        }
        transform.Translate(0,0, m_Speed);
        if (Vector3.Distance(transform.position, m_StartPosition) > distance)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        transform.localPosition =
            new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        m_Speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Paint()
    {
        var particles = new ParticleSystem.Particle[m_CloudSystem.particleCount];
        m_CloudSystem.GetParticles(particles);
        if (particles.Length > 0)
        {
            for (var i = 0; i < particles.Length; i++)
            {
                particles[i].startColor =
                    Color.Lerp(lining, color, particles[i].position.y / m_CloudSystem.shape.scale.y);
            }

            m_Painted = true;
            m_CloudSystem.SetParticles(particles, particles.Length);
        }
    }
}
