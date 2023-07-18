using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GateSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> gates;
    [SerializeField] private GameObject gate;
    public Action onPlayerDead;
    public Action onPlayerContinue;
    public bool isStop = true;
    void Start()
    {
        gates = new List<GameObject>();
        isStop = true;
    }
    public void SpawnGate()
    {
        Vector2 pos = Vector2.right * 10 + Vector2.up * UnityEngine.Random.Range(-3,5);
        if(gates.Count > 0)
        {
            gates[0].transform.position = pos;
            gates[0].SetActive(true);
            gates.RemoveAt(0);
        }
        else 
        {
            Instantiate(gate, pos, Quaternion.identity, transform);
        }
    }

    public void DeSpawn(GameObject go)
    {
        go.SetActive(false);
        gates.Add(go);
    }
    public void PlayerDead()
    {
        isStop = true;
    }
    public void PlayerContinue()
    {
        isStop = false;
    }
}
