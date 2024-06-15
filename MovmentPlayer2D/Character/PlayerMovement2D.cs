using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
//var movimentação em X
    private float move;
    [SerializeField]private float moveSpeed;
//var movimentação em y/Jump
    [SerializeField]private float jumpSpeed;
    [SerializeField]private float dbjumpSpeed;
    [SerializeField]private bool isGrounded;
    [SerializeField]private int nJump;
    public Transform footPosition;
    public LayerMask whatIsGround;
    public float sizeRadius; 
//var dash
    [SerializeField]private float dashSpeed;
    [SerializeField]private bool canDash;
    [SerializeField]private bool isDashing;
    [SerializeField]private float dashingTime;
    [SerializeField]private float dashingCoolDown;
    [SerializeField] TrailRenderer tr;

//var geral
    public Rigidbody2D rigB;
    public SpriteRenderer sprite;
    public Animator animationPlayer;

    public bool flipX;

// Todos os comandos no Start são executados somente uma vez, na inicialização do Script
  void Start()
    {
        rigB =  GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animationPlayer = GetComponent<Animator>();
        
    }

// Update executa o código a cada freme, porem objetos com fisica não é recomendado que sejam executados aqui
    void Update()
    {
        if (isDashing)
        {
            return;
        }
//Reconhecimento do chão
        isGrounded = Physics2D.OverlapCircle(footPosition.position,sizeRadius,whatIsGround);
//input que recebe o valor da movimentação do personagem, o valor vai de -1 a 1, toda essa movimentação ocorre no eixo x (Horizontal)
//-1 = esquerda
//1 = direita
        move = Input.GetAxisRaw("Horizontal");
        if(move != 0)
        {
            moveSpeed += 15f * Time.deltaTime;
            if(moveSpeed >= 6.0f)
            {
                moveSpeed = 6.0f;
            }
        }
        else
        {
            moveSpeed = 0;
        }

//inverter a posição do sprite do personagem
        if (flipX == false && move < 0 )    
        {
            Flip();
        }
        else if (flipX == true && move > 0)
        {
            Flip();    
        }
//+-------------------------------------------------+
//Pulo do Personagem
        if(isGrounded == true)
        {
            nJump = 1;
            if(Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
        }
        else
        {
            if(Input.GetButtonDown("Jump") && nJump > 0 )
                {
                    nJump--;
                    DoubleJump();
                    
                }
        }
//+-------------------------------------------------+
//Dash do Personagem
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
       
//CONTROLADOR DAS ANIMAÇÕES DO PERSONAGEM

//Quando o personagem estiver em contato com o chão
        if (isGrounded == true)
        {
        
            animationPlayer.SetBool("Jump",false);
            animationPlayer.SetBool("JumpToFall",false);
            animationPlayer.SetBool("Fall",false);
//Quando o persongem começar a correr e os valores da velocidade forem diferentes de 0
// 1 = direita
//-1 = esquerda 
            if(rigB.velocity.x != 0 && move != 0 )
            {
                animationPlayer.SetBool("Run",true);
            }
//caso o personagem pare de se movimentar (move = 0), a animação de corrida será resetada e a de parado entrara como padrão
            else
            {
                animationPlayer.SetBool("Run",false);
            }
        }
//Se o personagem não estiver em contato com o chão então as seguintes animaçoes serão executadas
        else
        {
            if(rigB.velocity.y > 0)
            {
                animationPlayer.SetBool("Jump",true);
                animationPlayer.SetBool("JumpToFall",false);
                animationPlayer.SetBool("Fall",false);
            }
            if(rigB.velocity.y < 0 && rigB.velocity.y > -0.5 )
            {    
                animationPlayer.SetBool("Jump",false);
                animationPlayer.SetBool("JumpToFall",true);
                animationPlayer.SetBool("Fall",false); 
            }
            if(rigB.velocity.y < -0.5 )
            {    
                animationPlayer.SetBool("Jump",false);
                animationPlayer.SetBool("JumpToFall",false);
                animationPlayer.SetBool("Fall",true); 
            }
        }
    }
//FixedUpdate executa o código um controle fixo de frames per second, estipulado pela engine de física da Unity, sendo assim é melhor para funçoes com fisica 
    void FixedUpdate() 
    {
        if (isDashing)
        {
            return;
        }
        rigB.velocity = new Vector2(move*moveSpeed,rigB.velocity.y);
    }

    void Flip()
    {
        flipX = !flipX;
        float x = transform.localScale.x;
        x*= -1;
        transform.localScale = new Vector3(x,transform.localScale.y,transform.localScale.z);
    }
//função que executa o pulo do personagem
    void Jump()
    {
        rigB.velocity = Vector2.up*jumpSpeed;
    }
//função do double jump
    void DoubleJump()
    {
        rigB.velocity = Vector2.up*dbjumpSpeed;
    }
//rotina do dash
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rigB.gravityScale;
        rigB.gravityScale = 0f;
        rigB.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds (dashingTime);
        tr.emitting = false;
        rigB.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

}

