using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class Player : MonoBehaviour
{

    public int score;
    InputAction jumpAction;
    Rigidbody2D rb;
    AudioSource audioSource;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float maxJumpSpeed = 20f;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip hurt;
    [SerializeField] Button JumpBtn;
    [SerializeField] Button ShootBtn;
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject BulletPos;
    [SerializeField] GameObject GunObj;



    void Start()
    {
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        JumpBtn.onClick.AddListener(Jump);
        ShootBtn.onClick.AddListener(Shoot);
        GameManager.Instance.updateAllCounters();

    }

    private void SetupBindings()
    {
        jumpAction = new InputAction("Press", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.Enable();
        jumpAction.performed += InputDoJump;

    }

    void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    void PlayAnimation()
    {
        var animator = GunObj.GetComponent<Animator>();
        animator.Play("Gun1911Reload");
    }
    void InputDoJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxJumpSpeed, maxJumpSpeed);
        PlaySound(jump);
    }

    public void Shoot()
    {
        PlaySound(GameManager.Instance.gun.ShootSound);
        Instantiate(Bullet, BulletPos.transform.position, BulletPos.transform.rotation);
        Debug.Log("shoot: bang");
        if (--GameManager.Instance.Ammo == 0)
        {
            StartCoroutine(PerformReload());
        }
    }
    IEnumerator PerformReload()
    {
        var gun = GameManager.Instance.gun;
        GameManager.Instance.GreyOutInAmmo(true);
        ShootBtn.interactable = false;
        // block icon and shoot button
        // start to play sound 
        // throw gun and catch
        // after waitinf for sound and wepon catch, unblock ui, change counter

        // wait for the shoot to finish
        yield return new WaitForSeconds(0.2f);
        // start the animation
        PlayAnimation();
        // start the sound clip so it will end with the animation
        // yield return new WaitForSeconds(2.5f - gun.reloadSound.length);
        PlaySound(gun.reloadSound);

        yield return new WaitForSeconds(gun.reloadTimeSeconds); // Wait for the specified time
        GameManager.Instance.GreyOutInAmmo(false);
        ShootBtn.interactable = true;
        // Code to execute after the delay
        Debug.Log("Function executed after " + gun.reloadTimeSeconds + " seconds!");
    }


    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlaySound(hurt);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // change later
            PlaySound(hurt);
            Destroy(collision.gameObject);
        }
    }
}
