using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 3.5f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothvelocity;
    public bool HasPowerUp = false;
    public GameObject _gameObject;
    public float powerTime = 0.0f;


    [SerializeField]
    PhotonView pv;

  //  public bool HasPowerUp { get; internal set; }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
        
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3 (horizontal, 0f, vertical).normalized;
        _gameObject = GameObject.FindGameObjectWithTag("CherryImage");

        powerTime -= Time.deltaTime;

        if (powerTime > 0.0f && !HasPowerUp)
        {
            HasPowerUp = true;
          
        }
        else if (HasPowerUp)
        {
            HasPowerUp = false;
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only master

        if(other.CompareTag("Cherry"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            _gameObject.SetActive(true);
            
        }

        if(other.CompareTag("Ghost") && HasPowerUp)
        {
            other.GetComponent<GhostNavAgent>().Kill();
        }

        if (other.CompareTag("PowerUp"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            HasPowerUp = true;
        }
    }

    
}
