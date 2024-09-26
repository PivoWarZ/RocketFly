using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RocketController : MonoBehaviour
{
    [SerializeField] float rootSpeed = 100f; // коэффициент скорости вращения
    [SerializeField] float flightSpeed = 100f;
    [SerializeField] AudioClip flyRocket;
    [SerializeField] AudioClip finishRocket;
    [SerializeField] AudioClip boomRocketFirst;
    [SerializeField] AudioClip boomRocketSecond;
    [SerializeField] ParticleSystem finishParticle;
    [SerializeField] ParticleSystem flyParticle;
    [SerializeField] ParticleSystem boomParticle;

    Rigidbody rocketRigidBody;
    AudioSource audioSource;


    [SerializeField] float CurrentRocketRotation;

    int Currentlevel = 0; //текущий уровень 
    enum PlayerStatus
    { 
        playing,
        finish,
        dead
    }

    PlayerStatus CurrentPlayerStatus;

    // Start is called before the first frame update
    void Start()
    {
        rocketRigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        CurrentPlayerStatus = PlayerStatus.playing;
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentPlayerStatus == PlayerStatus.playing)
        {
            RocketFlight();
            RocketRotation();
        }
        CurrentRocketRotation = rocketRigidBody.transform.localRotation.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        { 
            case "Finish":
                {
                    FinishTrigger();
                }
                break;
            case "Friendly":
                {
                    print("это дгуг, всё хорошо"); 
                }
                break;
            case "Battery":
                {
                    print("энергетик выпил");
                }
                break;
            default:
                {

                    Boom();
                    
                }
                break;
        }
                
    }
    void Boom()
    {
        print("взрыв");
        audioSource.Stop();
        audioSource.PlayOneShot(boomRocketFirst);
        CurrentPlayerStatus = PlayerStatus.dead;
        boomParticle.Play();
        Invoke("LoadFirstLevel", 3f);
    }
    void FinishTrigger()
    {
        //Финиш
        audioSource.Stop();
        if (CurrentPlayerStatus == PlayerStatus.playing &&
            Mathf.Abs(CurrentRocketRotation) < 0.15)
        {
            audioSource.PlayOneShot(finishRocket);
            Invoke("LoadNextLevel", 3f);
            finishParticle.Play();
            CurrentPlayerStatus = PlayerStatus.finish;
        }
        else //взрыв Boom
        {
            
            CurrentPlayerStatus = PlayerStatus.dead;
            audioSource.Stop();
            flyParticle.Stop();
            boomParticle.Play();
        }
    }
    void LoadNextLevel ()
    {
        Currentlevel += 1;
        SceneManager.LoadScene(Currentlevel);
    }
    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    void RocketFlight()
    {
        

        if (Input.GetKey(KeyCode.Space))
        {
            rocketRigidBody.AddRelativeForce(Vector3.up * flightSpeed);

            if (flyParticle.isStopped)
            {
                flyParticle.Play();
            }

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(flyRocket);
            }

        }
        else  
        {
            audioSource.Pause();
            flyParticle.Stop();
        }

        
    }
    void RocketRotation()
    {
        
        float rotationSpeed = rootSpeed * Time.deltaTime;

        rocketRigidBody.freezeRotation = true;

        if (CurrentPlayerStatus == PlayerStatus.playing)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.forward * rotationSpeed);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(-Vector3.forward * rotationSpeed);
            }
        }
        
        rocketRigidBody.freezeRotation = false;
        
    }
}