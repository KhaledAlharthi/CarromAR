using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragLine : MonoBehaviour
{
    public float minSwipeDistance;
    public float errorRange;

    RectTransform imageRect;
    Vector2 startPos;
    Vector2 endPos;
    public SwipeDirection direction = SwipeDirection.None;
    public enum SwipeDirection { Right, Left, Up, Down, None }

    // Start is called before the first frame update
    void Start()
    {
        imageRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                var deltaX = touch.position.x - startPos.x; //greater than 0 is right and less than zero is left
                var deltaY = touch.position.y - startPos.y; //greater than 0 is up and less than zero is down

                var swipeDistance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);

                if (swipeDistance > minSwipeDistance && (Mathf.Abs(deltaX) > 0 || Mathf.Abs(deltaY) > 0))
                {
                    CalculateSwipeDirection(deltaX, deltaY);

                    if (direction == SwipeDirection.Up || direction == SwipeDirection.Down)
                    {


                        GetComponent<RawImage>().enabled = true;

                        endPos = touch.position;
                        Vector3 differenceVector = endPos - startPos;

                        imageRect.sizeDelta = new Vector2(differenceVector.magnitude, 15);
                        imageRect.pivot = new Vector2(0, 0.5f);
                        imageRect.position = startPos;
                        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
                        imageRect.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    else if (direction == SwipeDirection.Right || direction == SwipeDirection.Left)
                    {
                        GetComponent<RawImage>().enabled = false;

                        float diff = (touch.position - startPos).magnitude;
                        float p = diff / (Screen.width * 0.5f);
                        p = Mathf.Min(p, 1);
                        p = direction == SwipeDirection.Right ? p : -p;
                        print("Moving ball " + p);
                        TapToPlace.get().moveMainBall(p);
                    }
                }

                
            }

            if (touch.phase == TouchPhase.Ended)
            {
               

                if (direction == SwipeDirection.Up || direction == SwipeDirection.Down)
                { 
                    int height = Screen.height;
                    float diff = (endPos - startPos).magnitude;
                    float p = diff / (height / 2.0f);
                    p = Mathf.Min(p, 1);
                    TapToPlace.get().shootSliderReleased(p);
                } 

                    GetComponent<RawImage>().enabled = false;
            }
        }

        
    }



    void CalculateSwipeDirection(float deltaX, float deltaY)
    {
        bool isHorizontalSwipe = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

        // horizontal swipe
        if (isHorizontalSwipe && Mathf.Abs(deltaY) <= errorRange)
        {
            //right
            if (deltaX > 0)
                direction = SwipeDirection.Right;
            //left
            else if (deltaX < 0)
                direction = SwipeDirection.Left;
        }
        //vertical swipe
        else if (!isHorizontalSwipe && Mathf.Abs(deltaX) <= errorRange)
        {
            //up
            if (deltaY > 0)
                direction = SwipeDirection.Up;
            //down
            else if (deltaY < 0)
                direction = SwipeDirection.Down;
        }
    }
}
