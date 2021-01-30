using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToPlace : MonoBehaviour
{

    public static TapToPlace instance;

    public static TapToPlace get()
    {
        if (instance == null)
        {
            instance = new TapToPlace();
        }
        return instance;
    }

    public GameObject blackScoreText;
    public GameObject yellowScoreText;
    public GameObject totalScoreText;

    public PhysicMaterial physicMaterialBall;
    public GameObject mainBall;
    public GameObject carromObj;
    public GameObject placementIndicator;
    public GameObject blackBall;
    public GameObject yellowBall;
    public GameObject redBall;

    LineRenderer line;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool poseIsValid = false;
    private bool boardAdded = false;
    private bool black = false;
    private bool mainAtCenter = true;
    private Vector3 mainBallCenter;

    private int blackScore = 0, yellowScore = 0, score = 0;
    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        line = FindObjectOfType<LineRenderer>();
        line.enabled = false;
        //mainBall = Instantiate(mainBall).gameObject;

    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (!boardAdded)
        {
            UpdatePlacementPose();
        }
        

        if (!mainAtCenter && mainBall.GetComponent<Rigidbody>().IsSleeping())
        {


            mainAtCenter = true;
            mainBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            mainBall.transform.position = mainBallCenter;
            line.enabled = true;
        }


        if (boardAdded)
        {
            mainBall.SetActive(true);
            carromObj.SetActive(true);
        } else
        {
            UpdatePlacementIndicator();
            mainBall.SetActive(false);
            carromObj.SetActive(false);
        }
        


        if (!boardAdded && poseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            boardAdded = true;
            PlaceCarrom();
            placementIndicator.SetActive(false);

        }


        if (mainAtCenter && boardAdded)
        {
            Vector3 target = getTargetPosition();
            if (target != Vector3.zero)
            {
                line.enabled = true;
                line.SetPosition(0, mainBall.transform.position - new Vector3 (0, 0, 0.0020f));
                line.SetPosition(1, new Vector3(target.x, mainBall.transform.position.y, target.z));
            }
            
        }






    }

    private void UpdatePlacementIndicator()
    {
        
        if (poseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        } else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        poseIsValid = hits.Count > 0;
        if (poseIsValid) {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void PlaceCarrom()
    {
        carromObj.SetActive(true);
        carromObj.transform.rotation = placementPose.rotation;
        carromObj.transform.position = placementPose.position;

        mainAtCenter = true;
        mainBall.transform.position = placementPose.position + new Vector3(0, 0.0446f, -0.3089f);
        Quaternion adjustedRotation = Quaternion.Euler(new Vector3(placementPose.rotation.eulerAngles.x - 90, placementPose.rotation.eulerAngles.y, placementPose.rotation.eulerAngles.z));
        mainBall.transform.rotation = adjustedRotation;
        mainBall.transform.RotateAround(carromObj.transform.position, Vector3.up, placementPose.rotation.eulerAngles.y);

        mainBallCenter = mainBall.transform.position;


        Vector3 boardCenter = new Vector3(0.0014f, 0.0408f, 0.0068f);
        Quaternion rot = Quaternion.Euler(placementPose.rotation.eulerAngles.x - 90, placementPose.rotation.eulerAngles.y, placementPose.rotation.eulerAngles.z);
        Vector3 redPos = placementPose.position + boardCenter;
        GameObject instance = Instantiate (redBall, redPos, rot);
        instance.transform.localScale = new Vector3(0.0013f, 0.0013f, 0.0013f);
        instance.transform.RotateAround(carromObj.transform.position, Vector3.up, placementPose.rotation.eulerAngles.y);
        Rigidbody rr = instance.AddComponent<Rigidbody>();
        rr.useGravity = true;
        rr.constraints = RigidbodyConstraints.FreezeRotation;
        rr.drag = 0f;
        rr.mass = 0.5f;
        instance.AddComponent<Ball>().initPos = redPos;
        instance.AddComponent<SphereCollider>().material = physicMaterialBall;





        int numberOfObjects = 6;
        float radius = 0.0332f;
        for (int i = 0; i < numberOfObjects; i++)
        {
            initBall(black, radius, numberOfObjects, i);
            black = !black;
        }

        black = true;
        radius = 0.0594f;
        numberOfObjects = 14;
        for (int i = 0; i < numberOfObjects; i++)
        {
            initBall(black, radius, numberOfObjects, i);
            black = !black;
        }

    }

    private void initBall(bool black, float radius, int numberOfObjects, int position) 
    {
        Vector3 boardCenter = new Vector3(0.0014f, 0.0408f, 0.0068f);
        float angle = position * Mathf.PI * 2 / numberOfObjects;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 pos = placementPose.position + new Vector3(x, 0, z) + boardCenter;
        float angleDegrees = -angle * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(placementPose.rotation.eulerAngles.x - 90, angleDegrees, placementPose.rotation.eulerAngles.z);
        GameObject ballInstance;
        
        if (black)
        {
            
            ballInstance = Instantiate(blackBall, pos, rot);
            Ball ball = ballInstance.AddComponent<Ball>();
            ball.ballColor = BallColor.BLACK;
        }
        else
        {
            ballInstance = Instantiate(yellowBall, pos, rot);
            Ball ball = ballInstance.AddComponent<Ball>();
            ball.ballColor = BallColor.YELLOW;
        }
        
        ballInstance.transform.localScale = new Vector3(0.0013f, 0.0013f, 0.0013f);
        Rigidbody br = ballInstance.AddComponent<Rigidbody>();
        br.mass = 0.5f;
        br.drag = 0f;
        br.useGravity = true;
        br.constraints = RigidbodyConstraints.FreezeRotation;
        SphereCollider collider = ballInstance.AddComponent<SphereCollider>();
        collider.material = physicMaterialBall;
    }

    Vector3 getTargetPosition()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);


        if (hits.Count > 0)
        {
            return hits[0].pose.position;
        }

        return Vector3.zero;

    }

    public void shootSliderReleased(float slider_value)
    {

        Vector3 target = getTargetPosition();

        if (target != Vector3.zero)
        {
            target = new Vector3(target.x, mainBall.transform.position.y, target.z);
            Vector3 push = (target - mainBall.transform.position) * (slider_value * 5);
            mainBall.GetComponent<Rigidbody>().AddForce(push, ForceMode.Impulse);
            mainAtCenter = false;
            line.enabled = false;
        }

    }


    public void moveMainBall (float value) {
        float max = 0.28f;
        mainBall.transform.position = placementPose.position + new Vector3(max * value, 0.0408f, -0.3089f);
        Quaternion adjustedRotation = Quaternion.Euler(new Vector3(placementPose.rotation.eulerAngles.x - 90, placementPose.rotation.eulerAngles.y, placementPose.rotation.eulerAngles.z));
        mainBall.transform.rotation = adjustedRotation;
        mainBall.transform.RotateAround(carromObj.transform.position, Vector3.up, placementPose.rotation.eulerAngles.y);
    }



    public void countScore(BallColor color)
    {
        switch (color)
        {
            case BallColor.BLACK:
                blackScore++;
                score += 5;
                blackScoreText.GetComponent<Text>().text = blackScore + "";
                break;
            case BallColor.YELLOW:
                yellowScore++;
                score += 10;
                yellowScoreText.GetComponent<Text>().text = yellowScore + "";
                break;
            case BallColor.RED:
                score += 50;
                break;
        }
        totalScoreText.GetComponent<Text>().text = score + "";
    }

}
