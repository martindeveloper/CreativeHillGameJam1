using UnityEngine;
using System.Collections;

public class PlayerEntity : MonoBehaviour {

    public float Speed = 10.0f;
    public float JumpPower = 300.0f;

    private Transform PlayerTransform;
    private Rigidbody2D PlayerRigidBody;

    private bool IsFacingRight = true;
    private bool IsJumping = false;

    private bool HaveKey = false;

    public void Start()
    {
        PlayerTransform = GetComponent<Transform>();
        PlayerRigidBody = GetComponent<Rigidbody2D>();
    }

	public void Update()
    {
    }

    public void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleJumpMovement();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                IsJumping = false;
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        switch(collider.gameObject.tag)
        {
            case "Key":
                KeyEntity key = collider.gameObject.GetComponent<KeyEntity>();
                key.Pickup();

                HaveKey = true;
                break;

            case "Door":
                if (HaveKey)
                {
                    GameManager.Instance.OnWin();
                }
                break;
        }
    }

    private void HandleJumpMovement()
    {
        float jump = Input.GetAxis("Jump");

        if(jump > 0 && !IsJumping)
        {
            PlayerRigidBody.AddForce(Vector2.up * JumpPower, ForceMode2D.Force);
            IsJumping = true;
        }
    }

    private void HandleHorizontalMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");

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
