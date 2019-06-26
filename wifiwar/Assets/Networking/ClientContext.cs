using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ClientContext
{
    public static Dictionary<int, ClientContext> ClientContexts = new Dictionary<int, ClientContext>();
    
    public int connectionIndex;
    public Entity playerEntity;
    
}
