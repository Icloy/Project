using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region component
    private float horizontal;
    public float maxSpeed;
    public float jumpingPower = 25f;
    private bool doubleJump;
    private float doubleJumpingPower = 20f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool isWallSliding;
    private float wallSlidingSpeed = 3f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private Vector2 wallJumpingPower = new Vector2(16f, 32f);

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 100f;
    private float dashingTime = 0.1f;
    private float dashingCooldown = 1f;

    SpriteRenderer spriteRenderer;
    Animator anim;



    private float Laddervertical;
    private float Ladderspeed = 8f;
    private bool isLadder;
    private bool isClimbing;


    [HideInInspector]  [SerializeField] private Rigidbody2D rigid;
    [HideInInspector]  [SerializeField] private Transform groundCheck;
    [HideInInspector]  [SerializeField] private LayerMask groundLayer;
    [HideInInspector]  [SerializeField] private Transform wallCheck;
    [HideInInspector]  [SerializeField] private LayerMask wallLayer;
    [HideInInspector]  [SerializeField] private TrailRenderer tr;

    private float curTime;
    public float coolTime = 0.5f;
    [HideInInspector]  public Transform pos;

    bool zoomIn;

    [HideInInspector]
    [Range(2, 10)]
    public float zoomSize;

    [HideInInspector]
    [Range(0.01f, 0.1f)]
    public float zoomSpeed;

    //�� ��Ÿ��
    float hcurT;
    float hgoalT = 1.5f;

    // �÷��̾� �������ͽ�
    public int AtDmg; //���� ������
    public int maxHp; //�ִ� ü�� 
    public int curHp; //���� ü��
    [HideInInspector]  public GameObject Stat;
    private int i = 0;
    [HideInInspector]  public GameObject AEffect;
    [HideInInspector]  public GameObject AEffect_Up;
    [HideInInspector]  public GameObject AEffect_Down;
    Camera cam;
    public bool fadeInOut;
    public bool SmoothMoving;

    [HideInInspector]  public GameObject HealEffect;
    [HideInInspector]  public AudioClip AttackSound;
    [HideInInspector]  public AudioClip HealSound;
    [HideInInspector]  public AudioClip DashSound;

    //�����
    private AudioSource AudioPlayer; //����� �ҽ� ������Ʈ

    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public bool IsWallJumping { get => isWallJumping; set => isWallJumping = value; }

    GameManager gameManager;
    Hp hp;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        AudioPlayer = GetComponent<AudioSource>();
        gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        hp = GameObject.Find("Hp").GetComponent<Hp>();
        cam = Camera.main;
    }
    // Update is called once per frame
    #endregion


    private void Start()
    {
        hp.udtHp(curHp, maxHp);
    }

    void Update()
    {

        #region Move

        horizontal = Input.GetAxisRaw("Horizontal");

        if (!Input.anyKeyDown)
        {
            anim.SetBool("Idle", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)&&!gameManager.isPanelOpen)
        {
            transform.localScale = new Vector3(-1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)&& !gameManager.isPanelOpen)
        {
            transform.localScale = new Vector3(1f, 1f);
        }

        if (rigid.velocity.normalized.x == 0)
        {
            anim.SetBool("Run", false);
        }
        else
            anim.SetBool("Run", true);

        #endregion

        #region Jump

        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }
        if (Input.GetButtonDown("Jump") && !gameManager.isShopOpen)
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, doubleJump ? doubleJumpingPower : jumpingPower);
                doubleJump = !doubleJump;
                if (jumpBufferCounter > 0f)
                {
                    jumpBufferCounter = 0f;
                }
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        if (Input.GetButtonUp("Jump") && rigid.velocity.y > 0f)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (!IsGrounded() && !IsWalled() && isLadder == false)
        {
            coyoteTimeCounter -= Time.deltaTime;
            anim.SetBool("Jump", true);
        }
        else
        {
            coyoteTimeCounter = coyoteTime;
            anim.SetBool("Jump", false);
        }

        WallSlide();
        WallJump();



        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.W) && canDash)
        {
            StartCoroutine(Dash());
        }

        #endregion


        #region Attack

        if (curTime <= 0)
        {
           
            if(Input.GetKey(KeyCode.UpArrow) && Input.GetKeyDown(KeyCode.Q) && !gameManager.isPanelOpen && !isWallSliding)
            {
                anim.SetTrigger("UpA");
                AudioPlayer.PlayOneShot(AttackSound);
                if (i % 2 == 0 && i == 0)
                {
                    AEffect_Up.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }
                else if (i % 2 == 1)
                {
                    AEffect_Up.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }

                curTime = coolTime;
                i++;
                if (i == 2)
                {
                    i = 0;
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Q) && !gameManager.isPanelOpen && !isWallSliding)
            {
                anim.SetTrigger("DownA");
                AudioPlayer.PlayOneShot(AttackSound);
                if (i % 2 == 0 && i == 0)
                {
                    AEffect_Down.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }
                else if (i % 2 == 1)
                {
                    AEffect_Down.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }

                curTime = coolTime;
                i++;
                if (i == 2)
                {
                    i = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q) && !gameManager.isPanelOpen && !isWallSliding)
            {
                anim.SetTrigger("Attack");
                AudioPlayer.PlayOneShot(AttackSound);
                if (i % 2 == 0 && i == 0)
                {
                    AEffect.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }
                else if (i % 2 == 1)
                {
                    AEffect.gameObject.SetActive(true);
                    Invoke("HideEffect", 0.2f);

                }

                curTime = coolTime;
                i++;
                if (i == 2)
                {
                    i = 0;
                }
            }

        }
        else
        {
            curTime -= Time.deltaTime;
        }




        if (Input.GetKeyDown(KeyCode.S))
        {
            Stat.GetComponent<Stat>().MP += 50;
        }
        #endregion

        #region Heal


        //Down�κп� �̷� ��� �ڷ�ƾ�� ���۵Ǹ� �ȵȴ� �ϴ� ����� �� �߰� 
        if (Input.GetKeyDown(KeyCode.A) && curHp < maxHp && IsGrounded() && Stat.GetComponent<Stat>().MP >= 100)
        {
            StartCoroutine("Heal");
        }
        else if (Input.GetKeyDown(KeyCode.A) && maxHp == curHp)
        {
            Debug.Log("ȸ���� ü���� �����ϴ�");
        }
        else if (Input.GetKeyDown(KeyCode.A) && Stat.GetComponent<Stat>().MP < 100)
        {
            Debug.Log("��������");

        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            StopCoroutine("Heal");
            hcurT = 0f;
            rigid.constraints = RigidbodyConstraints2D.None;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            ZoomOut();
            canDash = true;
            anim.SetBool("Sit", false);
            HealEffect.gameObject.SetActive(false);

        }
        #endregion


        #region Ladder

        Laddervertical = Input.GetAxis("Vertical");

        if (isLadder && Mathf.Abs(Laddervertical) > 0f)
        {
            isClimbing = true;
        }

        if (isLadder && isClimbing)
        {
            anim.SetBool("Ladder", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Run", false);
        }

        #endregion

    }

    void FixedUpdate()
    {

        rigid.AddForce(Vector2.right * horizontal, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); // Right Max Speed
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y); // Left Max Speed

        if (isClimbing)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.x, Laddervertical * Ladderspeed);

        }
        else
        {
            rigid.gravityScale = 4f;
        }

    }



    private void HideEffect()
    {
        AEffect.gameObject.SetActive(false);
        AEffect_Up.gameObject.SetActive(false);
        AEffect_Down.gameObject.SetActive(false);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        IsDashing = true;
        maxSpeed = 80;
        DashAnim();
        AudioPlayer.PlayOneShot(DashSound);
        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0f;
        rigid.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rigid.gravityScale = originalGravity;
        IsDashing = false;
        maxSpeed = 8;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }



    #region Check
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Run", false);
            anim.SetBool("WallS", true);
            isWallSliding = true;
            rigid.velocity = new Vector2(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            anim.SetBool("WallS", false);
            isWallSliding = false;
        }

    
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            IsWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            IsWallJumping = true;
            rigid.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;


        }
    }


    private void StopWallJumping()
    {
        IsWallJumping = false;
    }

    #endregion

    IEnumerator Heal()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.A))
            {
                //����� ���� �����ؾ��ϴ°͵�
                hcurT += Time.deltaTime;
                canDash = false;
                anim.SetBool("Idle", false);
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                ZoomIn();
                HealEffect.gameObject.SetActive(true);
                anim.SetBool("Sit", true);

                if (hgoalT <= hcurT)
                {
                    //�� ������
                    AudioPlayer.PlayOneShot(HealSound);
                    HealEffect.gameObject.SetActive(false);
                    ZoomOut();
                    StartCoroutine(StageMgr.Instance.MoveNext3(fadeInOut, SmoothMoving));
                    ShakeCamera.instance.StartShake(0.2f, 0.2f);
                    anim.SetBool("Sit", false);
                    Stat.GetComponent<Stat>().MP -= 100;
                    canDash = true;
                    rigid.constraints = RigidbodyConstraints2D.None;
                    rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                    Damaged(1);
                    hcurT = 0f;
                    yield break;
                }
            }
            yield return null;
        }
    }

  void HideHealEffect()
    {
        HealEffect.gameObject.SetActive(false);

    }


    public void DashAnim()
    {
        anim.SetTrigger("Dash");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            anim.SetBool("Ladder", false);
            isLadder = false;
            isClimbing = false;
        }
    }

    public void Damaged(int dmg)
    {
        if(dmg > 0)
        {
            for (int i = 0; i < dmg; i++)
            {
                hp.Recover(curHp);
                curHp++;
            }
            return;
        }
        curHp += dmg;
        hp.udtHp(curHp, maxHp);
        if (curHp <= 0)
        {
            gameManager.isGameOver = true;
            gameManager.PlayerDead();
            gameObject.SetActive(false); //���߿� ������ȭ�ؼ� �ı��� �ٲܿ���
        }
    }

    public void ZoomIn()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, zoomSpeed);
    }

    public void ZoomOut()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 12, 1);
    }
}