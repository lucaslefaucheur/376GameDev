﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour {
    float weaponStr;
    int durability;

    private void Start()
    {
        durability = Random.Range(10, 15);
        
    }

    public int weaponAttack(float attackVar, int attack)
    {
        weaponStr = Random.Range(5, 15);
        durability--;
        Debug.Log(durability);
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }
        return (int)Mathf.Floor((attack + weaponStr) * (1 + attackVar));


    }
}
