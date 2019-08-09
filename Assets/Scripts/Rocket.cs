using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;
    Text deathText;
    [SerializeField] float rcsThrust = 2f;
    [SerializeField] float mainThrust = 2f;
    [SerializeField] AudioClip mainEngine = null;
    [SerializeField] AudioClip finish = null;
    [SerializeField] AudioClip dead = null;
    [SerializeField] ParticleSystem mainParticle = null;
    [SerializeField] ParticleSystem finishParticle = null;
    [SerializeField] ParticleSystem deadParticle = null;
    enum State { Alive, Dying, Transcending}
    State state = State.Alive;
    bool collisionDisabled = true;
    private void Thrust()
    {
        
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(mainEngine);
            mainParticle.Play();
        }
        else
        {
            audioSource.Stop();
            mainParticle.Stop();
        }
        mainParticle.Play();
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
       
        float rotationspeed = rcsThrust + Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        { 
            transform.Rotate(Vector3.forward * rotationspeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationspeed);
        }
        rigidBody.freezeRotation = false;
    }

    void OnCollisionEnter(Collision collision) {

        if (state != State.Alive || !collisionDisabled) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                print("Friendly");
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(finish);
                finishParticle.Play();
                Invoke("loadNext", 1f);
                print("Finish");
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(dead);
                deadParticle.Play();
                int hi = PlayerPrefs.GetInt("score") + 1; //save death counter
                PlayerPrefs.SetInt("score", hi);
                Invoke("loadStart", 1f);
                print("Dead");
                break;
        }
    }

    private void loadNext() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings){
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void loadStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //0
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        deathText = GameObject.Find("Canvas").GetComponentInChildren<Text>();
        deathText.GetComponent<Text>().text = PlayerPrefs.GetInt("score").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        //RespondToDebugKeys();
        if (Input.GetKey("escape"))
        {
            
            Application.Quit();
        }
    }
    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) loadNext();
        else if (Input.GetKeyDown(KeyCode.C)) collisionDisabled = !collisionDisabled;
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}

