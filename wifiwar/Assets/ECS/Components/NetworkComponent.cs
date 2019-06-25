using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct NetworkComponent : IComponentData
{
    /* The network component contains all of the information
     need to communicate with owner of this entity. */
    public int connectionIdx;
    
}
