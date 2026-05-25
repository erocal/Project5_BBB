using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{

    protected AudioSource audioSource;

    public virtual void TakeHit()
    {

        OnHit();

    }

    protected virtual void OnHit()
    {

        audioSource?.Stop();
        audioSource?.Play();

    }

}
