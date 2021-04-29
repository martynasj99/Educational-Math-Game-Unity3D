using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICoLearner : MonoBehaviour
{

    public Transform[] targets;

    public GameObject player;
    public GameObject gunHolder;

    public Transform target;

    public int lookSpeed = 5;

    private Vector3 dir;
    public bool isExecuting;


    void Start()
    {
        target = player.transform;

    }

    void Update()
    {
        if(QuizManager.currentQuestion.answer < 0)
        {
            gunHolder.GetComponent<WeaponSwitch>().selected = 1;
            gunHolder.GetComponent<WeaponSwitch>().SelectWeapon();
        }
        LookAt(target);
    }
/*    private void OnCollisionStay(Collision collision)
    {
            Vector3 direction = collision.transform.position - transform.position;
            transform.position = new Vector3(transform.position.x - (direction.x > 0 ? 0.1f : -0.1f), transform.position.y, transform.position.z - (direction.z > 0 ? 0.1f : -0.1f));
    }*/

    private void LookAt(Transform target)
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * lookSpeed);
    }

    public IEnumerator ExecuteAction(int number)
    {
        yield return new WaitForSeconds(1);
        target = targets[number];
        yield return new WaitForSeconds(2);
        gunHolder.GetComponent<Projectile>().Throw();
        yield return new WaitForSeconds(1);
        target = player.transform;
    }
}
