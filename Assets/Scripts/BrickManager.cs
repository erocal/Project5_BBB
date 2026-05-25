using System.Collections;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;

public class BrickManager : EnvironmentObject
{

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHit()
    {

        base.OnHit();

        MusicManager.Instance.PlayBrickAudio();

        this.gameObject.Release();

    }

}
