using UnityEngine;

public class LarvaMedkit : MonoBehaviour, ICollectable
{
    [SerializeField] private Transform larvaModel;
    [SerializeField] private float squashSpeed = 6f;
    [SerializeField] private float squashAmount = 0.3f;
    [SerializeField] private float stretchAmount = 0.2f;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointReachDistance = 1.5f;
    [SerializeField] private bool randomOrder = true;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundAlignSpeed = 5f;
    [SerializeField] private float groundStickForce = 20f;
    [SerializeField] private float maxSlopeAngle = 50f;
    [SerializeField] private float slopeSpeedMultiplier = 1.5f;

    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private Vector3 originalScale;
    private bool isCollected = false;
    private float animationTime = 0f;

    private int currentWaypointIndex = 0;
    private Transform targetWaypoint;
    private Rigidbody rb;
    private Vector3 groundNormal = Vector3.up;
    private bool isGrounded = false;

    void Start()
    {
        if (larvaModel != null)
        {
            originalScale = larvaModel.localScale;
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.mass = 2f;
        rb.drag = 3f;
        rb.angularDrag = 10f;
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (waypoints != null && waypoints.Length > 0)
        {
            SetNextWaypoint();
        }
    }

    void Update()
    {
        if (!isCollected)
        {
            AnimateLarva();
        }
    }

    void FixedUpdate()
    {
        if (!isCollected)
        {
            CheckGround();
            ApplyHoverForce();
            MoveToWaypoint();
            AlignToGround();
        }
    }

    void AnimateLarva()
    {
        if (larvaModel == null) return;

        animationTime += Time.deltaTime * squashSpeed;
        float wave = (Mathf.Sin(animationTime) + 1f) * 0.5f;
        float scaleY = originalScale.y * (1f - (squashAmount * wave));
        float scaleZ = originalScale.z * (1f + (stretchAmount * wave));
        larvaModel.localScale = new Vector3(originalScale.x, scaleY, scaleZ);
    }

    void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = Vector3.down;

        if (Physics.Raycast(rayStart, rayDirection, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            groundNormal = hit.normal;

            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (slopeAngle > maxSlopeAngle)
            {
                groundNormal = Vector3.Lerp(groundNormal, Vector3.up, 0.5f);
            }
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }
    }

    void ApplyHoverForce()
    {
        if (!isGrounded) return;

        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = Vector3.down;

        if (Physics.Raycast(rayStart, rayDirection, out hit, groundCheckDistance, groundLayer))
        {
            float distanceToGround = hit.distance;

            if (distanceToGround < groundCheckDistance * 0.8f)
            {
                Vector3 stickForce = Vector3.down * groundStickForce;
                rb.AddForce(stickForce, ForceMode.Force);
            }
        }
    }

    void MoveToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0 || targetWaypoint == null || !isGrounded)
            return;

        Vector3 targetPosition = targetWaypoint.position;
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;
        direction.Normalize();

        Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, groundNormal).normalized;

        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        float slopeFactor = 1f;

        if (slopeAngle > 5f)
        {
            slopeFactor = slopeSpeedMultiplier;

            Vector3 upSlope = Vector3.ProjectOnPlane(Vector3.up, groundNormal).normalized;
            Vector3 antiGravity = upSlope * (moveSpeed * 20f);
            rb.AddForce(antiGravity, ForceMode.Force);
        }

        Vector3 moveForce = slopeDirection * moveSpeed * 100f * slopeFactor;
        rb.AddForce(moveForce, ForceMode.Force);

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float maxSpeed = moveSpeed * 2.5f;

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        float horizontalDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)
        );

        if (horizontalDistance <= waypointReachDistance)
        {
            SetNextWaypoint();
        }
    }

    void AlignToGround()
    {
        if (!isGrounded)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = Mathf.LerpAngle(currentRotation.x, 0, groundAlignSpeed * Time.fixedDeltaTime);
            transform.eulerAngles = currentRotation;
            return;
        }

        if (targetWaypoint != null)
        {
            Vector3 targetDirection = (targetWaypoint.position - transform.position);
            targetDirection.y = 0;

            if (targetDirection.magnitude > 0.01f)
            {
                float targetYRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                Vector3 currentRotation = transform.eulerAngles;
                currentRotation.y = Mathf.LerpAngle(currentRotation.y, targetYRotation, rotationSpeed * Time.fixedDeltaTime);
                transform.eulerAngles = currentRotation;
            }
        }

        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(groundNormal, Vector3.up), groundNormal);
        float targetXRotation = -slopeAngle * Mathf.Sign(Vector3.Dot(slopeDirection, transform.forward));

        Vector3 rotation = transform.eulerAngles;
        rotation.x = Mathf.LerpAngle(rotation.x, targetXRotation, groundAlignSpeed * Time.fixedDeltaTime);
        transform.eulerAngles = rotation;
    }

    void SetNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (randomOrder)
        {
            int randomIndex = Random.Range(0, waypoints.Length);

            if (waypoints.Length > 1)
            {
                while (randomIndex == currentWaypointIndex)
                {
                    randomIndex = Random.Range(0, waypoints.Length);
                }
            }

            currentWaypointIndex = randomIndex;
        }
        else
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        targetWaypoint = waypoints[currentWaypointIndex];
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        IMedkitInventory inventory = other.GetComponent<IMedkitInventory>();

        if (inventory != null)
        {
            if (inventory.AddMedkit())
            {
                Collect();
            }
        }
    }

    public void Collect()
    {
        if (isCollected) return;

        isCollected = true;

        if (collectSound != null)
            AudioManager.PlaySFX2D(collectSound);

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        // Notificar al manager
        if (LarvaManager.Instance != null)
            LarvaManager.Instance.MarkLarvaAsCollected(this);

        // Solo desactivar, NO destruir
        gameObject.SetActive(false);
    }
}