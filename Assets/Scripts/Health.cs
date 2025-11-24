using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int healthPoints = 50;
    [SerializeField] ParticleSystem hitParticles;

    [SerializeField] bool applyCameraShake;
    CameraShake cameraShake;
    AudioManager audioManager;

    void Start()
    {   
        cameraShake = Camera.main.GetComponent<CameraShake>();   
        audioManager = FindObjectOfType<AudioManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer != null)
        {
            TakeDamage(damageDealer.GetDamage());
            PlayHitParticles();
            damageDealer.Hit();
            audioManager.PlayDamageSFX();

            if (applyCameraShake)
            {
                cameraShake.Play();
            }
        }
    }

    void TakeDamage(int damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    void PlayHitParticles()
    {
        if (hitParticles != null)
        {
            ParticleSystem particles = Instantiate(hitParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles, particles.main.duration + particles.main.startLifetime.constantMax);
        }
    }
}
