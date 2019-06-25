using System.Collections;
using System.Collections.Generic;
using ClientMessages;
using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    public IEnumerator StartKeepAliveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Keep Alive Hello Sent ");
            ClientMessageHello hello = new ClientMessageHello();
            ServerConnection.Send(hello);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartKeepAliveCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
