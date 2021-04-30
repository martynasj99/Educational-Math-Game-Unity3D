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

    public void Hit()
    {
        GameObject.Find("QuizManager").GetComponent<QuizManager>().ProvideNumber(value);
        GameObject explodeParticle = Instantiate(explode, transform.position, Quaternion.identity);
        Destroy(explodeParticle, 1.0f);
        Die();
    }

    public void Die()
    {
        Invoke("Respawn", 3.0f);
        Destroy(gameObject, 3.0f);
        
    }

    

    public void Respawn()
    {
        Instantiate(gameObject, startPosition, startRotation);
    }
    
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void ApplyModifier(int modifier)
    {
        value = Mathf.Abs(value) * modifier;
        Hit();
    }
}
