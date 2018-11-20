using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    float weaponStr = 10f;
    int durability;

    private void Start()
    {
        durability = Random.Range(1, 5);

    }

    public int weaponAttack(float attackVar, int attack)
    {
        weaponStr = Random.Range(1, 5);
        durability--;
        Debug.Log(durability);
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }
        return (int)Mathf.Floor( weaponStr * (1 + attackVar));


    }
}
