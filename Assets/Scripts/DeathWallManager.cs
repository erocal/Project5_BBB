using System.Collections;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;

public class DeathWallManager : EnvironmentObject
{

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnHit(GameObject hitObject)
    {

        base.OnHit(hitObject);

        hitObject.Release();

    }

}
