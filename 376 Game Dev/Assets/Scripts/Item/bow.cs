using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow : MonoBehaviour {
    float weaponStr = 10f;
    int durability;
    public Object arrow;

    private void Start()
    {
        durability = Random.Range(30, 50);
        arrow = Resources.Load("Arrow", typeof(GameObject));
        if (arrow == null)
        {
            Debug.Log("fail");
        }
        
    }

    public void weaponAttack(float attackVar, int attack)
    {
        durability--;
        Debug.Log(durability);
        if (durability == 0)
        {
            gameObject.GetComponent<PlayerController>().unequip();
        }

        Instantiate(arrow, transform.position, transform.rotation);


    }
}
