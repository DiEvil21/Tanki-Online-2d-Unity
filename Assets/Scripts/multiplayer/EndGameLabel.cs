using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class EndGameLabel : MonoBehaviourPunCallbacks
{
    public Image player_image;
    public TextMeshProUGUI coins_count;
    private SpriteRenderer player_spriteRenderer;

    private PlayerController playerController;

    private void OnEnable()
    {
        // Получаем компонент PlayerController
        playerController = FindObjectOfType<PlayerController>();
        player_spriteRenderer = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        // Получаем компоненты цвета (R, G, B, A) из SpriteRenderer игрока
        float r = player_spriteRenderer.color.r;
        float g = player_spriteRenderer.color.g;
        float b = player_spriteRenderer.color.b;
        float a = player_spriteRenderer.color.a;

        // Устанавливаем цвет player_image, используя полученные компоненты цвета
        Color imageColor = new Color(r, g, b, a);
        player_image.color = imageColor;

        // Устанавливаем coins_count, взяв его из скрипта PlayerController у оставшегося игрока
        coins_count.text = playerController.coins.ToString();

        // Показываем изменения всем игрокам через Photon
        photonView.RPC("UpdatePlayerInfo", RpcTarget.All, r, g, b, a, playerController.coins);
    }
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }

    [PunRPC]
    private void UpdatePlayerInfo(float r, float g, float b, float a, int coins)
    {
        Color imageColor = new Color(r, g, b, a);
        player_image.color = imageColor;
        coins_count.text = coins.ToString();
    }
}
