using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : EnvironmentObject
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
        MusicManager.Instance.PlayWallAudio();

    }

}
