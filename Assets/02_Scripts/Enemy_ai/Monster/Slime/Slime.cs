using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    BoxCollider2D box;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;

    Coroutine coroutine;
    Coroutine thinkcoroutine;
    string animationState = "animationState";
    private float nextMove;
    private int direction;
    private int actmove;
    private bool attackflag;
    private bool turnflag;

    public float jumpPower;
    public float turnrange;

    public GameObject alert;

    enum States
    {
        walk = 1,
        jump = 2,
        spin = 3
    }
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        box = GetComponentInChildren<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Start()
    {
        coroutine = StartCoroutine(move());
        ThinkCall();
        attackflag = turnflag = false;
    }

    void Update()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * turnrange, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 255, 0));
    }

    public IEnumerator Think()
    {
        while (true)
        {
            float nextThinkTime = Random.Range(2f, 5f); // ���ð� 2�ʿ��� 5�� ���̷���
            if (turnflag == true)
            {
                yield return new WaitForSeconds(nextThinkTime);
            }
            nextMove = Random.Range(-1, 2); // -1~1 ���������� �� ������
            yield return new WaitForSeconds(nextThinkTime);
        }
    }

    public IEnumerator move()
    {
        while(true)
        {
            if (rigid.velocity.x > 0)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
            animator.SetInteger(animationState, (int)States.walk);
            Vector2 frontVec = new Vector2(rigid.position.x + nextMove * turnrange, rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 255, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2, LayerMask.GetMask("Platform"));
            if (rayHit.collider == null)
            {
                Turn();
            }
            rigid.velocity = new Vector2(nextMove * movespeed, rigid.velocity.y);
            yield return new WaitForSeconds(0f);
        }
    }

    public IEnumerator attack()
    {
        Debug.Log("attack");
        dis = Vector2.Distance(PlayerPos.transform.position, rigid.transform.position);
        if (PlayerPos.transform.position.x < rigid.transform.position.x)
        {
            direction = 1;
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
        else
        {
            direction = 2;
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }
        actmove = Random.Range(1, 3);
        switch (actmove)
        {
            case 1:
                Debug.Log("case1");
                if (direction == 1)
                {
                    rigid.AddForce(Vector2.up * jumpPower / 2, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.5f);
                    animator.SetInteger(animationState, (int)States.jump);
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    rigid.AddForce(Vector2.left * dis * 1.2f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.9f);
                    animator.SetInteger(animationState, (int)States.walk);
                }
                else
                {
                    rigid.AddForce(Vector2.up * jumpPower / 2, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.5f);
                    animator.SetInteger(animationState, (int)States.jump);
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    rigid.AddForce(Vector2.right * dis * 1.2f, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.9f);
                    animator.SetInteger(animationState, (int)States.walk);
                }
                break;
            case 2:
                Debug.Log("case2");
                if (direction == 1)
                {
                    rigid.AddForce(Vector2.up * jumpPower / 2, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.5f);
                    animator.SetInteger(animationState, (int)States.spin);
                    rigid.AddForce(Vector2.left * dis * 3, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(1f);
                    animator.SetInteger(animationState, (int)States.walk);
                }
                else
                {
                    rigid.AddForce(Vector2.up * jumpPower / 2, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(0.5f);
                    animator.SetInteger(animationState, (int)States.spin);
                    rigid.AddForce(Vector2.right * dis * 3, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(1f);
                    animator.SetInteger(animationState, (int)States.walk);
                }
                break;
        }
        yield break;
    }

    public IEnumerator KnockBack()
    {
        if (PlayerPos != null)
        {
            rigid.velocity = Vector2.zero;
            animator.SetInteger(animationState, (int)States.walk);
            rigid.AddForce(Vector2.up * knockbackdis, ForceMode2D.Impulse);
            if (PlayerPos.transform.position.x < rigid.transform.position.x)
            {
                rigid.AddForce(Vector2.right * knockbackdis, ForceMode2D.Impulse);
            }
            else
            {
                rigid.AddForce(Vector2.left * knockbackdis, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(1.2f);
            coroutine = StartCoroutine(move());
            ThinkCall();
            yield break;
        }
    }

    IEnumerator Alert()
    {
        alert.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        alert.gameObject.SetActive(false);
    }

    public override void TakeDamage(int AtDmg)
    {
        Hp = Hp - AtDmg;
        Debug.Log(Hp);
        StopAllCoroutines();
        StartCoroutine(KnockBack());
        if (Hp <= 0)
        {
            Die();
        }

    }

    void Turn()//��
    {
        nextMove = nextMove * (-1);
        turnflag = true;
        ThinkStop();
        ThinkCall();
    }

    void ThinkCall()
    {
        thinkcoroutine = StartCoroutine(Think());
    }

    void ThinkStop()
    {
        if (thinkcoroutine != null)
        {
            StopCoroutine(thinkcoroutine);
        }
    }

    void Die()
    {
        DropItem();
        Vector2 position = new Vector2(rigid.position.x, rigid.position.y + 0.2f);
        Instantiate(Corpse, position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void DropItem()
    {
        Debug.Log("ȣ��");
        for (int i = 0; i < dropcoincnt; i++)
        {
            float x = Random.Range(-1f, 1f); // x�� ��ġ ���� ����
            float y = Random.Range(0f, 1f); // y�� ��ġ ���� ����
            Vector2 position = new Vector2(transform.position.x + x, transform.position.y + y);
            Instantiate(Item, position, Quaternion.identity);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player.instance.Damaged(-collision_damage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerPos == null)
            {
                PlayerPos = collision.gameObject.transform;
            }
            if (attackflag == false)
            {
                StartCoroutine(Alert());
                attackflag = true;
                StopAllCoroutines();
                coroutine = StartCoroutine(attack());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (attackflag == true)
            {
                attackflag = false;
                StopAllCoroutines();
                ThinkCall();
                coroutine = StartCoroutine(move());
            }
        }
    }

}