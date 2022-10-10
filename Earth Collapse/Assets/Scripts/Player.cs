using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _runSpeed = 20f;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravityScale = -9.81f;
    [SerializeField] private float _lerpGravityValue = 0.01f;
    [SerializeField] private float _lerpValue = 0.001f;
    [SerializeField] private CharacterController _characterController;

    [Header("Input")]
    [SerializeField] private KeyCode _crouchButton;
    [SerializeField] private KeyCode _sprintButton = KeyCode.LeftShift;
    [SerializeField] private KeyCode _jumpButton = KeyCode.Space;
    private KeyCode[] _wasd = new KeyCode[4];

    [Header("Ground Check")]
    [SerializeField] private float _checkRadius;
    [SerializeField] private Transform _groundCheckCenter;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _isCrouching;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _controlIsLocked = false;
    [SerializeField] private bool _gravitationIsLocked = false;


    private Vector3 _velocity;
    private float _cachedSpeed;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        SoftFall();
        CheckGround();
        Crouch();
        Jump();
        Sprint();
        Movement();
        Gravitation();
    }

    private void Init()
    {
        _wasd[0] = KeyCode.W;
        _wasd[1] = KeyCode.A;
        _wasd[2] = KeyCode.S;
        _wasd[3] = KeyCode.D;

        _cachedSpeed = _speed;
        _characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Movement()
    {
        if (_controlIsLocked == false)
        {
            float vertical = Input.GetAxis("Vertical") * _speed;
            float horizontal = Input.GetAxis("Horizontal") * _speed;

            Vector3 direction = transform.forward * Mathf.Lerp(0f, vertical, Time.deltaTime) + transform.right * Mathf.Lerp(0f, horizontal, Time.deltaTime);
            _characterController.Move(direction);
        }
    }

    private void Sprint()
    {
        for (int i = 0; i < _wasd.Length; i++)
        {
            if (Input.GetKey(_sprintButton) && Input.GetKey(_wasd[i]))
            {
                _speed = Mathf.Lerp(_speed, _runSpeed, _lerpValue);
            }
            else if (!Input.GetKey(_sprintButton))
            {
                _speed = Mathf.Lerp(_speed, _cachedSpeed, _lerpValue);
            }
        }
    }

    private void Crouch()
    {
        if (_isCrouching)
        {
            _characterController.height = Mathf.Lerp(_characterController.height, 0f, Time.deltaTime);
        }
        else
        {
            _characterController.height = Mathf.Lerp(_characterController.height, 4f, Time.deltaTime);
        }

        if (Input.GetKeyDown(_crouchButton))
        {
            if (_isCrouching)
            {
                _isCrouching = false;
            }
            else
            {
                _isCrouching = true;
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(_jumpButton) && _gravitationIsLocked == false)
        {
            if (_isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpForce * -2 * _gravityScale);
            }
        }
    }

    private void Gravitation()
    {
        if (_gravitationIsLocked == false)
        {
            _velocity.y = Mathf.Lerp(_velocity.y, _gravityScale, _lerpGravityValue);
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }

    private void SoftFall()
    {
        if (!_isGrounded && _velocity.y < 0f)
        {
            _velocity.y = Mathf.Lerp(_velocity.y, _gravityScale, _lerpGravityValue);
        }
    }

    private void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(_groundCheckCenter.position, _checkRadius, _groundLayer);

        if (_isGrounded)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    public bool IsGroundedNow()
    {
        return _isGrounded;
    }

    public void LockControl()
    {
        _controlIsLocked = true;
    }

    public void UnlockControl()
    {
        _controlIsLocked = false;
    }

    public void LockGravitation()
    {
        _gravitationIsLocked = true;
    }

    public void UnlockGravitation()
    {
        _gravitationIsLocked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckCenter.position, _checkRadius);
    }
}
