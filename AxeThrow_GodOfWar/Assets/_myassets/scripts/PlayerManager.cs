using System;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class PlayerManager : MonoBehaviour
{
    InputActions newActions;

    AxeBehaviour axeScript;

    AttacksPlayer attacksPlayer;

    CharacterController playerController;
    Animator playerAnimator;

    [SerializeField] Transform axe, curvePoint, targetPoint;
    Rigidbody axeRb;

    [SerializeField] Collider handCollider;

    Vector3 lastAxePoint;

    [SerializeField] Transform restraint;

    public Vector2 move;
    float rotate;

    public float slowSpeed;
    public float speed;
    public float rotationSpeed;

    public float throwForce;

    public bool isThrowing;
    public bool isAiming;
    public bool isReturning;
    public bool weapon;

    float time = 0.0f;
    float returnDuration = 1.0f;
    
    public bool isRunning;
    public bool isRolling;
    public bool isDodging;

    private void Awake()
    {
        newActions = new InputActions();
        axeScript = GameObject.FindGameObjectWithTag("Axe").GetComponent<AxeBehaviour>();
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        axeRb = GameObject.FindGameObjectWithTag("Axe").GetComponent<Rigidbody>();
        axe = GameObject.FindGameObjectWithTag("Axe").GetComponent <Transform>();
        attacksPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<AttacksPlayer>();
        restraint = GameObject.FindGameObjectWithTag("Restraint").GetComponent<Transform>();

        PlayerActions();
    }

    private void PlayerActions()
    {
        newActions.Player.Walk.performed += ctx => move.y = ctx.ReadValue<float>();

        newActions.Player.Walk.canceled += _ => move.y = 0f;

        newActions.Player.Rotate.performed += ctx => rotate = ctx.ReadValue<float>();

        newActions.Player.Rotate.canceled += _ => rotate = 0f;


        newActions.Player.Run.started += _ =>
        {
            isRunning = true;
        };

        newActions.Player.Run.canceled += _ =>
        {
            isRunning = false;
        };

        newActions.Player.Avoid.started += _ =>
        {
            if (move.y >= 0f && !isRolling)
                Roll();
            else if (move.y < 0f && !isDodging)
                Dodge();
        };

        newActions.Player.Equip.started += _ =>
        {
            if (weapon && !axeScript.isThrowed)
            {
                weapon = false;
                playerAnimator.SetTrigger("Disarm");
            }
            else if (!weapon)
            {
                weapon = true;
                playerAnimator.SetTrigger("Equip");
            }
        };

        newActions.Player.Attack.started += _ => attacksPlayer.Attack();
        
        newActions.Player.Throw.started += _ =>
        {
            if (weapon) 
            { 
                //playerAnimator.SetLayerWeight(1, 1);
                isAiming = true;
                playerAnimator.SetBool("Aim", isAiming);
            }
        };

        newActions.Player.Throw.canceled += _ =>
        {
            //playerAnimator.SetLayerWeight(1, 0);
            isAiming = false;
            playerAnimator.SetBool("Aim", isAiming);
            isThrowing = true;
            playerAnimator.SetTrigger("Throw");
            weapon = false;
               
        };

        newActions.Player.ReturnAxe.started += _ =>
        {
            if (!weapon && axeScript.isThrowed)
            {
                playerAnimator.SetTrigger("Return");
                ReturnAxe();
            }
        };

    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handCollider.GetComponent<Collider>().enabled = false;
        isThrowing = false;
        isReturning = false;
        weapon = true;
        rotationSpeed = 0.3f;
        throwForce = 40f;
    }


    // Update is called once per frame
    void Update()
    {
        CheckSpeed();
        if (isThrowing || isReturning || attacksPlayer.isAttacking || isDodging || isRolling)
            playerAnimator.SetFloat("Walk", 0f);
        else 
        {
            UpdateAnimations();
            MovePlayer();
        }

        if (isReturning)
        {
            time += Time.deltaTime/returnDuration;
            float t = Mathf.Clamp01(time);
            axeRb.position = getBQC(t, lastAxePoint, curvePoint.position, targetPoint.position);
        }
    }
    private void CheckSpeed()
    {
        if (move.y < 0.4)
            slowSpeed = 1.8f;
        else if (move.y >= 0.4)
            speed = 3.5f;
        if (isRunning)
            speed = 5.2f;
    }

    private void UpdateAnimations()
    {
        playerAnimator.SetFloat("Walk", move.y);
        playerAnimator.SetBool("IsRunning", isRunning);
    }

    private void MovePlayer()
    {
        if (Mathf.Abs(move.y) > 0.1f)
            playerController.SimpleMove(transform.forward * move.y * speed);


            transform.Rotate(Vector3.up * rotate * rotationSpeed * Time.deltaTime * 360f); 
    }
    private void Dodge()
    {
        isDodging = true;
        playerAnimator.SetTrigger("Dodge");
    }

    private void Roll()
    {
        isRolling = true;
        playerAnimator.SetTrigger("Roll");
    }

    public void CheckAvoidState()
    {
        if (isRolling)
            isRolling = false;
        if (isDodging)
            isDodging= false;
    }

    public void ThrowAxe()
    {
        axeScript.isThrowed = true;
        axeRb.isKinematic = false;
        axeRb.transform.parent = null;
        axeRb.transform.rotation = Quaternion.Euler(52.8f, 175.7f, 79.4f);
        axeRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    public void RestartBools()
    {
        if (isThrowing)
            isThrowing = false;
    }
    private void ReturnAxe()
    {
        isReturning = true;
        time = 0.0f;
        lastAxePoint = axe.transform.position;
        axeRb.isKinematic = false;
        handCollider.enabled = true;
    }

    Vector3 getBQC(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }


    public void RestartAxe()
    {
        isReturning = false;
        weapon = true;
        axeRb.transform.SetParent(targetPoint, false);
        axeRb.transform.localPosition = Vector3.zero;
        axeRb.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        axeRb.isKinematic = true;
        playerAnimator.SetTrigger("Catch");
        handCollider.enabled = false;
    }

    public void Equip()
    {
        axeRb.transform.parent = null;
        axeRb.transform.SetParent(targetPoint, false);
        axeRb.transform.localPosition = Vector3.zero;
        axeRb.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (!weapon)
            weapon = true;
    }

    public void Disarm()
    {
        axeRb.transform.parent = null;
        axeRb.transform.SetParent(restraint, false);
        axeRb.transform.localPosition = Vector3.zero;
        axeRb.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (weapon)
            weapon = false;
    }

    private void OnEnable()
    {
        newActions.Enable();
    }

    private void OnDisable()
    {
        newActions.Disable();
    }
}
