using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoundSide : MonoBehaviour
{
    Rigidbody2D rigid;
    bool platformrepeat;
    bool enemyrepeat;
    [SerializeField] PlayerAudio playerAudio = null;

    void OnEnable()
    {
        enemyrepeat = platformrepeat = false;
        rigid = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            if (!platformrepeat && !enemyrepeat)
            {
                platformrepeat = true;
                rigid.velocity = Vector2.zero;
                Player.Instance.sideForce();
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (!platformrepeat && !enemyrepeat)
            {
                playerAudio.Play(PlayerAudio.AudioType.Dameged_Enemy, true);

            }
        }
    }
}
