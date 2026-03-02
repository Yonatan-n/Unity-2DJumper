using UnityEngine;

public class Enemy : MonoBehaviour
{
    int Health;
    int Shield;
    [SerializeField] GameObject ShieldObj;
    [SerializeField] GameObject bloodPrefab;

    public void Init(int health, int shield = 0)
    {
        Health = health;
        Shield = shield;
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
    }
    private void Die()
    {
        var collider = GetComponent<Collider2D>();
        var rb = GetComponent<Rigidbody2D>();
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
    }
}