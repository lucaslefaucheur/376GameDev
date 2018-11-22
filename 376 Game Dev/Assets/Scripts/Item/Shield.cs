using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    float weaponStr = 10f;
    int durability;

    private void Start()
    {
        durability = Random.Range(1, 5);

    }

    public float weaponAttack(float attackVar, int attack)
    {
        weaponStr = Random.Range(2, 4);
        durability--;
        Debug.Log(durability);
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }
        return (weaponStr / 2);


    }
}