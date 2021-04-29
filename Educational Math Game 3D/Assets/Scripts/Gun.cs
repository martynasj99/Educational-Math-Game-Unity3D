using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public int modifier;

    public GameObject source;
    public ParticleSystem bullets;
    public GameObject hitEffect;

    public bool isPlayer;

    void Update()
    {
        if (Input.GetButton("Fire1") && isPlayer)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(source.transform.position, source.transform.forward, out hit, range))
        {
            GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.ApplyModifier(modifier);
                target.TakeDamage(damage);
            }
        }
        bullets.Play();
    }
}
