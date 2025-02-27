using UnityEngine;

public class Orbit : MonoBehaviour
{
    // Real value of gravitational constant is 6.67408 � 10-11
    // Can increase to make things go faster instead of increasing the timestep of Unity
    readonly float G = 1000f;
    GameObject[] celestials;

    [SerializeField]
    bool IsElipticalOrbit = false;

    private Vector3[] storedVelocities;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        celestials = GameObject.FindGameObjectsWithTag("Celestial");
        storedVelocities = new Vector3[celestials.Length];
        SetInitialVelocity();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPaused)
        {
            Gravity();
        }
    }

    void SetInitialVelocity()
    {
        foreach (GameObject a in celestials)
        {
            foreach (GameObject b in celestials)
            {
                if (!a.Equals(b))
                {
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    a.transform.LookAt(b.transform);

                    if (IsElipticalOrbit)
                    {
                        // Eliptic orbit = G * M  ( 2 / r + 1 / a) where G is the gravitational constant, M is the mass of the central object, r is the distance between the two bodies
                        // and a is the length of the semi major axis (!!! NOT GAMEOBJECT a !!!)
                        a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) * ((2 / r) - (1 / (r * 1.5f))));
                    }
                    else
                    {
                        // Circular Orbit = ((G * M) / r)^0.5, where G = gravitational constant, M is the mass of the central object and r is the distance between the two objects
                        // We ignore the mass of the orbiting object when the orbiting object's mass is negligible, like the mass of the earth vs. mass of the sun
                        a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) / r);
                    }
                }
            }
        }
    }

    void Gravity()
    {
        foreach (GameObject a in celestials)
        {
            foreach (GameObject b in celestials)
            {
                if (!a.Equals(b))
                {
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized * (G * (m1 * m2) / (r * r)));
                }
            }
        }
    }

    public void PauseMovement()
    {
        isPaused = true;
        for (int i = 0; i < celestials.Length; i++)
        {
            Rigidbody rb = celestials[i].GetComponent<Rigidbody>();
            storedVelocities[i] = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true; // Disables physics
        }
    }

    public void ResumeMovement()
    {
        isPaused = false;
        for (int i = 0; i < celestials.Length; i++)
        {
            Rigidbody rb = celestials[i].GetComponent<Rigidbody>();
            rb.isKinematic = false; // Re-enables physics
            rb.velocity = storedVelocities[i];
        }
    }
}
