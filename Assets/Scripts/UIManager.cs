using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject logginMenu;
    public void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host Conectado");
            logginMenu.SetActive(false);
            Cursor.visible = false;
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
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("Error al conectar Cliente");
        }
    }
}
