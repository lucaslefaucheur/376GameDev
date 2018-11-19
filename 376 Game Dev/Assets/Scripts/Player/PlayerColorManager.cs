using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorManager : MonoBehaviour {

    private Color[] colorString = { new Color(1f, 0.8f, 0.5f, 1f), new Color(0.5f, 0.5f, 1f, 1f), new Color(0.5f, 1f, 0.5f, 1f), new Color(0.8f, 0.5f, 1f, 1f) };
    private int number;
    void Start()
    {
        number = gameObject.GetComponent<PlayerController>().getNumber();
        gameObject.GetComponent<SpriteRenderer>().color = colorString[number-1];
    }
}
