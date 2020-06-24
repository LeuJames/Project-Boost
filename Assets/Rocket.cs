using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  [SerializeField] float rcsThrust = 200f;
  [SerializeField] float mainThrust = 100f;
  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip death;
  [SerializeField] AudioClip start;


  Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
      rigidBody = GetComponent<Rigidbody>();
      audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if(state == State.Alive)
      {
        ProcessInput();
      }
    }

  private void OnCollisionEnter(Collision collision)
  {
    if ( state != State.Alive){ return; } //ignore collisions when dead
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
    rigidBody.freezeRotation = false;
  }

  private void StartDeathSequence()
  {
    state = State.Dying;
    audioSource.Stop();
    audioSource.PlayOneShot(death);
    deathParticles.Play();
    Invoke("LoadAfterDeath", 1f);
  }

  private void StartSuccessSequence()
  {
    state = State.Transcending;
    audioSource.Stop();
    audioSource.PlayOneShot(start);
    successParticles.Play();
    Invoke("LoadNextScene", 1f); //todo parameterise time
  }

  private void LoadAfterDeath()
  {
    SceneManager.LoadScene(0);
  }

  private void LoadNextScene()
  {
    SceneManager.LoadScene(1); //allow for more than 1 level
  }


  private void ProcessInput()
  {
    RespondToThrustInput();
    RespondToRotateInput();
  }

  private void RespondToRotateInput()
  {
    rigidBody.freezeRotation = true; //take manual control of rotation

    float rotationThisFrame = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
    {
      transform.Rotate(Vector3.forward * rotationThisFrame);
    }
    else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
    {
      transform.Rotate(-Vector3.forward * rotationThisFrame);
    }

    rigidBody.freezeRotation = false; // resume physics control of rotation
  }

  private void RespondToThrustInput()
  {
    if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
    {
      ApplyThrust();
    }
    else
    {
      audioSource.Stop();
      mainEngineParticles.Stop();
    }
  }

  private void ApplyThrust()
  {
    rigidBody.AddRelativeForce(Vector3.up * mainThrust);
    if (!audioSource.isPlaying)
    {
      audioSource.PlayOneShot(mainEngine);
    }
    mainEngineParticles.Play();
  }
}
