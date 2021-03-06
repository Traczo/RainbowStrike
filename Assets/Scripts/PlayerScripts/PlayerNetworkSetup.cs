using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class PlayerNetworkSetup : MonoBehaviour
{
    public GameObject playerThirdpersonMesh;
    public GameObject UI;
    public GameObject PlayerHands;
    PhotonView PV;
    public PlayerManager playerManager { get; private set; }

    public Material blueTeamMat;
    public Material redTeamMat;
    public GameObject firstPersonCam;

    public bool isSpectatingThis { get; private set; }

    public GameObject[] hardcoreModeDeactivate;

    public CinemachineVirtualCamera playerCam;

    // Start is called before the first frame update
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        int Id = (int)PV.InstantiationData[0];
        PhotonView managerObj = PhotonView.Find(Id);
        playerManager = managerObj.GetComponent<PlayerManager>();
        playerManager.SetPlayerGameObject(this.gameObject);

    }
    void UpdateSettings()
    {
        playerCam.m_Lens.FieldOfView = PlayerSettings.Instance.FieldOfView;
    }
    void Start()
    {
        PlayerSettings.Instance.OnSettingsChanged += UpdateSettings;
        UpdateSettings();
        if (PV.IsMine)
        {
            playerThirdpersonMesh.layer = LayerMask.NameToLayer("DontSee");
            PlayerHands.layer = LayerMask.NameToLayer("LocalWeapon");

            Team t = playerManager.localPlayerTeam;
            PV.RPC("RPC_SetMeshMaterial", RpcTarget.All, t);

            if(GameManager.Instance.currentGameState == GameState.RoundPrepare)
            {
                GetComponent<PlayerMove>().DisablePlayer();
            }

            if(GameManager.Instance.hardcoreMode)
            {
                foreach(GameObject ob in hardcoreModeDeactivate)
                {
                    ob.SetActive(false);
                }
            }
        }
        else
        {
            PlayerHands.layer = LayerMask.NameToLayer("DontSee");
            UI.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        PlayerSettings.Instance.OnSettingsChanged -= UpdateSettings;
    }

    public void SetSpectatorFpsView()
    {
        isSpectatingThis = true;
        GetComponent<PlayerShooting>().StartSpectating();
        UI.SetActive(true);

        playerThirdpersonMesh.layer = LayerMask.NameToLayer("DontSee");
        PlayerHands.layer = LayerMask.NameToLayer("LocalWeapon");
    }
    public void StopSpectating()
    {
        isSpectatingThis = false;
        GetComponent<PlayerShooting>().StopSpectating();

        UI.SetActive(false);
        PlayerHands.layer = LayerMask.NameToLayer("DontSee");
        playerThirdpersonMesh.layer = 0;
    }

    [PunRPC]
    void RPC_SetMeshMaterial(Team t)
    {
        if(t==Team.Red)
        {
            playerThirdpersonMesh.GetComponent<SkinnedMeshRenderer>().material = redTeamMat;
        }
        else if(t==Team.Blue)
        {
            playerThirdpersonMesh.GetComponent<SkinnedMeshRenderer>().material = blueTeamMat;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
