using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject logginMenu;
    [SerializeField] private GameObject spawnerController;

    private bool hasServerStarted;

    private void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-client") { StartClient(); }
            else if (args[i] == "-launch-as-server") { StartServer(); }
            else if (args[i] == "-launch-as-host") { StartHost(); }
        }
    }

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

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Servidor Conectad");
            logginMenu.SetActive(false);
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Error al conectar Servidor");
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
