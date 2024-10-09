using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;  
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        anim = GetComponent<Animator>(); 
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Updata Function 단발적인 입력할 때 사용
    private void Update() 
    {
        // Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) 
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
        // Stop Speed
        if(Input.GetButtonUp("Horizontal")) 
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }
        
        // Direction Sprite
        if(Input.GetButton("Horizontal")) 
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        
        //Animation
        if(Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        
        // Max Speed
        if(rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed*(-1)) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }
        
        // Landing Playform
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            // 땅에 닿았을 때
            if(rayHit.collider != null)
            {
                if(rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            //Damaged
            else 
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
    //Tag가 item일 때
	if (collision.gameObject.tag == "Item") 
    {
        // Point
        bool isBronze = collision.gameObject.name.Contains("Bronze");
        bool isSilver = collision.gameObject.name.Contains("Silver");
        bool isGold = collision.gameObject.name.Contains("Gold");
        if(isBronze){
            gameManager.stagePoint += 50;
        }
        else if(isSilver){
            gameManager.stagePoint += 100;
        }
        else if(isGold){
            gameManager.stagePoint += 300;
        }
		// Deactive Item
		collision.gameObject.SetActive(false);
	}
    else if(collision.gameObject.tag == "Finish")
    {
        // Next Stage
        gameManager.NextStage();
    }
}

    //몬스터의 죽음 관련 함수 호출
    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint +=100;
        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    //Damage를 입었을 때
    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();
        // Change Layer (Immortal Active)
        gameObject.layer = 11;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // 0.4f <- 투명도

        // Reaction Force
        int dirc = transform.position.x - targetPos.x >0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);
    }

    // Off Immortal Active
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // 죽은 이팩트 표현
    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
}
