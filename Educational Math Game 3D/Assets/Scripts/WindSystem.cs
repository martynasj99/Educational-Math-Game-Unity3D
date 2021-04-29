using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindSystem : MonoBehaviour
{
    public float windSpeed;
    public Vector3 direction;

    public static List<GameObject> windable = new List<GameObject>();

    public GameObject arrow;
    public GameObject speedText;

    void Start()
    {
        InvokeRepeating("ApplyWind", 0.1f, 0.1f);
    }

    private void Update()
    {
        speedText.GetComponent<Text>().text = windSpeed + "m/s";
    }

    public static void AddWindable(GameObject windableObject)
    {
        windable.Add(windableObject);
    }

    public void GenerateWind()
    {
        windSpeed = Random.Range(0.0f, 150.0f);
        direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        arrow.transform.Rotate(0, 0, Mathf.Atan2(direction.z, direction.x )*180/Mathf.PI+90, Space.World);
    }

    public void ApplyWind()
    {
        foreach (GameObject o in windable.ToArray()){
            if(o == null)
            {
                windable.Remove(o);
            }
            else
            {
                o.GetComponent<Rigidbody>().AddForce(direction * windSpeed);
            }
        }
    }
}
