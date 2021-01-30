using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootSlider : MonoBehaviour, IPointerUpHandler
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Sliding finished");
        
        TapToPlace.get().shootSliderReleased(1 - GetComponent<Slider>().value);
        GetComponent<Slider>().value = 1;
    }



}
