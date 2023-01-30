using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hp == 0 && Hp <0)
        {
            Destroy(this);
        }
    }

    public int Hp = 5;

    public void TakeDamage(int damage)
    {
        Hp = Hp - damage;
    }

    
}
