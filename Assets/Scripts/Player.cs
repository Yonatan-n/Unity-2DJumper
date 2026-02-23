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
    [SerializeField] Button JumpBtn;
    [SerializeField] Button ShootBtn;
    [SerializeField] Button PauseBtn;
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject BulletPos;
    [SerializeField] GameObject GunAnimator;
    [SerializeField] AudioClip AKShoot;
    [SerializeField] AudioClip MP3Shoot;
    public float StartPositionX = 0f;
    Camera _camera;
    int jumps;
    private Animator playerAnimator;
    IEnumerator Start()
    {
        _camera = Camera.main;
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
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
        var animator = GunAnimator.GetComponent<Animator>();

        var gunId = PlayerData.GetEquippedId(GearSlot.Gun);
        if (gunId == PlayerData.empty) return;
        if (gunId == "13") // ak
        {
            animator.Play("GunAKReload");
        }
        else if (gunId == "14") // mp3
        {
            animator.Play("GunMP3Reload");
        }
        else // default pistol 1911
        {
            animator.Play("Gun1911Reload");

        }
    }
    void InputDoJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        if (!PlayerData.GetBoolById(PlayerData.isGodMode) && jumps-- <= 0) return;
        playerAnimator.Play("PlayerJump");
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxJumpSpeed, maxJumpSpeed);
        PlaySound(jump);
    }

    public void Shoot()
    {
        var gunId = PlayerData.GetEquippedId(GearSlot.Gun);
        if (gunId == PlayerData.empty) return;
        if (gunId == "13") // ak
        {
            PlaySound(AKShoot);
        }
        else if (gunId == "14") // mp3
        {
            PlaySound(MP3Shoot);
        }
        else // default pistol 1911
        {
            PlaySound(GameManager.Instance.gun.ShootSound);
        }
        Instantiate(Bullet, BulletPos.transform.position, BulletPos.transform.rotation);
        Debug.Log("shoot: bang");
        if (PlayerData.GetBoolById(PlayerData.IsScreenShake))
        {
            _camera.GetComponent<CameraShake2D>().Shake();
        }

        if (PlayerData.GetBoolById(PlayerData.IsEarRinging))
        {
            GunTinnitus.Instance.TriggerTinnitus();
        }

        if (!PlayerData.GetBoolById(PlayerData.isGodMode) && --GameManager.Instance.Ammo == 0)
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
        var isGodMode = PlayerData.GetBoolById(PlayerData.isGodMode);
        if (tag == Tags.Obstacle)
        {
            // do falling glass or metal pipe thing lol
            // maybe 3 options, metal, wood, glass for cars trees and barrels
            PlaySound(hurt);
            Destroy(collision.gameObject);
            if (!isGodMode) GameManager.Instance.Lives--;
        }
        else if (tag == Tags.Enemy || tag == Tags.FlyingEnemy)
        {
            // change later
            PlaySound(hurt);
            Destroy(collision.gameObject);
            if (!isGodMode) GameManager.Instance.Lives--;
        }
        else if (tag == Tags.Ground)
        {
            playerAnimator.Play("PlayerWalk");
            resetJumps();
        }
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
