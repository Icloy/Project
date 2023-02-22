using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    CircleCollider2D circle;
    Rigidbody2D rigid;

    Coroutine actcoroutine;
    public int actmove;
    public float movespeed;
    public GameObject Player;
    public GameObject Boss;
    public GameObject trap;
    public GameObject drop;

    private float dis;
    private int direction;
    private int dropcnt;
    private int dropran;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
    }

    public IEnumerator Think()
    {
        while (true)
        {
            dis = Vector2.Distance(Player.transform.position, Boss.transform.position);
            Vector2 player = Player.transform.position;
            if (player.x < Boss.transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = 2;
            }
            actmove = Random.Range(1, 3);
            Debug.Log(dis);
            Debug.Log(direction);
            Debug.Log(actmove);
            if (dis <= 2.5)
            {
                StartCoroutine(act1(actmove));
            }
            else if (dis <= 5)
            {
                StartCoroutine(act2(actmove));
            }
            else
            {
                switch (actmove)
                {
                    case 1:
                        Debug.Log("else case1");
                        for (int i = -2; i < 3; i++)
                        {
                            Instantiate(trap, new Vector3(Player.transform.position.x + 1f * i, Player.transform.position.y + 4f, Player.transform.position.z), Quaternion.identity);
                        }
                        break;
                    case 2:
                        Debug.Log("else case2");
                        dropcnt = Random.Range(-3, 5);
                        Debug.Log(dropcnt);
                        for (int i = -3; i <= dropcnt; i++)
                        {
                            dropran = Random.Range(1, 6);
                            if(dropran <= 4)
                            {
                                Instantiate(drop, new Vector3(Player.transform.position.x +1f * i, Player.transform.position.y + 4f, Player.transform.position.z), Quaternion.identity);
                            }
                        }
                        break;
                }
            }
            yield return new WaitForSeconds(4f);
        }
       
    }

    public IEnumerator act1(int actmove)
    {
        Debug.Log("act1");
        switch (actmove)
        {
            case 1:
                break;
            case 2:
                break;
        }
        yield return new WaitForSeconds(3f);
    }

    public IEnumerator act2(int actmove)
    {
        Debug.Log("act2");
        switch (actmove)
        {
            case 1:
                rigid.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
                if(direction == 1)
                {
                    rigid.AddForce(Vector2.left * dis, ForceMode2D.Impulse);
                }
                else if (direction == 2)
                {
                    rigid.AddForce(Vector2.right * dis, ForceMode2D.Impulse);
                }
                break;
            case 2:
                int i = Random.Range(1, 3);
                switch (i)
                {
                    case 1:
                        gameObject.transform.position = new Vector3(Player.transform.position.x + 1f, Player.transform.position.y, Player.transform.position.z);
                        break;
                    case 2:
                        gameObject.transform.position = new Vector3(Player.transform.position.x + -1f, Player.transform.position.y, Player.transform.position.z);
                        break;
                }
                break;
        }
        yield return new WaitForSeconds(3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Think());
        }
    }

}