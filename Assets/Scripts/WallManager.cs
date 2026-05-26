using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : EnvironmentObject
{

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnHit(GameObject hitObject)
    {
        base.OnHit(hitObject);


    }

}
