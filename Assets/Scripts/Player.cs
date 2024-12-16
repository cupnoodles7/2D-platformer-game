using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Player : MonoBehaviour
{   
    public Text coinText;
    public int maxHealth = 3;
    public Animator animator; 
    public Rigidbody2D rb;
    public Text health;
    public Text winText;

    public int currentCoin = 0;

    public float jumpHeight = 5f;
    public bool isGround = true;

    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer; 

    // Start is called before the first frame update
    void Start()
    {
         winText.gameObject.SetActive(false); 
    }

    // Update is called once per frame
    void Update()

    {

        if (maxHealth <= 0)
        {
           
            Die();
        }

        coinText.text = currentCoin.ToString();

        health.text = maxHealth.ToString();

       movement = Input.GetAxis("Horizontal"); 

       if (movement < 0f && facingRight)
       {
        transform.eulerAngles = new Vector3(0f, -180f, 0f);
        facingRight = false;
       }
       else if (movement > 0f && facingRight == false)
       {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        facingRight = true;
       }
       if (Input.GetKey(KeyCode.Space) && isGround)
       {
        Jump();
        isGround = false;
        animator.SetBool("Jump", true);
       }

       if (Mathf.Abs(movement) > 0.1f)
       {
        animator.SetFloat("Run", 1f);
       }
       else if (movement < .1f)
       {
        animator.SetFloat("Run", 0f);
       }
       if (Input.GetMouseButtonDown(0))
       {
        animator.SetTrigger("Attack");
       }

    }

    private void FixedUpdate() 
    {
        transform.position += new Vector3(movement,0f,0f) * Time.fixedDeltaTime * moveSpeed;

    }

    void Jump()
    {

        rb.AddForce(new Vector2(0f, jumpHeight ), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("Jump", false);

        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer); 
        if (collInfo)
        {
            if (collInfo.gameObject.GetComponent<PatrolEnemy>() != null)
            {
                collInfo.gameObject.GetComponent<PatrolEnemy>().TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) {return;}
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius );
    }

    public void TakeDamage(int damage)
    {   
        if (maxHealth <= 0 ) {return;}
        maxHealth -= damage;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Coin")
        {
            currentCoin += 1;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");        
            Destroy(other.gameObject, 1f);
        }

        if (other.gameObject.tag == "VictoryPoint")
        {
            Debug.Log("Victory");
            WinGame(); 


    }
    }
    

    void Die()
    {
        Debug.Log("player died.");
        FindObjectOfType<GameManager>().isGameActive = false;
        Destroy(this.gameObject);
    }

    void WinGame()
    {
        Debug.Log("You Win!");
        winText.gameObject.SetActive(true);  // Show the "You Win" text
        Time.timeScale = 0f;  // Freeze the game
    }
    
}
