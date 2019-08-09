using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

[DisallowMultipleComponent]
public class Obstacle : MonoBehaviour
{
    //Used for rotation movement
    [SerializeField] float rotationSpeed = 0.0f;
    [SerializeField] GameObject pivot = null;

    //Used for vector movement
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 2f;
    float movementFactor; //0 not moved, 1 moved

    Vector3 startingPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pivot)
            transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        else
        {
            float cycles = Time.time / period; // grow continually from 0
            const float tau = Mathf.PI * 2;
            float rawSinWave = Mathf.Sin(cycles * tau);
            movementFactor = rawSinWave / 2f + .5f;
            Vector3 offset = movementFactor * movementVector;
            transform.position = startingPos + offset;
        }
    }
}

