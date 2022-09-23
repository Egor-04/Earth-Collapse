using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityValue = -8.91f;
    [SerializeField] private float _checkRadius = 2f;
    [SerializeField] private Transform _groundCheckCenter;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private KeyCode _crouchButton;
    [SerializeField] private KeyCode _jumpButton;

    private Vector3 _velocity;
    private const float _minSpeed = 0f;
    private const float _normalHeight = 4f;
    private const float _crouchHeight = 2f;
    private bool _isCrouching;
    private bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        SoftFall();
        CheckGround();
        Jump();
        Crouch();
        Movement();
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(_crouchButton) && _isCrouching == false)
        {
            _characterController.height = _crouchHeight;
        }
        else
        {
            _characterController.height = _normalHeight;
        }
    }

    private void Movement()
    {
        float vertical = Input.GetAxis("Vertical") * _speed;
        float horizontal = Input.GetAxis("Horizontal") * _speed;
        _velocity = transform.forward * Mathf.Lerp(0f, vertical, Time.deltaTime) + transform.right * Mathf.Lerp(0f, horizontal, Time.deltaTime);

        _characterController.Move(_velocity);
    }

    private void Jump()
    {
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }

        if (Input.GetKeyDown(_jumpButton))
        {
            _velocity.y += Mathf.Sqrt(_jumpForce * -3.0f * _gravityValue);
        }

        _velocity.y += _gravityValue * Time.deltaTime;
        _characterController.Move(_velocity);
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

    private void SoftFall()
    {
        if (!_isGrounded && _velocity.y < 2f)
        {
            _velocity.y = Mathf.Lerp(_velocity.y, _gravityValue, Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckCenter.position, _checkRadius);
    }
}
