using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using DG.Tweening;

public class ShooterPlayer : MonoBehaviourPun
{
    [SerializeField] float speed = 5;
    [SerializeField] float maxHealth = 10;
    [SerializeField] float health = 100;
    [SerializeField] float restoreValue = 5;
    [SerializeField] float damageValue = 10;
    [SerializeField] Animator animator;
    [SerializeField] ShooterBullet bulletPrefab;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Rigidbody2D rb;
    // [SerializeField] SpriteRenderer rend;
    // [SerializeField] Sprite[] avatarSprites;
    [SerializeField] Renderer rend;
    [SerializeField] Texture[] avatarTexture;
    Vector2 moveDir;
    private void Start()
    {
        playerName.text = photonView.Owner.NickName + $" ({health})";
        if (photonView.Owner.CustomProperties.TryGetValue(PropertyNames.Player.AvatarIndex, out var avatarIndex))
            // rend.sprite = avatarSprites[(int)avatarIndex];
            rend.material.mainTexture = avatarTexture[(int)avatarIndex];

        // set local properties
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var roomMaxHealth))
        {
            this.maxHealth = (float)roomMaxHealth;
            this.health = this.maxHealth;
            playerName.text = photonView.Owner.NickName + $" ({health})";
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.RestoreValue, out var roomRestoreValue))
        {
            this.restoreValue = (float)roomRestoreValue;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.DamageValue, out var roomDamageValue))
        {
            this.damageValue = (float)roomDamageValue;
        }

    }
    private void Update()
    {
        if (photonView.IsMine == false)
            return;
        moveDir = new Vector2(
           Input.GetAxisRaw("Horizontal"),
           Input.GetAxisRaw("Vertical")
       );

        if (moveDir == Vector2.zero)
            animator.SetBool("IsMove", false);
        else
            animator.SetBool("IsMove", true);

        // transform.Translate(moveDir * Time.deltaTime * speed);

        if (Input.GetMouseButtonDown(0))
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
            var directionVector = mouseWorldPos - this.transform.position;

            Fire(this.transform.position, directionVector.normalized, new PhotonMessageInfo());

            photonView.RPC("Fire",
                RpcTarget.Others,
                this.transform.position,
                directionVector.normalized);
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        //     photonView.RPC("TakeDamage", RpcTarget.All, 1);

    }
    private void FixedUpdate()
    {
        if (photonView.IsMine == false)
            return;
        rb.velocity = moveDir * speed;
    }

    [PunRPC]
    public void Fire(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        if (photonView.IsMine)
            lag = 0;

        var bullet = Instantiate(bulletPrefab);
        bullet.Set(this, position, direction, lag);
    }

    [PunRPC]
    public void TakeDamage()
    {
        health -= damageValue;
        health = Mathf.Clamp(health, 0, maxHealth);
        playerName.text = photonView.Owner.NickName + $" ({health})";
        // GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
        var sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo));
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }

    public void RestoreHealth()
    {
        if (photonView.IsMine)
            photonView.RPC("RestoreHealthRPC", RpcTarget.AllViaServer);

    }

    [PunRPC]
    public void RestoreHealthRPC()
    {
        health += restoreValue;
        health = Mathf.Clamp(health, 0, maxHealth);

        playerName.text = photonView.Owner.NickName + $" ({health})";

        // GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(1, LoopType.Yoyo).From();
        var sequence = DOTween.Sequence();
        sequence.Append(rend.material.DOColor(Color.green, 0.2f).SetLoops(1, LoopType.Yoyo));
        sequence.Append(rend.material.DOColor(Color.white, 0.1f));
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (photonView.IsMine)
                photonView.RPC("TakeDamage", RpcTarget.AllViaServer);
        }
    }
}
