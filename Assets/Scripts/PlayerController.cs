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

    [Header("Damage")]
    [SerializeField] private float knockbackSpeed;

    private bool _canDash;
    private bool _isDashing;
    private bool _dashInput;

    private bool _isDamaged;
    private Vector3 knockbackDirection;
    private bool _canBeDamaged;

    private bool _isDead;

    private float _currentSpeed;
    private float maxSpeed;

    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Animator _anim;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _canDash = true;
        maxSpeed = walkingSpeed;
        knockbackDirection = Vector3.zero;
        _isDead = false;
        _canBeDamaged = true;
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
        
        if(_dashInput && _canDash && !_isDamaged && !_isDead && !_anim.GetBool("isHitting") && !_anim.GetBool("isShooting"))
        {
            StartCoroutine(DashRoutine());
        }
    }
    public void TakeDamage(Vector3 knockbackDirection)
    {
        this.knockbackDirection = knockbackDirection;
        if (_canBeDamaged)
        {
            StartCoroutine(DamageRoutine());
        }
    }
    public void Die(Vector3 knockbackDirection)
    {
        this.knockbackDirection = knockbackDirection;
        StartCoroutine(DeadRoutine());
    }
    public IEnumerator DeadRoutine()
    {
        while (!_canBeDamaged)
        {
            yield return null;
        }
        _canBeDamaged = false;
        _isDamaged = true;
        _anim.SetBool("isDamaged", true);
        maxSpeed = knockbackSpeed;
        while (_anim.GetBool("isDamaged"))
        {
            yield return null;
        }
        _isDead = true;
        _isDamaged=false;
        maxSpeed = 0;
        _anim.SetBool("isDead", true);
    }
    private IEnumerator DamageRoutine()
    {
        _canBeDamaged = false;
        _isDamaged = true;
        _anim.SetBool("isDamaged", true);
        maxSpeed = knockbackSpeed;
        while (_anim.GetBool("isDamaged"))
        {
            yield return null;
        }
        maxSpeed = walkingSpeed;
        _isDamaged= false;
        _canBeDamaged=true;
    }
    private IEnumerator DashRoutine()
    {
        _canDash = false;
        _isDashing = true;
        _anim.SetBool("isDashing", true);
        maxSpeed = dashingSpeed;
        while (_anim.GetBool("isDashing"))
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
        if (_isDamaged)
        {
            _currentSpeed = knockbackSpeed;
        }
        else if (_isDashing)
        {
            _currentSpeed = dashingSpeed;
        }
        else if (_input != Vector3.zero && _currentSpeed < maxSpeed)
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
        if (_isDashing || _isDamaged ||_isDead)
        {
            return;
        }
        if (_input == Vector3.zero) { return; }

        Quaternion rotation = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (_isDamaged)
        {
            Vector3 knockback = transform.position + knockbackDirection * _currentSpeed * Time.deltaTime;
            _rigidBody.MovePosition(knockback);
            return;
        }
        else if (_isDashing)
        {
            Vector3 dashTo = transform.position + transform.forward *_currentSpeed*Time.deltaTime;
            _rigidBody.MovePosition(dashTo);
            return;
        }
        if (_anim.GetBool("isHitting") || _isDead)
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
        if (_anim.GetBool("isHitting") || _anim.GetBool("isShooting") || _isDashing || _isDamaged || _isDead)        {
            return;
        }
        Debug.Log("Hit");
        _anim.SetBool("isHitting", true);
    }
    private void Shoot(InputAction.CallbackContext obj)
    {
        if (_anim.GetBool("isHitting") || _anim.GetBool("isShooting") || _isDamaged || _isDashing || _isDead)
        {
            return;
        }
        Debug.Log("Shot");
        _anim.SetBool("isShooting", true);
    }
  
}
public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
