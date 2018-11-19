using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour {
    float weaponStr = 10f;
    int durability;

    private void Start()
    {
        durability = Random.Range(30, 50);
    }

    public int weaponAttack(float attackVar, int attack)
    {
        durability--;
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }
        return (int)Mathf.Floor((attack + weaponStr) * (1 + attackVar));


    }
}
