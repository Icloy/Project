using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMap : MonoBehaviour
{
    bool exitTrigger = false;
    GameObject exitpos;
    GameObject player;
    GameObject map;
    [SerializeField] GameObject text;
    MapGenerator mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveMap()
    {
        while (exitTrigger)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                player.transform.position = new Vector3(exitpos.transform.position.x, exitpos.transform.position.y + 2f, exitpos.transform.position.z);
                map.GetComponent<MapGenerator>().ClearBspMap();
                exitpos.GetComponent<EnterMap>().SetCreateTrigger();
            }
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            text.gameObject.SetActive(true);
            player = collision.gameObject;
            exitpos = GameObject.FindWithTag("ExitMap");
            map = GameObject.FindWithTag("BSPMap");
            exitTrigger = true;
            StartCoroutine("MoveMap");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            text.gameObject.SetActive(false);
            exitTrigger = false;
            StopCoroutine("MoveMap");
        }
    }
}
