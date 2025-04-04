using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Plyaer : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 nextVec = Time.fixedDeltaTime * speed * inputVec.normalized;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);
        
        if (inputVec.x != 0 )
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}
