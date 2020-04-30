using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public float projectileDamage;

    private Rigidbody2D rb;




    //public GameObject destroyEffect;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroyProjectile", lifetime);

    }

    private void Update()
    {
        rb.velocity = new Vector2(speed, 0);

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (other.gameObject.layer == 10 || other.gameObject.layer == 9 || other.gameObject.layer == 8)
        {
            if (other.gameObject.tag=="Enemy")
            {
                enemy.GetComponent<Enemy>().TakeDamage(projectileDamage);
            }
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        //Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
