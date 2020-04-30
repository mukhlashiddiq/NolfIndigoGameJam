using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Controller Input
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode meleeAttack;
    public KeyCode rangedAttack;

    //Start() Variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    //Finite State Machine
    private enum State {idle, running, jumping, falling, hurt} 
    private State state = State.idle;

    //Inspector Variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    bool facingRight;

    //PLAYER 1 VARIABLES
    //Melee Attack Variables
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform meleeAttackPoint;
    [SerializeField] private float attackRange = 0.5f;
    private float attackDamage = 35f;
    private float attackRate = 2f;
    float nextAttackTime = 0f;

    //PLAYER 2 VARIABLES
    //Ranged Attack Variables
    public GameObject projectileToRight;
    public GameObject projectileToLeft;
    public Transform rangedAttackPoint;
    private float timeBtwShots;
    public float startTimeBtwShots;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (state != State.falling)
        {
            Movement();
        }

        

        AnimationState();
        anim.SetInteger("state", (int)state);//sets animation based on enumerator state

        MeleeAttack();
        RangedAttack();
    }

    private void Movement()
    {

        //Moving Left
        if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
            facingRight = false;
        }
        //Moving Right
        else if (Input.GetKey(right))
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            facingRight = true;
        }

        //Jumping
        if (Input.GetKeyDown(jump) && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if(state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }

        else if (state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 3f)
        {
            state = State.running;
        }

        else
        {
            state = State.idle;
        }
    }


    private void MeleeAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(meleeAttack))
            {
                //Play Attack Animation
                anim.SetTrigger("MeleeAttack");

                //AttackSpeed
                nextAttackTime = Time.time + 1f / attackRate;

                //Detect Enemies in Range of Attack
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, enemyLayers);

                //Damage Them
                foreach (Collider2D enemy in hitEnemies)
                {
                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected() //Circle Melee Attack Point
    {
        if (meleeAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(meleeAttackPoint.position, attackRange);
    }

    private void RangedAttack()
    {
        if(timeBtwShots <= 0)
        {
            if (Input.GetKeyDown(rangedAttack))
            {
                anim.SetTrigger("RangedAttack");
                if (facingRight)
                {
                    Instantiate(projectileToRight, rangedAttackPoint.position, Quaternion.identity);
                    timeBtwShots = startTimeBtwShots;
                }
                else
                {
                    Instantiate(projectileToLeft, rangedAttackPoint.position, Quaternion.identity);
                    timeBtwShots = startTimeBtwShots;
                }
            }
        }
        
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }
}