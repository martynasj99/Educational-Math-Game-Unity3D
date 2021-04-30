using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * reference : https://www.youtube.com/watch?v=Dn_BUIVdAPg&ab_channel=Brackeys
 */
public class WeaponSwitch : MonoBehaviour
{

    public GameObject[] balls;
    public int selected = 0;

    public bool isPlayer;

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        int previousSelected = selected;

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SwitchWeaponForward();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SwitchWeaponBack();
        }

        if (previousSelected != selected)
        {
            SelectWeapon();
        }
    }

    public void SwitchWeaponForward()
    {
        if (isPlayer)
        {
            if (selected >= transform.childCount - 1)
            {
                selected = 0;
            }
            else
            {
                selected++;
            }
        }
        SelectWeapon();

    }

    public void SwitchWeaponBack()
    {
        if (isPlayer)
        {
            if (selected <= 0)
            {
                selected = transform.childCount - 1;
            }
            else
            {
                selected--;
            }
        }
        SelectWeapon();
    }

    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == selected)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
