using UnityEngine;
using UnityOSC;

public class RocketController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float thrustForce = 10f;
    public float maxSpeed = 20f;

    private Rigidbody rb;
    private bool isThrusting = false;

    [Header("Thruster Effect")]
    public GameObject thrusterEffect;
    public float flashSpeed = 10f;
    private Renderer thrusterRenderer;
    private Color originalColor;

    public GameObject collectibleParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 0.5f;
        rb.useGravity = false;
        rb.freezeRotation = true;

        OSCHandler.Instance.Init();

        thrusterRenderer = thrusterEffect.GetComponent<Renderer>();
        originalColor = thrusterRenderer.material.color;
    }

    void Update()
    {
        HandleThrust();

        HandleThrusterEffect();
    }

    void HandleThrust()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float upDown = 0f;
        if (Input.GetKey(KeyCode.Space)) upDown = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) upDown = -1f;

        bool wasThrusting = isThrusting;
        isThrusting = (horizontal != 0 || vertical != 0 || upDown != 0);

        if (isThrusting)
        {
            Vector3 thrust = new Vector3(horizontal, upDown, vertical) * thrustForce;
            rb.AddForce(thrust);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        if (isThrusting != wasThrusting)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/thruster", isThrusting ? 1 : 0);
        }
    }

    void HandleThrusterEffect()
    {
        if (isThrusting)
        {
            float flash = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            thrusterRenderer.material.color = Color.Lerp(originalColor, Color.red, flash);
        }
        else
        {
            thrusterRenderer.material.color = originalColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            Instantiate(collectibleParticles, other.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/collection", 1);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/crash", 1);
    }
}