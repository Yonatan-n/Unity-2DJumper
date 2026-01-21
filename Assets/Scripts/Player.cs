using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
public class Player : MonoBehaviour
{

    public int score;
    InputAction jumpAction;
    Rigidbody2D rb;
    [SerializeField] float jumpForce = 20f;
    bool isJumpStarted = false;



    void Start()
    {
        SetupBindings();
        rb = GetComponent<Rigidbody2D>();
    }

    private void SetupBindings()
    {
        jumpAction = new InputAction("Press", InputActionType.Button);
        jumpAction.AddBinding("<Touchscreen>/primaryTouch/press");
        jumpAction.AddBinding("<Mouse>/leftButton");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.Enable();
        jumpAction.performed += Dojump;

    }

    void Dojump(InputAction.CallbackContext context)
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }


    void Update()
    {

    }
}
