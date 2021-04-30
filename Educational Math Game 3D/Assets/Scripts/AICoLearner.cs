using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AICoLearner : MonoBehaviour
{

    public Transform[] targets;

    public GameObject player;
    public GameObject gunHolder;

    public Transform target;
    public Transform moveTarget;

    public int lookSpeed = 5;

    private Vector3 dir;
    public bool isExecuting;

    public GameObject helpText;

    void Start()
    {
        target = player.transform;
        moveTarget = transform;
    }

    void Update()
    {
        if(QuizManager.currentQuestion.answer < 0)
        {
            gunHolder.GetComponent<WeaponSwitch>().selected = 1;
        }
        else
        {
            gunHolder.GetComponent<WeaponSwitch>().selected = 0;   
        }
        gunHolder.GetComponent<WeaponSwitch>().SelectWeapon();
        LookAt(target);
        MoveTowards(moveTarget);
    }

    private void LookAt(Transform target)
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * lookSpeed);
    }

    public void MoveTowards(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, transform.position.y, transform.position.z), 10.0f*Time.deltaTime);
    }

    public IEnumerator ExecuteAction(int number)
    {
        Debug.Log("Number @ " + number);
        yield return new WaitForSeconds(1);
        target = targets[number];
        moveTarget = target;
        yield return new WaitForSeconds(2);
        gunHolder.GetComponent<Projectile>().Throw();
        yield return new WaitForSeconds(1);
        target = player.transform;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            helpText.GetComponent<Text>().text = "Press H for Help";
            if (Input.GetKeyDown(KeyCode.H))
            {
                GameObject.Find("SpeechManager").GetComponent<SpeechManager>().Help(QuizManager.level);
               
            }
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        helpText.GetComponent<Text>().text = "";
    }
}
