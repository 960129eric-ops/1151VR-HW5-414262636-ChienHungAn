using UnityEngine;

public class TrainerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public GameObject pokeballPrefab;
    public Transform throwPoint;

    public float throwForwardForce = 10f;
    public float throwDownForce = 4.5f;
    public float throwSideForce = 3.5f;

    public float minSwipeDistance = 40f;
    public float doubleTapThreshold = 0.3f;

    private Vector2 touchStartPos;
    private Vector2 touchMoveDir;
    private bool isSwiping;
    private float lastTapTime;
    private bool throwRightNext = true;

    void Update()
    {
        Vector3 moveInput = GetKeyboardInput();

        if (moveInput.sqrMagnitude < 0.01f)
        {
            HandleTouchInput();
            moveInput = new Vector3(touchMoveDir.x, 0f, touchMoveDir.y);
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 dir = moveInput.normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowPokeball();
        }
    }

    Vector3 GetKeyboardInput()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v -= 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v += 1f;

        return new Vector3(h, 0f, v);
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (Time.time - lastTapTime <= doubleTapThreshold)
                {
                    ThrowPokeball();
                    lastTapTime = 0f;
                }
                else
                {
                    lastTapTime = Time.time;
                }

                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector2 delta = touch.position - touchStartPos;
                if (delta.magnitude >= minSwipeDistance)
                {
                    touchMoveDir = delta.normalized;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
                touchMoveDir = Vector2.zero;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastTapTime <= doubleTapThreshold)
                {
                    ThrowPokeball();
                    lastTapTime = 0f;
                }
                else
                {
                    lastTapTime = Time.time;
                }

                touchStartPos = Input.mousePosition;
                isSwiping = true;
            }
            else if (Input.GetMouseButton(0) && isSwiping)
            {
                Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;
                if (delta.magnitude >= minSwipeDistance)
                {
                    touchMoveDir = delta.normalized;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isSwiping = false;
                touchMoveDir = Vector2.zero;
            }
        }
    }

    public void ThrowPokeball()
    {
        if (pokeballPrefab == null || throwPoint == null) return;

        GameObject ball = Instantiate(pokeballPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            float sideDir = throwRightNext ? 1f : -1f;
            throwRightNext = !throwRightNext;

            Vector3 diagonalDir = (transform.forward * throwForwardForce)
                                + (Vector3.down * throwDownForce)
                                + (transform.right * (throwSideForce * sideDir));

            rb.AddForce(diagonalDir, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }
    }
}