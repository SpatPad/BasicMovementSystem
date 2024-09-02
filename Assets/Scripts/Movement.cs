using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintingMultiplier = 2.5f;
    public float jumpForce = 5f;
    public float dashSpeed = 10f;
    public float momentumMultiplier = 1.5f;
    public float dashCooldown = 0.5f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isSprinting = false;
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
    void Sprint() 
    {
        
    }


    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * momentum, rb.velocity.z);
            isGrounded = false;

            // Oblicz kierunek ruchu na podstawie prêdkoœci gracza
            Vector3 moveDirection = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

            // Jeœli gracz siê porusza, wykonaj salto w kierunku ruchu
            if (moveDirection != Vector3.zero)
            {
                StartCoroutine(PerformFlip(moveDirection));
            }
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

    IEnumerator PerformFlip(Vector3 direction)
    {
        // Zapisanie pocz¹tkowej rotacji
        Quaternion startRotation = transform.rotation;

        // Obrót o 360 stopni wokó³ osi odpowiadaj¹cej kierunkowi ruchu
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(360, direction);

        float flipDuration = 0.5f; // Czas trwania salta
        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            // Interpolacja rotacji od startRotation do endRotation w czasie
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ustawienie koñcowej rotacji, aby upewniæ siê, ¿e rotacja jest dok³adna
        transform.rotation = endRotation;
    }

    void Dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
        {
            Vector3 dashDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (dashDirection != Vector3.zero)
            {
                // Nag³e na³o¿enie si³y na kapsu³ê w kierunku dasha
                rb.AddForce(dashDirection * dashSpeed);
                isDashing = true; // Ustawienie flagi, ¿e gracz wykonuje dash
                canDash = false;

                // Obrót gracza przy daszu z zachowaniem rotacji 90 stopni w osi X
                transform.rotation = Quaternion.LookRotation(dashDirection) * Quaternion.Euler(90, 0, 0);

                Invoke("ResetDash", dashCooldown);
            }
        }
    }

    void Squeeze()
    {
        if (Input.GetButtonDown("Squeeze"))
        {
            // Œciœnij kapsu³ê poprzez zmianê skali
            transform.localScale = new Vector3(1.5f, transform.localScale.y, 0.5f);
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

        // Sprawdzenie, czy gracz wykonuje dash oraz czy koliduje z przeciwnikiem
        if (collision.gameObject.CompareTag("Enemy") && isDashing)
        {
            Destroy(collision.gameObject);
        }
    }

    void ResetDash()
    {
        canDash = true;
        isDashing = false; // Resetowanie flagi dasha
    }

}