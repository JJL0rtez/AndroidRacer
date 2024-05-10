using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControlls : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _carRB;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _slowPerTick = 0.5f;
    [SerializeField] private float _maxSpeed = 15f;
    //[SerializeField] private float _rotationSpeed = 300f;

    private float _moveInput;
    // players total speed, used to set a max speed and smooth out forward/backword movement
    private float xspeed;
    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {

       if(_moveInput != 0)
       {

         xspeed = xspeed + (_moveInput * _speed * Time.fixedDeltaTime);
         if(xspeed > _maxSpeed){
            xspeed = _maxSpeed;
         }
         else if (xspeed < -_maxSpeed)
         {
            xspeed = -_maxSpeed;
         }
        _carRB.velocity = new Vector3(xspeed,  _carRB.velocity.y);
       }
       else {
        if(xspeed == 0)
        {
            // Will be used to add idle detection later
        }
        else if(xspeed > 0)
        {
            if(xspeed - _slowPerTick < 0){
                xspeed = 0;
            }
            else 
            {
                xspeed -= _slowPerTick;
            }
        }
        else 
        {
            if(xspeed + _slowPerTick > 0){
                xspeed = 0;
            }
            else 
            {
                xspeed += _slowPerTick;
            }
        _carRB.velocity = new Vector3(xspeed,  _carRB.velocity.y);
       }
    }
}
}
