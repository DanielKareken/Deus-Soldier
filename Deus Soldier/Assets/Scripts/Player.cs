using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    //[SerializeField] RuntimeData _runtimeData;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Health _healthRef;
    

    [Header("Player Movement")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpForce = 10f;
    //[SerializeField] private float _gravity = 20f;
    [SerializeField] private LayerMask _groundMask;

    Vector3 _playerMovementInput;
    TraitManager _traitManager;
    private int _jumpCount; //number of jumps left before landing;
    private bool _jumpKeyDown = false;
    private bool _isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        _healthRef  = GetComponent<Health>();
        _traitManager = GetComponent<TraitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Aim();        
    }

    void FixedUpdate()
    {
        //conditions
        CheckIsGrounded();

        //actions
        Movement();
        Jump();
    }

    //allows player to rotate camera
    void Aim()
    {
        /*float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");        

        //allows mouse to pan camera
        transform.Rotate(Vector3.up, mouseX * _mouseSensitivity);

        //allows mouse to tilt cam up and down, but restricts to 180 degrees on y axis using clamp
        _currentTilt -= mouseY * _mouseSensitivity;
        _currentTilt = Mathf.Clamp(_currentTilt, -90, 90);

        _cam.transform.localEulerAngles = new Vector3(_currentTilt, 0, 0);           
        //_cam.transform.Rotate(Vector3.right, mouseY * _mouseSensitivity);*/
    }

    //handles player movement
    void Movement()
    {
        _playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveVec = transform.TransformDirection(_playerMovementInput) * _moveSpeed;

        _rb.velocity = new Vector3(moveVec.x, _rb.velocity.y, moveVec.z);

        if (_rb.velocity != Vector3.zero)
            return;
        
        //TODO: set weapon anim to Walking

            
    }
    
    //check if player's feet are touching ground
    void CheckIsGrounded()
    {
        if (Physics.CheckSphere(_feetTransform.position, 0.05f, _groundMask) && !_isGrounded)
        {
            _isGrounded = true;
            _jumpCount = _traitManager.GetTrait("Jump").traitLevel; //refresh jumps
        }          
        else if (!Physics.CheckSphere(_feetTransform.position, 0.05f, _groundMask) && _isGrounded)
        {
            _isGrounded = false;           
        }          
    }
    
    //allows player to jump
    void Jump()
    {
        //jump key pressed and player has jump ability
        if (Input.GetAxis("Jump") > 0 && !_jumpKeyDown)
        {
            //player has jump ability?
            if (_traitManager.GetTrait("Jump").isAquired)
            {
                //jumping while grounded
                if (_isGrounded)
                {
                    _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                    _jumpCount--;
                }
                //jumping while midair
                else if (!_isGrounded && _jumpCount > 0)
                {
                    _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                    _jumpCount--;
                }
            }

            _jumpKeyDown = true;
        }
        else if (!(Input.GetAxis("Jump") > 0) && _jumpKeyDown)
        {
            _jumpKeyDown = false;
        }
    }
}
