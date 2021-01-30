using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallColor ballColor;
    public Vector3 initPos;

    bool done = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.name == "tlCorner"
            || collision.collider.gameObject.name == "trCorner"
            || collision.collider.gameObject.name == "blCorner"
            || collision.collider.gameObject.name == "brCorner")
        {
            TapToPlace.get().countScore(ballColor);
            gameObject.SetActive(false);
        }
    }




}



public enum BallColor
{
    BLACK,
    RED,
    YELLOW
}
