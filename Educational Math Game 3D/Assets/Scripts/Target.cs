using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject explode;
    //public GameObject destroyedObject;

    public int value;
    public float health;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.Find("QuizManager").GetComponent<QuizManager>().ProvideNumber(value);
        //Instantiate(destroyedObject, destroyedObject.transform.position, destroyedObject.transform.rotation);
        //gameObject.SetActive(false);
        GameObject explodeParticle = Instantiate(explode, transform.position, Quaternion.identity);
        
        GameObject.Destroy(explodeParticle, 1.0f);
        Destroy(gameObject, 3.0f);

        health = 100f;
        Invoke("Respawn", 2.0f);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        Instantiate(gameObject, startPosition, startRotation);
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void ApplyModifier(int modifier)
    {
        value = Mathf.Abs(value) * modifier;
        Die();
    }
}
