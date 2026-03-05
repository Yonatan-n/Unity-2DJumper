using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class Player : MonoBehaviour
{
    [SerializeField] GameObject visual;
    public int score;
    private bool isMidAir;
    InputAction jumpAction;
    InputAction shootAction;
    bool canShoot = true;
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
    [SerializeField] AudioClip DefaultShoot;

    public float StartPositionX = 0f;
    Camera _camera;
    int jumps;
    Animator playerAnimator;
    FlipVisual flipVisual;
    [SerializeField] GameObject doubleJumpFX;
    public bool doneLoading = false;
    IEnumerator Start()
    {
        _camera = Camera.main;
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerAnimator = visual.GetComponent<Animator>();
        flipVisual = visual.GetComponent<FlipVisual>();
        JumpBtn.onClick.AddListener(Jump);
        ShootBtn.onClick.AddListener(Shoot);
        PauseBtn.onClick.AddListener(PauseButtonHandler);
        // AudioManager need to be loaded before GameManager
        yield return new WaitUntil(() => AudioManager.Instance.IsInitialized);
        GameManager.Instance.updateAllCounters();
        resetJumps();
        doneLoading = true;
    }
    void OnDisable()
    {
        shootAction.performed -= InputDoShoot;
        jumpAction.performed -= InputDoJump;
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
        isMidAir = false;
    }

    private void SetupBindings()
    {
        jumpAction = new InputAction("Press", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.Enable();
        jumpAction.performed += InputDoJump;
        shootAction = new InputAction("Press", InputActionType.Button);
        shootAction.AddBinding("<Keyboard>/f");
        shootAction.Enable();
        shootAction.performed += InputDoShoot;

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
        string animName = gunId switch
        {
            "13" => "GunAKReload",     // ak
            "14" => "GunMP3Reload",    // mp3
            "11" => "GunVSPReload",    // vsp
            "10" => "GunGlonkReload", // glonk
            _ => "Gun1911Reload"       // default pistol 1911, if == "9"
        };
        animator.Play(animName);
    }
    void InputDoShoot(InputAction.CallbackContext context)
    {
        Shoot();
    }
    void InputDoJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        if (!PlayerData.GetBoolById(PlayerData.isGodMode) && jumps-- <= 0) return;
        if (isMidAir)
        {
            // Reset vertical velocity, so that the next jump will have exactly the same force as the first one
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
            flipVisual.TriggerFlip();
            Instantiate(doubleJumpFX, transform.position, Quaternion.identity);
        }
        isMidAir = true;
        playerAnimator.Play("PlayerJump");
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxJumpSpeed, maxJumpSpeed);
        PlaySound(jump);
        StatsTracker.Instance.OnJump();
        if (GameManager.Instance.maxJumps - jumps >= 4)
        {
            StatsTracker.Instance.OnQuadJump();
        }
    }

    public void Shoot()
    {
        if (!canShoot) return;
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
            PlaySound(DefaultShoot);
        }
        Instantiate(Bullet, BulletPos.transform.position, BulletPos.transform.rotation);
        Debug.Log("shoot: bang");
        StatsTracker.Instance.OnShoot();
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
        canShoot = false;
        ShootBtn.interactable = canShoot;
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
        canShoot = true;
        ShootBtn.interactable = canShoot;
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
            var obs = collision.gameObject.GetComponent<Obstacle>();
            if (obs.is_first)
                StatsTracker.Instance.OnPlayerDiedToFirst();
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
        yield return new WaitUntil(() => doneLoading);
        yield return movePlayerOutOfScreen(true);
    }

    public void HideControlButtons()
    {
        JumpBtn.GetComponentInChildren<TextMeshProUGUI>().text = "";
        ShootBtn.GetComponentInChildren<TextMeshProUGUI>().text = "";
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
