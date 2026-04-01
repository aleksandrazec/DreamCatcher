using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float walkingSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float accelarationFactor;
    [SerializeField] private float decelarationFactor;

    [Header("Dash")]
    [SerializeField] private float dashingCooldown;
    [SerializeField] private float dashingSpeed;

    private bool _canDash;
    private bool _isDashing;

    private bool _dashInput;

    private float _currentSpeed;
    private float maxSpeed;

    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;

    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Animator _anim;


    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _canDash = true;
        maxSpeed = walkingSpeed;
    }
    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Hit.started += Hit;
        _playerInputActions.Player.Shoot.started += Shoot;
    }
    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
        _playerInputActions.Player.Hit.started -= Hit;
        _playerInputActions.Player.Shoot.started -= Shoot;

    }
    private void Update()
    {
        GatherInput();

        Look();

        CalculateSpeed();

        Move();
        
        if(_dashInput && _canDash && !_anim.GetBool("_isHitting") && !_anim.GetBool("_isShooting"))
        {
            StartCoroutine(DashRoutine());
        }
    }
    private IEnumerator DashRoutine()
    {
        _canDash = false;
        _isDashing = true;
        _anim.SetBool("_isDashing", true);
        maxSpeed = dashingSpeed;
        while (_anim.GetBool("_isDashing"))
        {
            yield return null;
        }
        maxSpeed = walkingSpeed;
        _isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        _canDash = true;
    }

    private void CalculateSpeed()
    {
        if (_isDashing)
        {
            _currentSpeed = dashingSpeed;
        }
        else if(_input != Vector3.zero && _currentSpeed< maxSpeed)
        {
            _currentSpeed += accelarationFactor * Time.deltaTime;
        }
        else if (_input == Vector3.zero && _currentSpeed > 0)
        {
            _currentSpeed -= decelarationFactor * Time.deltaTime;
        }
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, maxSpeed);
        _anim.SetFloat("_currentSpeed", _currentSpeed);
    }

    private void Look()
    {
        if (_isDashing)
        {
            return;
        }
        if (_input == Vector3.zero) { return; }

        Quaternion rotation = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (_isDashing)
        {
            Vector3 dashTo = transform.position + transform.forward *_currentSpeed*Time.deltaTime;
            _rigidBody.MovePosition(dashTo);
            return;
        }
        if (_anim.GetBool("_isHitting"))
        {
            return;
        }

        _rigidBody.MovePosition(transform.position+_input.ToIso()*_input.normalized.magnitude*_currentSpeed*Time.deltaTime);
    }

    private void GatherInput()
    {
        Vector2 input = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
        _dashInput = _playerInputActions.Player.Dash.IsPressed();
    }
    private void Hit(InputAction.CallbackContext obj)
    {
        if (_anim.GetBool("_isHitting") || _anim.GetBool("_isShooting"))
        {
            return;
        }
        Debug.Log("Hit");
        _anim.SetBool("_isHitting", true);
    }
    private void Shoot(InputAction.CallbackContext obj)
    {
        if (_anim.GetBool("_isHitting") || _anim.GetBool("_isShooting"))
        {
            return;
        }
        Debug.Log("Shot");
        _anim.SetBool("_isShooting", true);
    }
  
}
public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
