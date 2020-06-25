using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  [SerializeField] float rcsThrust = 100f;
  [SerializeField] float mainThrust = 1000f;
  [SerializeField] float levelLoadDelay = 2f;
  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip death;
  [SerializeField] AudioClip success;

  [SerializeField] ParticleSystem mainEngineParticles;
  [SerializeField] ParticleSystem deathParticles;
  [SerializeField] ParticleSystem successParticles;

  Rigidbody rigidBody;
  AudioSource audioSource;
  bool collisionsDisabled = false;
  bool isTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
      rigidBody = GetComponent<Rigidbody>();
      audioSource = GetComponent<AudioSource>();
    }

  // Update is called once per frame
  void Update()
  {
    if (!isTransitioning)
    {
      ProcessInput();
      RespondToLevelShortcut();
    }
    //only if debug on
    if (Debug.isDebugBuild)
    {
      RespondToDebug();
    }
  }

  private void RespondToDebug()
  {
    if (Input.GetKeyDown(KeyCode.C)) //toggle collision on & off
    {
      collisionsDisabled = !collisionsDisabled; //simple toggle on/off
    }
  }

  private void RespondToLevelShortcut()
  {
    if (Input.GetKeyDown(KeyCode.L))
    {
      LoadNextScene();
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if ( isTransitioning || collisionsDisabled){ return; } //ignore collisions when dead
    switch(collision.gameObject.tag)
    {
      case "Friendly":
        print("ok");
        break;
      case "Finish":
        StartSuccessSequence();
        break;
      default:
        StartDeathSequence();
        break;
    }
  }

  private void StartDeathSequence()
  {
    isTransitioning = true;
    audioSource.Stop();
    audioSource.PlayOneShot(death);
    deathParticles.Play();
    Invoke("LoadFirstLevel", levelLoadDelay);
  }

  private void StartSuccessSequence()
  {
    isTransitioning = true;
    audioSource.Stop();
    audioSource.PlayOneShot(success);
    successParticles.Play();
    Invoke("LoadNextScene", levelLoadDelay);
  }

  private void LoadFirstLevel()
  {
    SceneManager.LoadScene(0);
  }

  private void LoadNextScene()
  {
    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    if(currentSceneIndex != SceneManager.sceneCountInBuildSettings -1)
    {
      SceneManager.LoadScene(currentSceneIndex +1);
    }
    else
    {
      SceneManager.LoadScene(0);
    }
  }


  private void ProcessInput()
  {
    RespondToThrustInput();
    RespondToRotateInput();
  }

  private void RespondToRotateInput()
  {
    rigidBody.angularVelocity = Vector3.zero; //take manual control of rotation

    float rotationThisFrame = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
    {
      transform.Rotate(Vector3.forward * rotationThisFrame);
    }
    else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
    {
      transform.Rotate(-Vector3.forward * rotationThisFrame);
    }

  }

  private void RespondToThrustInput()
  {
    if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
    {
      ApplyThrust();
    }
    else
    {
      StopApplyingThrust();
    }
  }

  private void StopApplyingThrust()
  {
    audioSource.Stop();
    mainEngineParticles.Stop();
  }

  private void ApplyThrust()
  {
    rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
    if (!audioSource.isPlaying)
    {
      audioSource.PlayOneShot(mainEngine);
      mainEngineParticles.Play();
    }
  }
}
