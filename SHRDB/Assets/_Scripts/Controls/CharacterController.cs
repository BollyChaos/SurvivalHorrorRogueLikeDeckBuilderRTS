using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterController : MonoBehaviour,ICharacterController
    {
        // Start is called before the first frame update
        [Header("Locomotion")]
        [SerializeField] protected float MaxSpeed = 8;
        [SerializeField] protected float Acceleration = 200;
        [SerializeField] protected AnimationCurve AccelerationFactorFromDot;
        [SerializeField] protected float MaxAccelForce = 150;
        [SerializeField] protected AnimationCurve MaxAccelerationForceFactorFromDot;
        [SerializeField] protected Vector3 ForceScale = new Vector3(1, 0, 1);

       
        ///PRIVATE VARS
        protected Transform cameraTransform;
        protected Vector3 m_GoalVel= Vector3.zero;    
        protected Vector3 _input=Vector3.zero;




        //public Vector3 MoveDirection { No funciona si la camara esta en cenital, forward es atravesar el suelo
        //    get { return (new Vector3(cameraTransform.forward.x, 0,cameraTransform.forward.z) * _input.z + new Vector3(cameraTransform.right.x,0, cameraTransform.right.z) * _input.x).normalized; }
        //}
        public Vector3 MoveDirection
        {//la x es izquierda y derecha, la z es arriba y abajo, no creo que la camara rote por lo que no será necesario tener en cuenta su referencia
            get {return  new Vector3(_input.x,0,_input.z).normalized; }
        }

        public bool IsMoving
        {
            get { return _input.magnitude > 0; }
        }
      
        public Vector2 inputDir { get { return new Vector2(_input.x,_input.z); } }
        protected Rigidbody _rb;
        protected void Start()
        {
            cameraTransform=Camera.main.transform;
            _rb = GetComponent<Rigidbody>();
            LookForInput();
           
        }
        public void LookForInput()
        {
            PlayerInput input = InputManager.Instance.Input;
            if (input != null)
            {
                
                input.actions["Move"].started += onMove;
                input.actions["Move"].performed += onMove;
                input.actions["Move"].canceled += onMove;

            }
        }

      

        public void onMove(InputAction.CallbackContext ctx)
        {
            _input=new Vector3(ctx.ReadValue<Vector2>().x,0, ctx.ReadValue<Vector2>().y);
        }

      
        public void FixedUpdate()
        {
            MovePlayer();
        }

        

       

        protected virtual void MovePlayer()
        {
            Vector3 moveDirection = inputDir; 
            Vector3 unitVel = moveDirection.normalized;

            //Calculate new goal vel...
            float velDot = Vector3.Dot(m_GoalVel, unitVel);
            float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = moveDirection * MaxSpeed;
            m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accel * Time.deltaTime);
            //Actual force...

            Vector3 neededAccel = (m_GoalVel - _rb.linearVelocity) / Time.deltaTime;
            float maxAccel = MaxAccelForce + MaxAccelerationForceFactorFromDot.Evaluate(velDot);
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, ForceScale));

        }
    }
}
