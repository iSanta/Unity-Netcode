using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject logginMenu;
    [SerializeField] private GameObject spawnerController;

    private bool hasServerStarted;
    public void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host Conectado");
            logginMenu.SetActive(false);
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Error al conectar Host");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Cliente Conectad");
            logginMenu.SetActive(false);
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Error al conectar Cliente");
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            PyshicsSpawn();
        }
    }



    public void PyshicsSpawn()
    {
        if (!hasServerStarted)
        {
            Debug.Log("El servidor aun no inicia");
        }

        spawnerController.GetComponent<SpawnerController>().SpawnObjects();
    }
}
