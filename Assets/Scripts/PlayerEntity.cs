using UnityEngine;
using System.Collections;

public class PlayerEntity : MonoBehaviour {

    public float Speed = 10.0f;
    public float JumpPower = 300.0f;
    public float FireDelay = 1.0f;
    public bool IsShooting = false;
    public GameObject ChildSprite;
    public GameObject RespawnPlace;

    private Transform PlayerTransform;
    private Rigidbody2D PlayerRigidBody;
    private Animator PlayerAnimator;

    private bool IsFacingRight = true;
    private bool IsJumping = false;
    private bool IsMoving = false;
    private Coroutine FireCoroutine;

    private GameManager Game;

    public void Start()
    {
        PlayerTransform = GetComponent<Transform>();
        PlayerRigidBody = GetComponent<Rigidbody2D>();
        PlayerAnimator = ChildSprite.GetComponent<Animator>();
        Game = GameManager.Instance;
    }

	public void Update()
    {
        PlayerAnimator.enabled = IsMoving;
    }

    public void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleJumpMovement();
        HandleFire();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case TagsStructure.Ground:
                IsJumping = false;
                break;

            case TagsStructure.Enemy:
                OnDeath();
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        switch(collider.gameObject.tag)
        {
            case TagsStructure.Key:
                KeyEntity key = collider.gameObject.GetComponent<KeyEntity>();
                key.Pickup();
                Game.OnKeyPickup();
                break;

            case TagsStructure.Door:
                DoorEntity door = collider.gameObject.GetComponent<DoorEntity>();

                if (door.IsOpened)
                {
                    Game.OnWin();
                }
                break;

            case TagsStructure.KillingVolume:
                OnDeath();
                break;

            case TagsStructure.Bullet:
                BulletEntity bullet = collider.gameObject.GetComponent<BulletEntity>();

                bullet.OnHit();

                OnDeath();
                break;
        }
    }

    public void OnDeath()
    {
        PlayerTransform.position = RespawnPlace.GetComponent<Transform>().position;

        Game.OnPlayerDeath();
    }

    private void HandleFire()
    {
        if(Input.GetButton("Fire1") && IsShooting)
        {
            if (FireCoroutine == null)
            {
                Debug.Log("Player is shooting");

                FireCoroutine = StartCoroutine(Shoot());
            }
        }
    }

    private IEnumerator Shoot()
    {
        Vector3 bulletPosition = PlayerTransform.position;
        bulletPosition.y += 0.5f;

        GameObject bullet = Instantiate(Game.Settings.PlayerBulletEntity, bulletPosition, Quaternion.identity) as GameObject;

        if (bullet != null)
        {
            BulletEntity bulletObject = bullet.GetComponent<BulletEntity>();
            bulletObject.Direction = (IsFacingRight) ? Vector3.right : Vector3.left;
            bulletObject.Speed *= 2;
            bulletObject.OnShoot();
        }

        yield return new WaitForSeconds(FireDelay);

        FireCoroutine = null;
    }

    private void HandleJumpMovement()
    {
        bool jumpButtonState = Input.GetButton("Jump");

        if(jumpButtonState && !IsJumping)
        {
            PlayerRigidBody.AddForce(Vector2.up * JumpPower, ForceMode2D.Force);
            IsJumping = true;
        }
    }

    private void HandleHorizontalMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");

        IsMoving = false;

        if (horizontalMovement != 0.0f)
        {
            if (horizontalMovement > 0.0f && !IsFacingRight)
            {
                // Right
                FlipPlayerSprite();
            }

            if (horizontalMovement < 0.0f && IsFacingRight)
            {
                // Left
                FlipPlayerSprite();
            }

            float translation = horizontalMovement * Speed * Time.deltaTime;
            Vector2 targetPosition = PlayerRigidBody.position;
            targetPosition.x += translation;

            //PlayerRigidBody.MovePosition(targetPosition);
            PlayerRigidBody.velocity = new Vector2(translation, PlayerRigidBody.velocity.y);

            IsMoving = true;
        }
    }

    private void FlipPlayerSprite()
    {
        IsFacingRight = !IsFacingRight;

        Vector3 playerScale = PlayerTransform.localScale;
        playerScale.x *= -1.0f;

        PlayerTransform.localScale = playerScale;
    }
}
