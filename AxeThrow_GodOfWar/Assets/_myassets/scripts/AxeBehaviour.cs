using UnityEngine;

public class AxeBehaviour : MonoBehaviour
{

    PlayerManager playerManager;
    Rigidbody axeRb;

    public float rotateSpeed;

    public bool isThrowed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        axeRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isThrowed)
            transform.Rotate(transform.up * rotateSpeed * Time.deltaTime, Space.World);
        else if (playerManager.isReturning)
            transform.Rotate(transform.up * -rotateSpeed * Time.deltaTime, Space.World);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            axeRb.isKinematic = true;
            isThrowed = false;
        }
    }
}
