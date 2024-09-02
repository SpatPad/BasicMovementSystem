using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashSpeed = 10f;
    public float slideSpeed = 7f;
    public float momentumMultiplier = 1.5f;
    public float dashCooldown = 0.5f;
    public float slideCooldown = 0.5f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool canDash = true;
    private bool canSlide = true;
    private bool isSliding = false;
    private bool isDashing = false; // Flaga do �ledzenia dasha
    private float momentum = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ustawienie pocz�tkowej rotacji kapsu�y, aby le�a�a na boku
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        Move();
        Jump();
        Dash();
        Slide();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 movement = new Vector3(moveX, 0, moveZ);

        // Je�li gracz si� porusza (warto�� movement nie jest zerowa), ustaw kierunek kapsu�y
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

            // Obr�t gracza przy skoku, imituj�cy salto z zachowaniem rotacji 90 stopni w osi X
            StartCoroutine(PerformFlip());
        }
        else if (isGrounded)
        {
            // Reset momentum, je�li gracz dotknie ziemi
            momentum = 1f;
        }

        // Je�li gracz naciska przycisk skoku, ale nie jest na ziemi, zwi�ksz momentum
        if (Input.GetButton("Jump") && !isGrounded)
        {
            momentum += Time.deltaTime * momentumMultiplier;
        }
    }

    IEnumerator PerformFlip()
    {
        // Zapisanie pocz�tkowej rotacji
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(360, 0, 0);

        float flipDuration = 0.5f; // Czas trwania salta
        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            // Interpolacja rotacji od startRotation do endRotation w czasie
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / flipDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ustawienie ko�cowej rotacji, aby upewni� si�, �e rotacja jest dok�adna
        transform.rotation = endRotation;
    }

    void Dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
        {
            Vector3 dashDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (dashDirection != Vector3.zero)
            {
                // Nag�e na�o�enie si�y na kapsu�� w kierunku dasha
                rb.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);
                isDashing = true; // Ustawienie flagi, �e gracz wykonuje dash
                canDash = false;

                // Obr�t gracza przy daszu z zachowaniem rotacji 90 stopni w osi X
                transform.rotation = Quaternion.LookRotation(dashDirection) * Quaternion.Euler(90, 0, 0);

                Invoke("ResetDash", dashCooldown);
            }
        }
    }

    void Slide()
    {
        if (Input.GetButtonDown("Slide") && canSlide && isGrounded)
        {
            isSliding = true;
            Vector3 slideDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (slideDirection != Vector3.zero)
            {
                rb.velocity = slideDirection * slideSpeed;
                canSlide = false;

                // Obr�t gracza przy �lizgu z zachowaniem rotacji 90 stopni w osi X
                transform.rotation = Quaternion.LookRotation(slideDirection) * Quaternion.Euler(90, 0, 0);

                Invoke("ResetSlide", slideCooldown);
            }
        }

        if (isSliding && Input.GetButtonUp("Slide"))
        {
            isSliding = false;
            canSlide = true;
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

    void ResetSlide()
    {
        canSlide = true;
    }
}