using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    Vector2 dir;

    float[] origin = new float[2];

    public Rigidbody2D body;

    public bool simulating = false;

    float speed = 5;

    public int idOfPlayer;

    public  struct collisionInfo
    {
        public float timestamp;
        public int idOfPlayer;
        public int idOfZombie;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        this.transform.parent.SetParent(null);
        dir = new Vector2(0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        

        
    }

    private void FixedUpdate()
    {
        body.velocity = dir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            simulating = false;
            gameObject.SetActive(false);
        }

        if (collision.transform.tag == "Wall")
        {
            simulating = false;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            simulating = false;
            gameObject.SetActive(false);
            collisionInfo data;
            data.timestamp = Server.instance.gameTime;
            data.idOfPlayer = idOfPlayer;
            data.idOfZombie = collision.gameObject.GetComponent<AIZombie>().id;
            GameManager.instance.collisions.Add(data);
            Debug.Log("Hit Zombie");
        }
        
    }

    public void Fire(Vector2 origin, Vector2 _dir)
    {
        gameObject.SetActive(true);
        transform.position = origin;
        dir = _dir.normalized;
        simulating = true;
    }

}
