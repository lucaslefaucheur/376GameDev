using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour {

    public GameObject item;
    public string weaponType;
    public int cooldown;

    // On pickup
    public void getItem(ref string type, ref GameObject itemPref, ref int cd)
    {
        itemPref = item;
        type = weaponType;
        cd = cooldown;

        Destroy(gameObject);
    }
}
