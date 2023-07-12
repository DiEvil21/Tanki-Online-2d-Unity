using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 move;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float projectileLifetime = 2f;
    private float lastSpawnTime;
    private PhotonView view;
    private int health = 3;
    public int coins = 0;
    Image progressbar;
    TextMeshProUGUI coins_counter;
    

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        
        if (view.IsMine)
        {
            
            Color color = Random.ColorHSV();
            view.RPC("RPC_SetColor", RpcTarget.AllBuffered, color.r,color.g,color.b,color.a);
        }
        
        progressbar = GameObject.FindGameObjectWithTag("progress_bar").GetComponent<Image>();
        coins_counter = GameObject.FindGameObjectWithTag("coins_counter").GetComponent<TextMeshProUGUI>();
        
    }
    [PunRPC]
    void RPC_SetColor(float r, float g, float b, float a)
    {
        GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine) 
        {
            MovePlayer();
            UpdateHUD();
        }
        
    }
    private void UpdateHUD() 
    {
        progressbar = GameObject.FindGameObjectWithTag("progress_bar").GetComponent<Image>();
        if (health >= 0)
        {
            progressbar.fillAmount = (float)health/3;
        }
        else 
        {
            progressbar.fillAmount = 0f;
        }
        if (coins_counter != null) 
        {
            coins_counter.text = coins.ToString();
        }
        

        
    }
    public void MovePlayer()
    {
        if (move.sqrMagnitude > 0.1f)
        {
            Vector3 movement = new Vector3(move.x, move.y, 0f).normalized;
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

            transform.position += movement * speed * Time.deltaTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.tag.Equals("coin"))
        {
            coins++;
            PhotonNetwork.Destroy(collidedObject);
        }
            if (collidedObject.tag.Equals("projecttile")) 
        {
            PhotonView projectilePhotonView = collidedObject.GetComponent<PhotonView>();

            if (projectilePhotonView != null && projectilePhotonView.Controller == PhotonNetwork.LocalPlayer)
            {
                // Это ваш снаряд, пропускаем обработку
                return;
            }

            // удаляем нашего игрока
            if (view.IsMine)
            {
                
                health--;
                if (health <= 0) 
                {
                    progressbar.fillAmount = 0;
                    PhotonNetwork.Destroy(transform.gameObject);
                }
            }
        }
        
    }

    public void SpawnProjectile()
    {
        if (view.IsMine) 
        {
            if (Time.time - lastSpawnTime >= 1f)
            {
                GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, transform.position, Quaternion.identity);

                Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
                projectileRb.velocity = transform.right * projectileSpeed;


                Destroy(projectile, projectileLifetime);

                lastSpawnTime = Time.time;
            }
        }
        
    }
    public int GetCoins()
    {
        return coins;
    }

    

}
