using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject playerPrefab;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        GameObject plr = Instantiate(playerPrefab,new Vector3(0,1,0), Quaternion.identity);
        cinemachineVirtualCamera.Follow = plr.transform;
        cinemachineVirtualCamera.LookAt = plr.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
