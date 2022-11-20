using UnityEngine;
using Photon.Pun;

public class PlayerNetwork : MonoBehaviourPun
{
    public static PlayerNetwork LocalPlayer { get; private set; }

    [SerializeField]
    MonoBehaviour[] scriptTurnOffIfIsNotLocal = null;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv.AmOwner)
        {
            LocalPlayer = this;

            gameObject.name += "(Local)";

            // Set Camera to follow us
            Cinemachine.CinemachineFreeLook camCinemachine = GameObject.Find("Third Person Camera").GetComponent<Cinemachine.CinemachineFreeLook>();
            camCinemachine.Follow = transform;
            camCinemachine.LookAt = transform;

            GetComponent<ThirdPersonMovement>().cam = Camera.main.transform;
        }
        else
        {
            foreach (MonoBehaviour script in scriptTurnOffIfIsNotLocal)
            {
                script.enabled = false;
            }
        }

        GameManager._instance.NewPlayerConnected(pv);
    }
}
