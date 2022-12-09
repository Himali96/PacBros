using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class GhostNavAgent : MonoBehaviour, IPunObservable
{
    const float kTimeOfDeath = 5f;
    
    public Transform playerTransform;
    public NavMeshAgent agent;
    SkinnedMeshRenderer skinnedMeshRenderer;
    GameObject goPlayer,spawnPoint;
    bool playerPowerUp;
    public float CloseEnoughDistance = 5f;

    Vector3 startPosition;
    bool isDeath;

    PhotonView pv;

    public bool isChasingOn = true;

    public Material normalMat;
    public Material evadeMat;
    bool useNormalMat = true;

    public Transform[] waypoints;

    [SerializeField]
    private int currentWaypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        startPosition = transform.position;
        spawnPoint = GameObject.FindGameObjectWithTag("EnemySpawnPoint");

        GameManager._instance.OnPlayerListChange += OnPlayerListChange;
        GameManager._instance.OnGameFinish += OnGameFinish;

        currentWaypointIndex = Random.Range(0, waypoints.Length);
    }

    void OnDestroy()
    {
        if(GameManager._instance)
        {
            GameManager._instance.OnPlayerListChange -= OnPlayerListChange;
            GameManager._instance.OnGameFinish -= OnGameFinish;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isDeath) return;

        // The player is dead or disconnected, ask for the next player
        if (playerTransform == null)
        {
            OnPlayerListChange();
            if (playerTransform == null)
                return;
        }

        playerPowerUp = GameManager._instance.HasPowerUp;
        
        if(playerPowerUp)
        {
            if(Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
            {
                agent.destination = spawnPoint.transform.position;
            }

            skinnedMeshRenderer.material = evadeMat;
            useNormalMat = false;
        }
        else
        {
            if(isChasingOn)
                agent.destination = playerTransform.position;
            else
            {
                if(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) > 1f)
                {
                    agent.destination = waypoints[currentWaypointIndex].position;
                }
                else
                {
                    currentWaypointIndex = Random.Range(0, waypoints.Length);
                }
            }

            skinnedMeshRenderer.material = normalMat;
            useNormalMat = true;
        }

        /*if (playerPowerUp && Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
        {
            agent.destination = spawnPoint.transform.position;
        }
        else
        {
            agent.destination = playerTransform.position;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        
        if(other.gameObject.CompareTag("Player") && playerPowerUp == false)
        {
            PhotonView otherPv = other.GetComponent<PhotonView>();
            GameManager._instance.PlayerDie(otherPv);
            pv.RPC(nameof(DestroyRpc), otherPv.Owner, otherPv.ViewID);
        }
    }

    [PunRPC]
    void DestroyRpc(int _pvId)
    {
        PhotonNetwork.Destroy(PhotonView.Find(_pvId).gameObject);
    }

    void OnPlayerListChange()
    {
        playerTransform = GameManager._instance.GetPlayerToFollow();

        if (playerTransform)
        {
            goPlayer = playerTransform.gameObject;
        }
    }

    public void Kill()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        
        pv.RPC(nameof(KillRpc), RpcTarget.All);
    }

    [PunRPC]
    void KillRpc()
    {
        isDeath = true;
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = startPosition;
        }
        Invoke(nameof(Revive), kTimeOfDeath);
    }

    void Revive()
    {
        isDeath = false;
    }

    void OnGameFinish()
    {
        agent.speed = 0f;
        agent.isStopped = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(useNormalMat);
        }
        else
        {
            bool newUseNormalMat = (bool) stream.ReceiveNext();

            if (newUseNormalMat != useNormalMat)
            {
                useNormalMat = newUseNormalMat;
                skinnedMeshRenderer.material = newUseNormalMat ? normalMat : evadeMat;
            }
        }
    }
}
