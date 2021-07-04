using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    private CharacterController controller;
    [HideInInspector] public Vector3 playerVelocity;
    [HideInInspector] public bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    private float origSpeed;
    public float crouchSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float spinRadius;
    [SerializeField] private float damageBoost;
    [SerializeField] private float spinTime = 0.5f;
    [SerializeField] private float slideTime = 1.0f;
    [SerializeField] private float slideSpeedFactor = 2.0f;
    [SerializeField] private float poundSpeed;
    private float inputX, inputZ;
    private bool damageable = true;
    // anims controllers
    [HideInInspector] public bool jumping, sliding, crouching, holdC, pounding, flippable, spinning, jumpAfterPounding;
    private int numberOfJumps;
    public int health = 1;
    [SerializeField] private AudioSource akuExit, spin, slide;
    [SerializeField] private CapsuleCollider spinCollider;
    [SerializeField] private GameObject model;
    private Vector3 lastPosition, currentPosition;
    public float realVelocity;
    public bool raycast;
    private RaycastHit hit;
    private float crouchCheckDistanceUp = 1.0f;

    public GameObject mask, maskIndicator1, maskIndicator2, allHellBreaksLoose;
    public GameObject crashNow, crashSpinning;

    public CinemachineVirtualCamera cinemachine;
    public Vector3 offset;
    [HideInInspector] public Vector3 respawnPos;

    private void Start()
    {
        origSpeed = playerSpeed;
        controller = GetComponent<CharacterController>();
        respawnPos = transform.position;
    }

    void Update()
    {
        raycast = Physics.Raycast(transform.position, Vector3.up, out hit, crouchCheckDistanceUp);

        FixBrokenCharacterController();

        if (!sliding || (sliding && jumping))
        {
            if (!pounding)
            {
                inputX = Input.GetAxis("Horizontal");
                inputZ = Input.GetAxis("Vertical");
            }
        }

        Vector3 move = new Vector3(inputX, 0, inputZ);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && !raycast && !pounding)
        {
            if (groundedPlayer)
            {
                playerVelocity.y = 0;
                if (!jumpAfterPounding)
                {
                    Jump();
                }
                else
                {
                    Jump(10);
                    print("did it");
                    jumpAfterPounding = false;
                }
                controller.slopeLimit = 90;
            }
            else
            {
                if(numberOfJumps <= 1)
                {
                    playerVelocity.y = 0;
                    Jump();
                    controller.slopeLimit = 90;
                }
            }

            if(currentPosition.y < lastPosition.y)
            {
                flippable = true;
            }

            if (flippable)
            {
                //print("flip me one");
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        HandleMaskStates();

        if (Input.GetButtonDown("Fire1"))
        {
            spin.Play();
            spinning = true;
            StartCoroutine(StopSpinning());
        }
        if (Input.GetButtonDown("Fire2") && !sliding && (inputX != 0 || inputZ != 0) && !crouching && !jumping)
        {
            if (controller.isGrounded)
            {
                inputX = Input.GetAxisRaw("Horizontal");
                inputZ = Input.GetAxisRaw("Vertical");
                slide.Play();
                StartCoroutine(SlideForward());
            }
        }

        if (transform.parent == null)
        {
            realVelocity = (Vector3.Distance(lastPosition, currentPosition) / Time.deltaTime) * 100f;
        }

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !groundedPlayer)
        {
            flippable = currentPosition.y > lastPosition.y;
        }

        lastPosition = currentPosition;

        if (Input.GetKeyDown(KeyCode.C))
        {
            holdC = true;
            if (controller.isGrounded)
            {
                crouching = true;
            }
            else
            {
                // ground pound
                pounding = true;
            }
        }
        if ((Input.GetKeyUp(KeyCode.C)))
        {
            holdC = false;
            if (!raycast)
            {
                crouching = false;
            }
        }

        if(!holdC && !raycast)
        {
            crouching = false;
        }

        if (crouching)
        {
            playerSpeed = crouchSpeed;
            controller.height = 1;
            controller.center = new Vector3(0, -0.5f, 0);
        }
        else
        {
            if(!jumping && !sliding)
            {
                playerSpeed = origSpeed;
                controller.height = 2;
                controller.center = Vector3.zero;
            }
        }

        if (pounding) 
        {
            playerSpeed = 0.1f;
            StartCoroutine(StopPounding());
        }

        Debug.DrawRay(transform.position, Vector3.up, Color.cyan, crouchCheckDistanceUp);

        if (sliding)
        {
            //Spin();
        }

        if(transform.position.y < -1)
        {
            health = 0;
        }
    }

    private void LateUpdate()
    {
        cinemachine.transform.position = transform.position + offset;
    }

    private IEnumerator StopPounding()
    {
        float timer = 0;
        if (controller.isGrounded && pounding)
        {
            timer += Time.deltaTime;

            jumpAfterPounding = timer <= 1f;

            flippable = false;
            jumping = false;
            yield return new WaitForSeconds(0.3f);
            pounding = false;
        }
        else
        {
            playerVelocity.y = -poundSpeed;
        }

        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator SlideForward()
    {
        sliding = true;
        if (!jumping)
        {
            playerSpeed *= slideSpeedFactor;
        }

        controller.height = 1;
        controller.center = new Vector3(0, -0.5f, 0);

        yield return new WaitForSeconds(slideTime);

        sliding = false;
        if (!raycast)
        {
            controller.height = 2;
            controller.center = Vector3.zero;
        }
        else
        {
            crouching = true;
        }
    }

    IEnumerator StopSpinning()
    {
        crashNow.SetActive(false);
        crashSpinning.SetActive(true);
        yield return new WaitForSeconds(spinTime);
        spinning = false;
        crashNow.SetActive(true);
        crashSpinning.SetActive(false);
    }

    void FixBrokenCharacterController()
    {
        currentPosition = transform.position;
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y <= 0)
        {
            playerVelocity.y = -0.1f;
        }

        if (groundedPlayer)
        {
            controller.slopeLimit = 45;
            jumping = false;
            numberOfJumps = 0;
            flippable = false;
        }
    }

    public void Jump()
    {
        controller.slopeLimit = 90;
        jumping = true;
        numberOfJumps += 1; 
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * (gravityValue));
        if (sliding)
        {
            playerSpeed = origSpeed;
            playerVelocity.y *= 1.3f;
        }
    }

    public void Jump(float value)
    {
        controller.slopeLimit = 90;
        jumping = true;
        numberOfJumps += 1;
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -value * (gravityValue));
        if (sliding)
        {
            playerSpeed = origSpeed;
            playerVelocity *= 1.3f;
        }
    }

    public void DamagePlayer()
    {
        if (damageable)
        {
            if(health > 1)
            {
                akuExit.Play();
            }
            health -= 1;
            damageable = false;
            StartCoroutine(DamageBoost());
        }
    }

    IEnumerator DamageBoost()
    {
        yield return new WaitForSeconds(damageBoost);
        damageable = true;
    }

    void HandleMaskStates()
    {
        switch (health)
        {
            case 1:
                mask.SetActive(false);
                break;
            case 2:
                mask.SetActive(true);
                maskIndicator1.SetActive(true);
                maskIndicator2.SetActive(false);
                allHellBreaksLoose.SetActive(false);
                break;
            case 3:
                maskIndicator1.SetActive(false);
                maskIndicator2.SetActive(true);
                allHellBreaksLoose.SetActive(false);
                break;
            case 4:
                maskIndicator1.SetActive(false);
                maskIndicator2.SetActive(false);
                allHellBreaksLoose.SetActive(true);
                break;
            default:
                //print("game over");
                break;
        }
    }
}