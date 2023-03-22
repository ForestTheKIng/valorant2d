using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    
    PhotonView pv;

    GameObject controller;

    int kills;
    int deaths;
    
    public GameObject killFeedItem;

    [SerializeField] GameObject killFeedItemPrefab;
    [SerializeField] Transform killFeedContent;

    private const string TEAM_PROPERTY_KEY = "team";

    void Awake() {
        pv = GetComponent<PhotonView>();
        Settings.Instance.SceneChanged();
        // killFeedItemPrefab = Path.Combine("Prefabs", "KillFeedItem");
    }
    // Start is called before the first frame update
    void Start()
    {
        if (pv.IsMine){
            CreateController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static int GetPlayerTeam(Player player)
    {
        object teamObj;
        if (player.CustomProperties.TryGetValue(TEAM_PROPERTY_KEY, out teamObj))
        {
            return (int)teamObj;
        }
        return -1;
    }
    
    void CreateController(){
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerContainer"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID });
    }

    public void Die(){
        if (controller == null){
            return;
        }
        PhotonNetwork.Destroy(controller);
        controller.SetActive(false);
        CreateController();
        
        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    
    public void InstantiateKillFeedMessage(string killer, string killed){
        Debug.Log("Instantiating kill message: " + killer + " " + "brutally murdered" + " " + killed);
        GameObject killFeedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","KillFeedItem"), Vector3.zero, Quaternion.identity);
        killFeedItem.GetComponent<KillFeedItem>().SetUp(killer, killed);
        pv.RPC("RPC_SetKillMessageParent", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SetKillMessageParent(){
        Debug.Log("controller");
        killFeedItem.transform.SetParent(killFeedContent);
    }

    public void GetKill(){
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }

    [PunRPC]
    void RPC_GetKill(){
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player){
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }

}