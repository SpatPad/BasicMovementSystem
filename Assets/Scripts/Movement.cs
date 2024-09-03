using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashSpeed = 10f;
    public float momentumMultiplier = 1.5f;
    public float dashCooldown = 0.5f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool canDash = true;
    private bool isDashing = false; // Flaga do œledzenia dasha
    private float momentum = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ustawienie pocz¹tkowej rotacji kapsu³y, aby le¿a³a na boku
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        Move();
        Jump();
        Dash();
        Squeeze();
        Flatten();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 movement = new Vector3(moveX, 0, moveZ);

        // Jeœli gracz siê porusza (wartoœæ movement nie jest zerowa), ustaw kierunek kapsu³y
        if (movement != Vector3.zero)
        {
            // Ustawienie rotacji w kierunku ruchu z dodatkowym obrotem 90 stopni w osi X
            Quaternion targetRotation = Quaternion.LookRotation(movement) * Quaternion.Euler(90, 0, 0);
            transform.rotation = targetRotation;
        }


        // Dodanie ruchu w osi Y (skok) do wektora ruchu
        movement.y = rb.velocity.y;

        rb.velocity = movement;
    }



    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * momentum, rb.velocity.z);
            isGrounded = false;

            // Oblicz kierunek ruchu na podstawie prêdkoœci gracza
            Vector3 moveDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

            
        }
        else if (isGrounded)
        {
            // Reset momentum, jeœli gracz dotknie ziemi
            momentum = 1f;
        }

        // Jeœli gracz naciska przycisk skoku, ale nie jest na ziemi, zwiêksz momentum
        if (Input.GetButton("Jump") && !isGrounded)
        {
            momentum += Time.deltaTime * momentumMultiplier;
        }
    }


    void Dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
        {
            Vector3 dashDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (dashDirection != Vector3.zero)
            {
                // Nag³e na³o¿enie si³y na kapsu³ê w kierunku dasha
                rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
                isDashing = true; // Ustawienie flagi, ¿e gracz wykonuje dash
                canDash = false;

                // Obrót gracza przy daszu z zachowaniem rotacji 90 stopni w osi X
                transform.rotation = Quaternion.LookRotation(dashDirection) * Quaternion.Euler(90, 0, 0);

                Invoke("ResetDash", dashCooldown);
            }
        }
    }

    void Flatten()
    {
        if (Input.GetButtonDown("Flatten"))
        {
            // Œciœnij kapsu³ê poprzez zmianê skali
            transform.localScale = new Vector3(1.5f, transform.localScale.y, 0.5f);
        }

        if (Input.GetButtonUp("Flatten"))
        {
            // Przywróæ oryginaln¹ skalê kapsu³y
            transform.localScale = new Vector3(1f, transform.localScale.y, 1f);
        }
    }

    void Squeeze()
    {
        if (Input.GetButtonDown("Squeeze"))
        {
            // Œciœnij kapsu³ê poprzez zmianê skali
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 1.5f);
        }

        if (Input.GetButtonUp("Squeeze"))
        {
            // Przywróæ oryginaln¹ skalê kapsu³y
            transform.localScale = new Vector3(1f, transform.localScale.y, 1f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

    }

    void ResetDash()
    {
        canDash = true;
        isDashing = false; // Resetowanie flagi dasha
    }

}