using UnityEngine;

public class AttacksPlayer : MonoBehaviour
{
    PlayerManager manager;

    Animator playerAnimator;

    public bool isAttacking;
    public int attackState;
    public bool returnAttack;
    public bool comboAttack;

    public bool runAttack;
    public bool attack1;
    public bool attack2;
    public bool attack3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<PlayerManager>();
        attackState = 0;
        returnAttack = false;
        isAttacking = false;
        comboAttack = false;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        runAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        AnimatorStateInfo playerInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (!returnAttack && manager.weapon && !manager.isRolling && !manager.isDodging)
        {
            isAttacking = true;
            if (manager.isRunning && manager.move.y >= 0.1f && attackState == 0)
            {
                runAttack = true;
                playerAnimator.SetBool("IsAttacking", isAttacking);
            }
            else if (!runAttack)
            {
                comboAttack = true;

                if (comboAttack)
                    attackState++;
                if (attackState == 1)
                {
                    playerAnimator.SetInteger("Attack", 1);
                    if (!playerInfo.IsName("ReverseAttack"))
                    {
                        attack1 = true;
                        attack2 = false;
                        attack3 = false;
                    }
                }
            }
        }

    }

    public void CheckAttackState()
    {
        comboAttack = false;

        AnimatorStateInfo playerInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerInfo.IsName("Attack1") && attackState == 1)
        {
            playerAnimator.SetInteger("Attack", 0);
            comboAttack = true;
            returnAttack = true;
            isAttacking = false;
            attackState = 0;
        }
        else if (playerInfo.IsName("Attack1") && attackState >= 2)
        {
            playerAnimator.SetInteger("Attack", 2);
            comboAttack = true;
            attack1 = false;
            attack2 = true;
            attack3 = false;
        }
        else if (playerInfo.IsName("Attack2") && attackState == 2)
        {
            playerAnimator.SetInteger("Attack", 0);
            comboAttack = true;
            attackState = 0;
            isAttacking = false;
        }
        else if (playerInfo.IsName("Attack2") && attackState >= 3)
        {
            playerAnimator.SetInteger("Attack", 3);
            comboAttack = true;
            attack1 = false;
            attack2 = false;
            attack3 = true;
        }
        else if (playerInfo.IsName("Attack3"))
        {
            playerAnimator.SetInteger("Attack", 0);
            comboAttack = true;
            attackState = 0;
            isAttacking = false;
            attack1 = false;
            attack2 = false;
            attack3 = false;
        }
        else if (playerInfo.IsName("ReverseAttack"))
        {
            comboAttack = true;
            playerAnimator.SetInteger("Attack", 0);
            attackState = 0;
            returnAttack = false;
        }
        else if (playerInfo.IsName("Attack4"))
        {
            playerAnimator.SetInteger("Attack", 0);
            comboAttack = true;
            attackState = 0;
            isAttacking = false;
            runAttack = false;
            manager.isRunning = false;
            playerAnimator.SetBool("IsAttacking", isAttacking);
        }
    }

}
