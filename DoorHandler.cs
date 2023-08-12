using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{

    public Quaternion startQuaternion;
    //public Vector3 quaternionToVector;
    public Vector3 reversequaternion;
    public float Lerptime = 1f;
    public bool playerneerthedoor = false;
    [SerializeField] double door_Limit = 0.65;
    [SerializeField] AudioSource DourSound;



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")&& gameObject.transform.rotation.y < door_Limit)
        {
            Debug.Log("Open");
            playerneerthedoor = true;
            DourSound.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Close");
            playerneerthedoor = false;
            DourSound.Pause();
        }
    }

    void Start()
    {
        startQuaternion = transform.rotation;   
    }

    
    void Update()
    {
        if (playerneerthedoor == true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(reversequaternion), Time.deltaTime * Lerptime);
            //Debug.Log(gameObject.transform.rotation.y);
        }
        
    }

    public void snapRotation()
    {
        transform.rotation = startQuaternion;
    }
}
