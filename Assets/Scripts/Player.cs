using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
public class Player : MonoBehaviour
{

    public int score;
    InputAction jumpAction;
    Rigidbody2D rb;
    AudioSource audioSource;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip reload;
    [SerializeField] AudioClip hurt;
    [SerializeField] Button JumpBtn;
    [SerializeField] Button ShootBtn;


    void Start()
    {
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        JumpBtn.onClick.AddListener(Jump);
        ShootBtn.onClick.AddListener(Shoot);
    }

    private void SetupBindings()
    {
        jumpAction = new InputAction("Press", InputActionType.Button);
        // jumpAction.AddBinding("<Mouse>/leftButton");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.Enable();
        jumpAction.performed += InputDoJump;

    }
    void PlaySound(AudioClip clip)
    {
        // audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }
    void InputDoJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        PlaySound(jump);
    }

    public void Shoot()
    {
        Debug.Log("shoot: bang");
        PlaySound(shoot);

    }

    void Update()
    {

    }
}
