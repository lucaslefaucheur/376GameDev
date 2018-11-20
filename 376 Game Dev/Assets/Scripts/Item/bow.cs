using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow : MonoBehaviour {
    float weaponStr = 10f;
    int durability;

    private void Start()
    {
        durability = Random.Range(15, 25);
        
    }

    public int weaponAttack(float attackVar, int attack)
    {
        weaponStr = Random.Range(10, 25);
        durability--;
        Debug.Log(durability);
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }
        return (int)Mathf.Floor((attack + weaponStr) * (1 + attackVar));


    }
}
