using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ClientContext
{
    public static Dictionary<string, ClientContext> ClientContexts = new Dictionary<string, ClientContext>();
    
    public string clientId;
    public Entity playerEntity;
    
}
