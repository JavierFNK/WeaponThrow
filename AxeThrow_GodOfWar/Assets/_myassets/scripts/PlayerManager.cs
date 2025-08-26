using System;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class PlayerManager : MonoBehaviour
{
    InputActions newActions;

    AxeBehaviour axeScript;

    CharacterController playerController;
    Animator playerAnimator;

    [SerializeField] Transform axe, curvePoint, targetPoint;
    Rigidbody axeRb;

    Vector3 lastAxePoint;

    Vector2 move;
    float rotate;

    public float slowSpeed;
    public float speed;
    public float rotationSpeed;

    public float throwForce;

    public bool isThrowing;
    public bool isAiming;
    public bool isReturning;
    bool weapon;

    float time = 0.0f;
    float returnDuration = 1.0f;


    private void Awake()
    {
        newActions = new InputActions();
        axeScript = GameObject.FindGameObjectWithTag("Axe").GetComponent<AxeBehaviour>();
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        axeRb = GameObject.FindGameObjectWithTag("Axe").GetComponent<Rigidbody>();
        axe = GameObject.FindGameObjectWithTag("Axe").GetComponent <Transform>();

        PlayerActions();
    }

    private void PlayerActions()
    {
        newActions.Player.Walk.performed += ctx => move.y = ctx.ReadValue<float>();

        newActions.Player.Walk.canceled += _ => move.y = 0f;

        newActions.Player.Rotate.performed += ctx => rotate = ctx.ReadValue<float>();

        newActions.Player.Rotate.canceled += _ => rotate = 0f;

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
            if (!weapon)
            {
                playerAnimator.SetTrigger("Return");
                ReturnAxe();
            }
        };
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (isThrowing || isReturning)
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
        else if (time > 1.0f)
        {
            RestartAxe();
        }
    }
    private void CheckSpeed()
    {
        if (move.y < 0.4)
            slowSpeed = 1.8f;
        else if (move.y >= 0.4)
            speed = 2.4f;
    }

    private void UpdateAnimations()
    {
        playerAnimator.SetFloat("Walk", move.y);
    }

    private void MovePlayer()
    {
        if (move.y > 0.1f)
            playerController.SimpleMove(transform.forward * move.y * speed);

        transform.Rotate(Vector3.up * rotate * rotationSpeed *  Time.deltaTime * 360f); 
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
    }

    Vector3 getBQC(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }


    private void RestartAxe()
    {
        isReturning = false;
        axeRb.transform.SetParent(targetPoint, false);
        axeRb.transform.localPosition = Vector3.zero;
        axeRb.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        axeRb.isKinematic = true;
        playerAnimator.SetTrigger("Catch");
        weapon = true;
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
