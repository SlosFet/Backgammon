using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private ParticleSystem _brokeParticle;
    [SerializeField] private ParticleSystem _collectParticle;

    public void PlayBrokeParticle(Vector3 pos)
    {
        var particle = Instantiate(_brokeParticle);
        _brokeParticle.transform.position = pos;
        _brokeParticle.Play();
        Destroy(particle.gameObject,3);
    }

    public void PlayCollectParticle(Vector3 pos)
    {
        var particle = Instantiate(_collectParticle);
        _brokeParticle.transform.position = pos;
        _brokeParticle.Play();
        Destroy(particle.gameObject, 3);
    }
}
