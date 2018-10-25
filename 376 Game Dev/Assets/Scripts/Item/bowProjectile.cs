﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bowProjectile : MonoBehaviour {

    private ItemAttack bow;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * 500);
        bow = GetComponentInParent<ItemAttack>();
    }

    public int damage()
    {
        return bow.damage();
    }
}