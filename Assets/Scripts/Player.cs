using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] AudioClip coinSound;
    [SerializeField] Button JumpBtn;
    [SerializeField] Button ShootBtn;
    [SerializeField] Button PauseBtn;
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject BulletPos;
    [SerializeField] GameObject GunObj;



    IEnumerator Start()
    {
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        JumpBtn.onClick.AddListener(Jump);
        ShootBtn.onClick.AddListener(Shoot);
        PauseBtn.onClick.AddListener(PauseButtonHandler);
        // AudioManager need to be loaded before GameManager
        yield return new WaitUntil(() => AudioManager.Instance.IsInitialized);
        GameManager.Instance.updateAllCounters();
    }

    private void SetupBindings()
    {
        jumpAction = new InputAction("Press", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.Enable();
        jumpAction.performed += InputDoJump;

    }
    void PauseButtonHandler()
    {
        PauseGame.Instance.TogglePaused();
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
        var waitForShotToFade = 0.8f;
        yield return new WaitForSeconds(waitForShotToFade);
        // start the animation
        PlayAnimation();

        yield return new WaitForSeconds(gun.reloadTimeSeconds - waitForShotToFade);
        GameManager.Instance.GreyOutInAmmo(false);
        ShootBtn.interactable = true;
        // Code to execute after the delay
        Debug.Log("Function executed after " + gun.reloadTimeSeconds + " seconds!");
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // do falling glass or metal pipe thing lol
            // maybe 3 options, metal, wood, glass for cars trees and barrels
            PlaySound(hurt);
            Destroy(collision.gameObject);
            GameManager.Instance.Lives--;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // change later
            PlaySound(hurt);
            Destroy(collision.gameObject);
            GameManager.Instance.Lives--;
        }
    }
    public void GetCoins(CoinsEarned type)
    {
        GameManager.Instance.coins += (int)type;
        PlaySound(coinSound);
    }


    public IEnumerator ExitRight()
    {
        Camera cam = Camera.main;
        var speed = 20f;
        float rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 5f;

        while (transform.position.x < rightEdge)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            yield return null;
        }
    }
}
