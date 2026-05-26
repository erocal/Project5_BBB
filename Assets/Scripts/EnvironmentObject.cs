using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{

    protected AudioSource audioSource;

    public virtual void TakeHit(GameObject hitObject)
    {

        OnHit(hitObject);

    }

    protected virtual void OnHit(GameObject hitObject)
    {

        audioSource?.Stop();
        audioSource?.Play();

    }

}
