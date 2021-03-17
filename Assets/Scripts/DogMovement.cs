using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovement : MonoBehaviour
{

    public KeyCode Forward;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Bark;

    public float rotationSpeed;
    private float speed = 0f;
    public float maxSpeed;
    public float acceleration;
    public float barkTimer;
    public AudioSource barkSound;
    public Settings settings;


    private float timer = 0.0f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Move Forward
        if (Input.GetKey(Forward))
        {
            Acelerar(true);
        }
        else
        {
            Acelerar(false);
        }
        //RotateLeft
        if (Input.GetKey(Left))
        {
            RotateLeft(true);
        }

        //RotateRight
        if (Input.GetKey(Right))
        {
            RotateLeft(false);
        }

        //Al presionar tecla y si ha pasado un tiempo, ladrar.
        if (Input.GetKeyDown(Bark) & timer<=0f)
        {
            settings.distanciaPerro = 2.5f;

            //sonido, incrementar radio etc.
            timer = barkTimer;
            barkSound.Play();
        }
        
        if (timer / 2 <= 0f)
            settings.distanciaPerro = 1f;
        //Incrementar temporizador
        timer -= Time.fixedDeltaTime;
    }

    private void Acelerar(bool accelerate)
    {
        if (accelerate)
        {
            speed += acceleration * Time.fixedDeltaTime;
            speed = Mathf.Min(speed, maxSpeed); //cuando acelere al maximo, coger maxima velocidad.
        }
        else {
            speed -= acceleration * Time.fixedDeltaTime;
            speed = Mathf.Max(speed, 0); //cuando baje de 0, coger 0 velocidad.
        }
        rb.velocity = transform.forward * speed;
    }

    private void RotateLeft(bool rotate)
    {
        //rotar izquierda
        if (rotate)
        {
            //rb.angularVelocity = new Vector3(0, -rotationSpeed, 0);
            transform.Rotate(- transform.up * rotationSpeed * Time.fixedDeltaTime);
        }
        else //rotar derecha
        {
            transform.Rotate(transform.up * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
