using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {
    float weaponStr = 10f;
    public int weaponAttack(float attackVar, int attack)
    {
        return (int)Mathf.Floor((attack + weaponStr) * (1 + attackVar));
    }
}
