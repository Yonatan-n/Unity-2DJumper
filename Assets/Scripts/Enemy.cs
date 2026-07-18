using UnityEngine;

public class Enemy : MonoBehaviour
{
    int Health;
    int Shield;
    Rigidbody2D rb;
    [SerializeField] GameObject ShieldObj;
    [SerializeField] GameObject bloodPrefab;
    [SerializeField] GameObject shieldBreak;

    public void Init(int health, int shield = 0)
    {
        Health = health;
        Shield = shield;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public bool TakeDamage(int amount)
    {
        if (Shield > 0)
        {
            Shield -= amount;
            if (Shield > 0) return false;
            else
            {
                BreakShield();
                return false;
            }
        }
        Health -= amount;
        AudioManager.Instance.EnemyIsHit();
        if (Health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }
    private void BreakShield()
    {
        AudioManager.Instance.ShieldBroke();
        var _shield_anim = ShieldObj.GetComponent<Animator>();
        _shield_anim.SetTrigger("IsBroken");
        var shieldParticles = Instantiate(shieldBreak, transform.position, Quaternion.identity);
        var ps = shieldParticles.GetComponent<ParticleSystem>();
        var velocityModule = ps.velocityOverLifetime;
        velocityModule.x = -(GroundMover.Instance.speed * Obstacle.GetSpeedIncrease(gameObject));
        StatsTracker.Instance.OnShieldEnemyKilled();
    }
    private void Die()
    {
        var collider = GetComponent<Collider2D>();
        var anim = GetComponent<Animator>();
        var flyingAnim = GetComponentInChildren<FlyingAnimation>();
        if (flyingAnim != null)
        {
            flyingAnim.stop = true;
        }
        anim.SetTrigger("IsDie");
        collider.enabled = false; // no more colliding
        rb.simulated = true; // remove
        rb.gravityScale = 1f;
        rb.AddForce(new Vector2(Random.Range(-3f, 3f), 20f), ForceMode2D.Impulse);
        rb.angularVelocity = Random.Range(-200f, 200f);
        // blood fx
        Instantiate(bloodPrefab, transform);
        StatsTracker.Instance.OnEnemyKilled();
        GameManager.Instance.EnemyKilledThisRun();
        if (gameObject.CompareTag(Tags.FlyingEnemy))
            StatsTracker.Instance.OnFlyingEnemyKilled();
    }
}