using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ManagerUi : NetworkBehaviour
{
    [SerializeField] private GameObject mainMenu;

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            mainMenu.SetActive(false);
            Debug.Log("Server Conectado");
        }
        else
        {
            Debug.Log("Error: Server no Conectado");
        }
        
    }
    public void StartHost()
    {

        if (NetworkManager.Singleton.StartHost())
        {
            mainMenu.SetActive(false);
            Debug.Log("Host Conectado");
        }
        else
        {
            Debug.Log("Error: Host no Conectado");
        }
    }
    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            mainMenu.SetActive(false);
            Debug.Log("Client Conectado");
        }
        else
        {
            Debug.Log("Error: Client no Conectado");
        }

    }

}
