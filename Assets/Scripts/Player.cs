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
    public float StartPositionX = 0f;
    Camera _camera;
    int jumps;

    IEnumerator Start()
    {
        _camera = Camera.main;
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        JumpBtn.onClick.AddListener(Jump);
        ShootBtn.onClick.AddListener(Shoot);
        PauseBtn.onClick.AddListener(PauseButtonHandler);
        // AudioManager need to be loaded before GameManager
        yield return new WaitUntil(() => AudioManager.Instance.IsInitialized);
        GameManager.Instance.updateAllCounters();
        resetJumps();
    }
    public void SetButtons(bool value)
    {
        Button[] buttons = { JumpBtn, ShootBtn, PauseBtn };
        foreach (Button btn in buttons)
        {
            btn.interactable = value;
        }
    }
    void resetJumps()
    {
        jumps = GameManager.Instance.maxJumps;
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
        Debug.Log("pause btn");
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
        if (jumps-- <= 0) return;
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxJumpSpeed, maxJumpSpeed);
        PlaySound(jump);
    }

    public void Shoot()
    {
        PlaySound(GameManager.Instance.gun.ShootSound);
        Instantiate(Bullet, BulletPos.transform.position, BulletPos.transform.rotation);
        Debug.Log("shoot: bang");
        if (AudioManager.Instance.IsScreenShake)
        {
            _camera.GetComponent<CameraShake2D>().Shake();
        }

        if (AudioManager.Instance.IsEarRinging)
        {
            GunTinnitus.Instance.TriggerTinnitus();
        }

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
        var tag = collision.gameObject.tag;

        if (tag == "Obstacle")
        {
            // do falling glass or metal pipe thing lol
            // maybe 3 options, metal, wood, glass for cars trees and barrels
            PlaySound(hurt);
            Destroy(collision.gameObject);
            GameManager.Instance.Lives--;
        }
        else if (tag == "Enemy")
        {
            // change later
            PlaySound(hurt);
            Destroy(collision.gameObject);
            GameManager.Instance.Lives--;
        }
        else if (tag == "Ground")
        {
            resetJumps();
        }
    }

    public void GetCoins(CoinsEarned type)
    {
        GameManager.Instance.coins += (int)type;
        PlaySound(coinSound);
    }


    public IEnumerator ExitRight()
    {
        return movePlayerOutOfScreen(false);
    }
    public IEnumerator EnterLeft()
    {
        return movePlayerOutOfScreen(true);
    }

    private IEnumerator movePlayerOutOfScreen(bool isLeft)
    {

        float startX;
        float endX;
        if (isLeft) // enter
        {
            startX = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 5f;
            endX = StartPositionX;
        }
        else // exit
        {
            startX = transform.position.x;
            endX = _camera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 5f;
        }

        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        float duration = GameManager.Instance.SwitchLevelDuration;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            float x = Mathf.Lerp(startX, endX, easedT);
            // keep live Y + Z (jump, gravity, etc.)
            transform.position = new Vector3(
                x,
                transform.position.y,
                transform.position.z
            );
            yield return null;
        }
        transform.position = new Vector3(endX, transform.position.y, transform.position.z);
    }

}
