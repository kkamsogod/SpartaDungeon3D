using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFireHeal : MonoBehaviour
{
    public int heal;
    public float healRate;

    private List<IWarmable> things = new List<IWarmable>();

    private void Start()
    {
        InvokeRepeating("WarmHeal", 0, healRate);
    }

    void WarmHeal()
    {
        for (int i = 0; i < things.Count; i++)
        {
            things[i].HeatUp(heal);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IWarmable warmable))
        {
            things.Add(warmable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IWarmable warmable))
        {
            things.Remove(warmable);
        }
    }
}
