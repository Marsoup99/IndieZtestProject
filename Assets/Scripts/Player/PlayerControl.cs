using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float score = 2;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private GateSpawner gateSpawner;
    [SerializeField] private TextMeshProUGUI scoreText;
    private Action RunThis;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        score = 0;
        RunThis = Idle;
        rb.gravityScale = 0;
        ResetPlayerPos();
        ResetScore();
    }

    private void Idle()
    {
        if(Input.GetMouseButtonDown(0))
        {
            rb.gravityScale = 1;
            gateSpawner.PlayerContinue();
            RunThis = Playing;
        }
    }
    private void Playing()
    {
        if(gateSpawner.isStop) return;
        if(Input.GetMouseButtonDown(0))
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }
    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }
    public void ResetPlayerPos()
    {
        this.transform.position = Vector2.left * 2;
    }
    // Update is called once per frame
    void Update()
    {
        RunThis();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            score ++;
            scoreText.text = score.ToString();
            gateSpawner.SpawnGate();
        }
        else {
            PlayerDead();
        }
    }

    private void PlayerDead()
    {
        gateSpawner.PlayerDead();
    }

}
