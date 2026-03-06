using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System;
public class Player : MonoBehaviour
{
    [SerializeField] GameObject visual;
    public int score;
    private bool isMidAir;
    InputAction jumpAction;
    // --- Shooting ---
    InputAction shootAction;
    bool canShoot = true;
    bool isReloading = false;
    private bool isTriggerHeld = false;

    private const float SINGLE_FIRE_RATE = 0.2f;
    private const float BINARY_SHOT_DELAY = 0.09f;
    private const float BINARY_FIRE_RATE = 0.6f;
    private const float AUTO_FIRE_RATE = 0.1f;
    private const float BURST_FIRE_RATE = 0.07f;
    private const float BURST_DELAY = 0.2f;

    private RifleRecoil _recoil;

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
    [SerializeField] Animator GunAnimator;
    [SerializeField] AudioClip AKShoot;
    [SerializeField] AudioClip MP3Shoot;
    [SerializeField] AudioClip DefaultShoot;
    private GearItem Gun;
    public float StartPositionX = 0f;
    Camera _camera;
    int jumps;
    Animator playerAnimator;
    FlipVisual flipVisual;
    [SerializeField] GameObject doubleJumpFX;
    public bool doneLoading = false;

    private void OnShootPerformed(InputAction.CallbackContext ctx)
    {
        isTriggerHeld = true;
        Shoot();
    }
    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        isTriggerHeld = false;
    }

    private void OnShootPerformed(BaseEventData data)
    {
        isTriggerHeld = true;
        Shoot();
    }

    private void OnShootCanceled(BaseEventData data)
    {
        isTriggerHeld = false;
    }

    IEnumerator Start()
    {
        Gun = PlayerData.GetEquippedGun();
        _recoil = GetComponentInChildren<RifleRecoil>();
        _camera = Camera.main;
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerAnimator = visual.GetComponent<Animator>();
        flipVisual = visual.GetComponent<FlipVisual>();
        JumpBtn.onClick.AddListener(Jump);

        // for shoot, holding down option too
        var _gunTrigger = ShootBtn.gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener(OnShootPerformed);
        _gunTrigger.triggers.Add(pointerDown);
        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener(OnShootCanceled);
        _gunTrigger.triggers.Add(pointerUp);

        PauseBtn.onClick.AddListener(PauseButtonHandler);
        // AudioManager need to be loaded before GameManager
        yield return new WaitUntil(() => AudioManager.Instance.IsInitialized);
        GameManager.Instance.updateAllCounters();
        resetJumps();
        doneLoading = true;
    }
    void OnDisable()
    {
        jumpAction.performed -= InputDoJump;
        shootAction.performed -= OnShootPerformed;
        shootAction.canceled -= OnShootCanceled;
        shootAction.Disable();
        shootAction.Dispose();

        jumpAction.performed -= InputDoJump;
        jumpAction.Disable();
        jumpAction.Dispose();

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
        shootAction.performed += OnShootPerformed;
        shootAction.canceled += OnShootCanceled;
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

    void PlayAnimation(PlayerAction action)
    {
        if (Gun.Id == PlayerData.empty) return;
        var (_reload, _shoot) = GetGunAnimations();
        var _sfx = Gun.Id switch
        {
            GunsId.AK => AKShoot,
            GunsId.MP3 => MP3Shoot,
            _ => DefaultShoot
        };
        if (action == PlayerAction.Reload)
        {
            GunAnimator.Play(_reload);
        }
        else if (action == PlayerAction.Shoot)
        {
            PlaySound(_sfx);
            if (_shoot != null) GunAnimator.Play(_shoot);
        }
    }

    private (string, string) GetGunAnimations()
    {
        return Gun.Id switch
        {
            GunsId.AK => ("GunAKReload", null),
            GunsId.MP3 => ("GunMP3Reload", null),
            GunsId.VSP => ("GunVSPReload", "GunFlatRecoil"),
            GunsId.GLONK => ("GunGlonkReload", "GunFlatRecoilDouble"),
            _ => ("Gun1911Reload", "GunFlatRecoil")
        };
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
        if (Gun.fireMode == FireMode.Single)
        {
            StartCoroutine(SingleShot());
        }
        else if (Gun.fireMode == FireMode.BinaryTrigger)
        {
            StartCoroutine(BinaryTriggerShoot());
        }
        else if (Gun.fireMode == FireMode.FullAuto)
        {
            StartCoroutine(FullAutoShoot());
        }
        else if (Gun.fireMode == FireMode.Burst3Shots)
        {
            StartCoroutine(BurstShoot3());
        }
    }

    IEnumerator SingleShot()
    {
        canShoot = false;
        FireBullet();
        yield return new WaitForSeconds(SINGLE_FIRE_RATE);
        if (!isReloading) canShoot = true;
    }
    IEnumerator BinaryTriggerShoot()
    {
        canShoot = false;
        FireBullet();
        yield return new WaitForSeconds(BINARY_SHOT_DELAY);
        // only shoot the 2nd shoot if there is ammo in the magazine
        if (GameManager.Instance.Ammo > 0)
        {
            FireBullet();
            yield return new WaitForSeconds(BINARY_FIRE_RATE);
        }
        if (!isReloading) canShoot = true;
    }
    IEnumerator FullAutoShoot()
    {
        canShoot = false;
        FireBullet();
        _recoil.ApplyRecoil();
        yield return new WaitForSeconds(AUTO_FIRE_RATE);

        while (isTriggerHeld && GameManager.Instance.Ammo > 0)
        {
            FireBullet();
            _recoil.ApplyRecoil();
            yield return new WaitForSeconds(AUTO_FIRE_RATE);
        }
        _recoil.ResetRecoil();
        if (!isReloading) canShoot = true;
    }

    private IEnumerator BurstShoot3()
    {
        canShoot = false;
        while (isTriggerHeld && GameManager.Instance.Ammo > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                // stop if no ammo
                if (GameManager.Instance.ammo <= 0) break;
                FireBullet();
                _recoil.ApplyRecoil();
                yield return new WaitForSeconds(BURST_FIRE_RATE);

            }
            _recoil.ResetRecoil();
            yield return new WaitForSeconds(BURST_DELAY);
        }
        if (!isReloading) canShoot = true;
    }

    private void FireBullet()
    {
        PlayAnimation(PlayerAction.Shoot);
        Instantiate(Bullet, BulletPos.transform.position, BulletPos.transform.rotation);
        Debug.Log("shoot: bang");
        StatsTracker.Instance.OnShoot();

        if (PlayerData.GetBoolById(PlayerData.IsScreenShake))
            _camera.GetComponent<CameraShake2D>().Shake();

        if (PlayerData.GetBoolById(PlayerData.IsEarRinging))
            GunTinnitus.Instance.TriggerTinnitus();

        if (!PlayerData.GetBoolById(PlayerData.isGodMode) && --GameManager.Instance.Ammo == 0)
            StartCoroutine(PerformReload());
    }

    IEnumerator PerformReload()
    {
        GameManager.Instance.GreyOutInAmmo(true);
        canShoot = false;
        isReloading = true;
        ShootBtn.interactable = false;
        // wait for the shoot to finish
        var waitForShotToFade = 0.8f;
        yield return new WaitForSeconds(waitForShotToFade);
        // start the animation
        PlayAnimation(PlayerAction.Reload);

        yield return new WaitForSeconds(Gun.ReloadTime - waitForShotToFade);
        GameManager.Instance.GreyOutInAmmo(false);
        canShoot = true;
        isReloading = false;
        ShootBtn.interactable = true;
        // Code to execute after the delay
        Debug.Log("Function executed after " + Gun.ReloadTime + " seconds!");
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        var tag = collision.gameObject.tag;
        var isGodMode = PlayerData.GetBoolById(PlayerData.isGodMode);
        if (tag == Tags.Obstacle)
        {
            var obs = collision.gameObject.GetComponent<Obstacle>();
            if (!isGodMode && GameManager.Instance.Lives == 1 && obs.is_first)
                StatsTracker.Instance.OnPlayerDiedToFirst();
            StatsTracker.Instance.OnPlayerCollision(obs.type);
            PlaySound(hurt);
            Destroy(collision.gameObject);
            if (!isGodMode) GameManager.Instance.Lives--;
        }
        else if (tag == Tags.Enemy || tag == Tags.FlyingEnemy)
        {
            ObstacleType? obsType = tag switch
            {
                Tags.FlyingEnemy => ObstacleType.FlyingEnemy,
                Tags.Enemy => ObstacleType.WalkingEnemy,
                _ => null,
            };

            if (obsType.HasValue) StatsTracker.Instance.OnPlayerCollision(obsType.Value);
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

enum PlayerAction
{
    Reload, Shoot
}
