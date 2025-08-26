using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputActions newActions;
    CharacterController playerController;
    Animator playerAnimator;

    Vector2 move;
    float rotate;

    public float slowSpeed;
    public float speed;
    public float rotationSpeed;

    public bool isThrowing;
    public bool isAiming;
    public bool isReturning;


    private void Awake()
    {
        newActions = new InputActions();

        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();

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
            if (!isThrowing || !isReturning) 
            { 
                isAiming = true;
                playerAnimator.SetBool("Aim", isAiming);
            }
        };

        newActions.Player.Throw.canceled += _ =>
        {
            if (!isThrowing || !isReturning)
            {
                isAiming = false;
                playerAnimator.SetBool("Aim", isAiming);
                isThrowing = true;
                playerAnimator.SetTrigger("Throw");
                ThrowAxe();
            }        
        };

        newActions.Player.ReturnAxe.started += _ =>
        {
            isReturning = true;
            playerAnimator.SetTrigger("Return");
            ReturnAxe();
        };
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isThrowing = false;
        isReturning = false;
        CheckSpeed();
        rotationSpeed = 0.3f;
    }

    private void CheckSpeed()
    {
        if (move.y < 0.4)
            slowSpeed = 1.8f;
        else if (move.y >= 0.4)
            speed = 2.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isThrowing || !isReturning)
        {
            UpdateAnimations();
            MovePlayer();
        }
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

    private void ThrowAxe()
    {
    }

    private void ReturnAxe()
    {
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
