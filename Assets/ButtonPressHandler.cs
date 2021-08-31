using System.Collections;
using Pvr_UnitySDKAPI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;
using TMPro;
using UnityEngine.Video;

public class ButtonPressHandler : MonoBehaviour
{
    
    public GameObject Ball;
    public GameObject Ball_phantom_1, Ball_phantom_2, Ball_phantom_3;

    public GameObject FT1, FT2, FT3;
    public Material MT1, MT2, MT3;

    public VideoPlayer mainVideoPlayer;
    public GameObject videoPlane, schemaPlane;

    public AudioSource audioSource;
    public AudioClip audioClip1, audioClip2, audioClip3;

    public GameObject XY, Y_text;

    Rigidbody rb;
    [SerializeField]
    private Transform headsetPosition; //Assign to 'Pvr_UnitySDK/Head' in inspector.
    [SerializeField]
    private Transform controller0Position; //Assign to 'Pvr_UnitySDK/PvrController0' in inspector.
    private Ray ray = new Ray();

    // Start is called before the first frame update
    void Start()
    {
        
        
        rb = Ball.GetComponent<Rigidbody>();
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        mainVideoPlayer.Stop();
        videoPlane.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((!mainVideoPlayer.isPlaying) && (!audioSource.isPlaying))
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(MouseButtonController());
            }

            if (CheckHandInput(0))
            {
                StartCoroutine(PerformRaycast(controller0Position));
            }
        }
    }

    IEnumerator MouseButtonController()
    {
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
           yield return ControlLogics(hit);
        }
    }
    
    IEnumerator PerformRaycast(Transform rayStart)
    {
        ray.direction = rayStart.forward;
        ray.origin = rayStart.position;
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray); //This can be given a maximum range, if needed.
        foreach (var hit in hits)
        {
            yield return ControlLogics(hit);
        }
    }

    IEnumerator ControlLogics(RaycastHit hit)
    {
        //hit.collider.gameObject.GetComponent<Animator>().Play("Press");
        if (hit.collider.gameObject.name == "StartButton")
        {
            schemaPlane.SetActive(false);
            FT1.SetActive(false);
            FT2.SetActive(false);
            FT3.SetActive(false);

            mainVideoPlayer.loopPointReached += EndReached;
            videoPlane.SetActive(true);
            mainVideoPlayer.Play();
        }
            

        if (hit.collider.gameObject.name == "Cylinder_1")
        {
            setAction(1);
            schemaPlane.SetActive(true);
            schemaPlane.GetComponent<MeshRenderer>().material = MT1;
            audioSource.clip = audioClip1;
            audioSource.Play();
            FT1.SetActive(true);
            FT2.SetActive(false);
            FT3.SetActive(false);

            rb.isKinematic = true;
            Ball.transform.SetPositionAndRotation(new Vector3(0.02f, 1.25f, 3), Quaternion.identity);
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 4000);

            StartCoroutine(CreatePhantom());
        }
        if (hit.collider.gameObject.name == "Cylinder_2")
        {
            setAction(2);
            schemaPlane.SetActive(true);
            schemaPlane.GetComponent<MeshRenderer>().material = MT2;
            audioSource.clip = audioClip2;
            audioSource.Play();
            FT1.SetActive(false);
            FT2.SetActive(true);
            FT3.SetActive(false);

            rb.isKinematic = true;
            Ball.transform.SetPositionAndRotation(new Vector3(-2, 3.68f, 3), Quaternion.identity);
            rb.isKinematic = false;
            rb.AddForce(Vector3.right * 2500);

            StartCoroutine(CreatePhantom());
        }
        if (hit.collider.gameObject.name == "Cylinder_3")
        {
            setAction(3);
            schemaPlane.SetActive(true);
            schemaPlane.GetComponent<MeshRenderer>().material = MT3;
            audioSource.clip = audioClip3;
            audioSource.Play();
            FT1.SetActive(false);
            FT2.SetActive(false);
            FT3.SetActive(true);

            rb.isKinematic = true;
            Ball.transform.SetPositionAndRotation(new Vector3(-2, 1.25f, 3), Quaternion.identity);
            rb.isKinematic = false;
            rb.AddForce(Vector3.right * 1400);
            rb.AddForce(Vector3.up * 4000);

            StartCoroutine(CreatePhantom());
        }

        yield return new WaitForSeconds(0.62f);
    }

    IEnumerator CreatePhantom()
    {
        Ball_phantom_1.SetActive(false);
        Ball_phantom_2.SetActive(false);
        Ball_phantom_3.SetActive(false);

        SetPhantomPosition(Ball_phantom_1);

        yield return new WaitForSeconds(0.18f);
        SetPhantomPosition(Ball_phantom_2);

        yield return new WaitForSeconds(0.62f);
        SetPhantomPosition(Ball_phantom_3);
    }

    void SetPhantomPosition(GameObject phantomGameObject)
    {
        phantomGameObject.transform.SetPositionAndRotation(Ball.transform.position, Quaternion.identity);
        phantomGameObject.SetActive(true);
    }
    void setAction(int i)
    {
        if ((i == 1) || (i == 3))
        {
            XY.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Y_text.transform.localPosition = new Vector3(-2.903178f, 3.28f, 3.960313f);
        }
        else
        {
            XY.transform.localRotation = new Quaternion(180, 0, 0, 0);
            Y_text.transform.localPosition = new Vector3(-2.903178f, -0.23f, 3.960313f);
        }
    }

    private bool RaycastFromHeadInput()
    {
        return Input.GetKey(KeyCode.JoystickButton0);
    }
    private bool CheckHandInput(int handIndex)
    {
        if (Pvr_UnitySDKAPI.Controller.UPvr_GetControllerState(handIndex) != Pvr_UnitySDKAPI.ControllerState.Connected) return false;
        return Controller.UPvr_GetKey(handIndex, Pvr_KeyCode.TRIGGER);
    }

}
