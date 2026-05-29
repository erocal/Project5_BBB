using System.Collections;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;

public class BrickManager : EnvironmentObject
{

    // Start is called before the first frame update
    void Awake()
    {

        LevelCounter.Instance?.AddBrick();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnHit(GameObject hitObject)
    {

        base.OnHit(hitObject);

        MusicManager.Instance.PlayBrickAudio();

        LevelCounter.Instance.RemoveBrick();

        this.gameObject.Release();

    }

}
