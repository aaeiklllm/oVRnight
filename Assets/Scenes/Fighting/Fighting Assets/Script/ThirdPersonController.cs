// using System.Collections;
// using UnityEngine;
// #if ENABLE_INPUT_SYSTEM 
// using UnityEngine.InputSystem;
// using UnityEngine.ProBuilder;
// #endif

// /* Note: animations are called via the controller for both the character and capsule using animator null checks
//  */

// namespace StarterAssets
// {
//     [RequireComponent(typeof(CharacterController))]
// #if ENABLE_INPUT_SYSTEM 
//     [RequireComponent(typeof(PlayerInput))]
// #endif
//     public class ThirdPersonController : MonoBehaviour
//     {
//         [Header("Player")]
//         [Tooltip("Move speed of the character in m/s")]
//         public float MoveSpeed = 2.0f;

//         [Tooltip("Sprint speed of the character in m/s")]
//         public float SprintSpeed = 5.335f;

//         [Tooltip("How fast the character turns to face movement direction")]
//         [Range(0.0f, 0.3f)]
//         public float RotationSmoothTime = 0.12f;

//         [Tooltip("Acceleration and deceleration")]
//         public float SpeedChangeRate = 10.0f;

//         public AudioClip LandingAudioClip;
//         public AudioClip[] FootstepAudioClips;
//         [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

//         [Space(10)]
//         [Tooltip("The height the player can jump")]
//         public float JumpHeight = 1.2f;

//         [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
//         public float Gravity = -15.0f;

//         [Space(10)]
//         [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
//         public float JumpTimeout = 0.50f;

//         [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
//         public float FallTimeout = 0.15f;

//         [Header("Player Grounded")]
//         [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
//         public bool Grounded = true;

//         [Tooltip("Useful for rough ground")]
//         public float GroundedOffset = -0.14f;

//         [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
//         public float GroundedRadius = 0.28f;

//         [Tooltip("What layers the character uses as ground")]
//         public LayerMask GroundLayers;

//         [Header("Cinemachine")]
//         [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
//         public GameObject CinemachineCameraTarget;

//         [Tooltip("How far in degrees can you move the camera up")]
//         public float TopClamp = 70.0f;

//         [Tooltip("How far in degrees can you move the camera down")]
//         public float BottomClamp = -30.0f;

//         [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
//         public float CameraAngleOverride = 0.0f;

//         [Tooltip("For locking the camera position on all axis")]
//         public bool LockCameraPosition = false;

//         // cinemachine
//         private float _cinemachineTargetYaw;
//         private float _cinemachineTargetPitch;

//         // player
//         private float _speed;
//         private float _animationBlend;
//         private float _targetRotation = 0.0f;
//         private float _rotationVelocity;
//         private float _verticalVelocity;
//         private float _terminalVelocity = 53.0f;

//         // timeout deltatime
//         private float _jumpTimeoutDelta;
//         private float _fallTimeoutDelta;

//         // animation IDs
//         private int _animIDSpeed;
//         private int _animIDGrounded;
//         private int _animIDJump;
//         private int _animIDFreeFall;
//         private int _animIDMotionSpeed;

// #if ENABLE_INPUT_SYSTEM 
//         private PlayerInput _playerInput;
// #endif
//         private Animator _animator;
//         private CharacterController _controller;
//         private StarterAssetsInputs _input;
//         private GameObject _mainCamera;

//         private const float _threshold = 0.01f;

//         private bool _hasAnimator;

//         // NEW CODE -------------------------------------------------------------------------------------------

//         [Space(10)]
//         [Header("Additions")]


//         public Renderer characterRenderer; // Reference to the character's Renderer component
//         public Material flashMaterial; // Reference to the red flash material
//         public float flashDuration = 0.2f; // Duration of the flash effect in seconds

//         private Material[] originalMaterials; // Array to store original materials of the character
//         private float flashTimer; // Timer to track the duration of the flash effect
//         private bool isFlashing; // Flag to indicate if the character is currently flashing

//         public bool isBlocking = false;

//         public GameObject dagger;
//         public ParticleSystem lightning;
//         public ParticleSystem healFX;
//         public ParticleSystem barrier;

//         public ParticleSystem slashRight;
//         public ParticleSystem slashLeft;
//         public ParticleSystem slashDown;

//         public float abilityCooldown = 1f; // Cooldown between abilities
//         public int currentAbilityIndex = 0; // Index of the current selected ability
//         public float lastAbilityTime = 0f; // Time of the last ability usage

//         public int abilitiesUnlocked = 3;

//         public int maxHealth = 25;
//         public int currentHealth = 25;

//         public int currentMana = 0; // Initial mana
//         public int maxMana = 5; // Maximum mana limit

//         private int hitCounter = 0; // Counter for tracking hits
//         public int hitsPerMana = 3; // Hits required to gain 1 mana


//         public bool canReceiveInput = true;
//         public bool inputReceived;

//         public bool isAttacking;

//         public static ThirdPersonController instance;

//         //public int slashDamage = 3;
//         //public int daggerDamage = 1;
//         //public int lightningDamage = 5;
//         public int healAmount = 5;

//         public GameObject lightningHitbox; 
        
//         // NEW CODE -------------------------------------------------------------------------------------------

//         private bool IsCurrentDeviceMouse
//         {
//             get
//             {
// #if ENABLE_INPUT_SYSTEM
//                 return _playerInput.currentControlScheme == "KeyboardMouse";
// #else
// 				return false;
// #endif
//             }
//         }


//         private void Awake()
//         {
//             // get a reference to our main camera
//             if (_mainCamera == null)
//             {
//                 _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
//             }
//             instance = this;
//         }

//         private void Start()
//         {
//             _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
//             _hasAnimator = TryGetComponent(out _animator);
//             _controller = GetComponent<CharacterController>();
//             _input = GetComponent<StarterAssetsInputs>();
// #if ENABLE_INPUT_SYSTEM 
//             _playerInput = GetComponent<PlayerInput>();
// #else
// 			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
// #endif

//             AssignAnimationIDs();

//             // reset our timeouts on start
//             _jumpTimeoutDelta = JumpTimeout;
//             _fallTimeoutDelta = FallTimeout;

//             originalMaterials = characterRenderer.materials;
//         }

//         private void Update()
//         {
//             _hasAnimator = TryGetComponent(out _animator);

//             JumpAndGravity();
//             GroundedCheck();
//             Move();
//             Combat();


//             // Check if the character is currently flashing
//             if (isFlashing)
//             {
//                 // Update the flash timer
//                 flashTimer += Time.deltaTime;

//                 // If the flash duration has elapsed, stop flashing and restore original materials
//                 if (flashTimer >= flashDuration)
//                 {
//                     StopFlash();
//                 }
//             }
//         }

//         private void LateUpdate()
//         {
//             CameraRotation();
//         }

//         private void AssignAnimationIDs()
//         {
//             _animIDSpeed = Animator.StringToHash("Speed");
//             _animIDGrounded = Animator.StringToHash("Grounded");
//             _animIDJump = Animator.StringToHash("Jump");
//             _animIDFreeFall = Animator.StringToHash("FreeFall");
//             _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

//             // NEW CODE -------------------------------------------------------------------------------------------


//             // NEW CODE -------------------------------------------------------------------------------------------
//         }

//         private void GroundedCheck()
//         {
//             // set sphere position, with offset
//             Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
//                 transform.position.z);
//             Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
//                 QueryTriggerInteraction.Ignore);

//             // update animator if using character
//             if (_hasAnimator)
//             {
//                 _animator.SetBool(_animIDGrounded, Grounded);
//             }
//         }

//         private void CameraRotation()
//         {
//             // if there is an input and camera position is not fixed
//             if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
//             {
//                 //Don't multiply mouse input by Time.deltaTime;
//                 float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

//                 _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
//                 _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;

//                 // Get the rotation of the camera around the Y axis (horizontal rotation)
//                 float cameraRotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, _mainCamera.transform.eulerAngles.y, ref _rotationVelocity,
//                     RotationSmoothTime);

//                 // Set the player's rotation to match the camera's rotation around the Y axis
//                 transform.rotation = Quaternion.Euler(0f, cameraRotationY, 0f);
//             }

//             // clamp our rotations so our values are limited 360 degrees
//             _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
//             _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

//             // Cinemachine will follow this target
//             CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
//                 _cinemachineTargetYaw, 0.0f);


            
//         }

//         private void Move()
//         {
//             // set target speed based on move speed, sprint speed and if sprint is pressed
//             float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

//             // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

//             // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
//             // if there is no input, set the target speed to 0
//             if (_input.move == Vector2.zero) targetSpeed = 0.0f;

//             // a reference to the players current horizontal velocity
//             float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

//             float speedOffset = 0.1f;
//             float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

//             // accelerate or decelerate to target speed
//             if (currentHorizontalSpeed < targetSpeed - speedOffset ||
//                 currentHorizontalSpeed > targetSpeed + speedOffset)
//             {
//                 // creates curved result rather than a linear one giving a more organic speed change
//                 // note T in Lerp is clamped, so we don't need to clamp our speed
//                 _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
//                     Time.deltaTime * SpeedChangeRate);

//                 // round speed to 3 decimal places
//                 _speed = Mathf.Round(_speed * 1000f) / 1000f;
//             }
//             else
//             {
//                 _speed = targetSpeed;
//             }

//             _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
//             if (_animationBlend < 0.01f) _animationBlend = 0f;

//             // normalise input direction
//             Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

//             // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude


//             // if there is a move input rotate player when the player is moving
//             if (_input.move != Vector2.zero)
//             {
//                 _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
//                                   _mainCamera.transform.eulerAngles.y;
//                 float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
//                     RotationSmoothTime);

//                 // rotate to face input direction relative to camera position
//                 transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
//             }
           


//             Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

//             // move the player
//             _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
//                              new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

//             // update animator if using character
//             if (_hasAnimator)
//             {
//                 _animator.SetFloat(_animIDSpeed, _animationBlend);
//                 _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
//             }
//         }

//         private void JumpAndGravity()
//         {
//             if (Grounded)
//             {
//                 // reset the fall timeout timer
//                 _fallTimeoutDelta = FallTimeout;

//                 // update animator if using character
//                 if (_hasAnimator)
//                 {
//                     _animator.SetBool(_animIDJump, false);
//                     _animator.SetBool(_animIDFreeFall, false);
//                 }

//                 // stop our velocity dropping infinitely when grounded
//                 if (_verticalVelocity < 0.0f)
//                 {
//                     _verticalVelocity = -2f;
//                 }

//                 // Jump
//                 if (_input.jump && _jumpTimeoutDelta <= 0.0f)
//                 {
//                     // the square root of H * -2 * G = how much velocity needed to reach desired height
//                     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

//                     // update animator if using character
//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool(_animIDJump, true);
//                     }
//                 }

//                 // jump timeout
//                 if (_jumpTimeoutDelta >= 0.0f)
//                 {
//                     _jumpTimeoutDelta -= Time.deltaTime;
//                 }
//             }
//             else
//             {
//                 // reset the jump timeout timer
//                 _jumpTimeoutDelta = JumpTimeout;

//                 // fall timeout
//                 if (_fallTimeoutDelta >= 0.0f)
//                 {
//                     _fallTimeoutDelta -= Time.deltaTime;
//                 }
//                 else
//                 {
//                     // update animator if using character
//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool(_animIDFreeFall, true);
//                     }
//                 }

//                 // if we are not grounded, do not jump
//                 _input.jump = false;
//             }

//             // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
//             if (_verticalVelocity < _terminalVelocity)
//             {
//                 _verticalVelocity += Gravity * Time.deltaTime;
//             }
//         }

//         // NEW CODE -------------------------------------------------------------------------------------------

//         private void Combat() {

//             if (Grounded) {

//                 if (Input.GetMouseButtonDown(0) && !isBlocking) // Left click
//                 {
//                     if (_hasAnimator)
//                     {
//                         //_animator.SetTrigger("Attack"); // Trigger attack animation
//                         Attack();
//                     }
//                 }

//                 // Block
//                 if (Input.GetMouseButtonDown(1) && !isAttacking) // Right click (pressed)
//                 {
//                     isBlocking = true;
//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool("IsBlocking", true); // Set blocking animation state
//                     }

//                     barrier.Play();
//                 }
//                 else if (Input.GetMouseButtonUp(1)) // Right click (released)
//                 {
//                     isBlocking = false;
//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool("IsBlocking", false); // Unset blocking animation state
//                     }

//                     barrier.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
//                 }

//                 // Ability
//                 if (Input.GetKeyDown(KeyCode.Q) && !isBlocking && !isAttacking && Time.time >= lastAbilityTime + abilityCooldown)
//                 {
//                     UseCurrentAbility();
//                 }

//                 // Ability swapping with scroll wheel
//                 float scrollInput = Input.GetAxis("Mouse ScrollWheel");
//                 if (scrollInput != 0f)
//                 {
//                     ChangeCurrentAbility(scrollInput);
//                 }
//             }

//         }

//         public void Attack() 
//         {
//             if (canReceiveInput)
//             {
//                 inputReceived = true;
//                 canReceiveInput = false;
//             }
//             else
//             {
//                 return;
//             }
//         }

//         public void InputManager()
//         {
//             if (!canReceiveInput)
//             {
//                 canReceiveInput = true;
//             }
//             else 
//             {
//                 canReceiveInput = false;
//             }
//         }

//         public void countHit() 
//         {
//             hitCounter++; // Increment the hit counter
//             if (hitCounter >= hitsPerMana)
//             {
//                 currentMana++; // Gain 1 mana
//                 hitCounter = 0; // Reset hit counter
//             }
//         }

//         void UseCurrentAbility()
//         {
//             // Determine animation trigger based on current ability index
//             string animationTrigger = "Ability" + (currentAbilityIndex + 1); // Assuming animation triggers are named "Ability1", "Ability2", etc.
//             Debug.Log(animationTrigger);
//             _animator.SetTrigger(animationTrigger);
//             lastAbilityTime = Time.time;

//             if (currentAbilityIndex == 0)
//             {
//                 // Dagger
//                 Vector3 spawnOffset = transform.forward * 1f + transform.up * 1.3f;
//                 Vector3 daggerSpawn = transform.position + spawnOffset;

//                 GameObject projectile = Instantiate(dagger, daggerSpawn, _mainCamera.transform.rotation);
//                 Rigidbody rb = projectile.GetComponent<Rigidbody>();
//                 rb.AddForce(projectile.transform.forward * 20f, ForceMode.Impulse);

//             }
//             else if (currentAbilityIndex == 1 && currentMana > 0)
//             {
//                 // Heal 
//                 _animator.SetTrigger("Cast");
//                 healFX.Play(true);

//                 currentMana--;

//                 // Calculate the potential new health after healing
//                 int newHealth = currentHealth + healAmount;

//                 // Check if the new health exceeds the maximum health limit
//                 if (newHealth > maxHealth)
//                 {
//                     // Calculate the amount of healing that respects the health limit
//                     int overflow = newHealth - maxHealth;
//                     healAmount -= overflow;
//                     newHealth = maxHealth;
//                 }

//                 // Update the character's current health
//                 currentHealth = newHealth;
//             }
//             else if (currentAbilityIndex == 2 && currentMana > 0)
//             {
//                 currentMana --;

//                 // Lightning Burst Attack (Damage around large area, deals high damage)
//                 Debug.Log("blast!");
//                 _animator.SetTrigger("Cast");
//                 lightning.Play(true);

//                 GameObject blastHitbox = Instantiate(lightningHitbox, transform.position, Quaternion.identity);
//                 Destroy(blastHitbox, 0.25f);
//             }
            
//         }

//         public void takeDamage(int damageAmount) 
//         {
//             if (!isBlocking)
//             {
//                 currentHealth -= damageAmount;

//                 Debug.Log("HP: " + currentHealth);

//                 StartFlash();

//                 StartFlash();
//                 Debug.Log("Character took damage. Current Health: " + currentHealth);
//                 if (currentHealth <= 0)
//                 {
//                     // Defeat
//                     Debug.Log("Defeated.");
//                 }
//             }
//             else
//             {
//                 // PLAY BLOCK SFX
//                 Debug.Log("Blocked");
//             }
//         }

//         // Method to start the red flash effect
//         private void StartFlash()
//         {
//             // Create a new array to hold modified materials (copies of original materials)
//             Material[] modifiedMaterials = new Material[originalMaterials.Length];
//             for (int i = 0; i < originalMaterials.Length; i++)
//             {
//                 // Create a copy of each original material
//                 modifiedMaterials[i] = new Material(originalMaterials[i]);
//             }

//             // Assign the flashMaterial to all modified materials
//             for (int i = 0; i < modifiedMaterials.Length; i++)
//             {
//                 modifiedMaterials[i].color = Color.red;
//             }

//             // Assign the modified materials to the character's Renderer component
//             characterRenderer.materials = modifiedMaterials;

//             // Initialize the flash timer and set the flashing flag
//             flashTimer = 0f;
//             isFlashing = true;
//         }

//         // Method to stop the red flash effect and restore original materials
//         private void StopFlash()
//         {
//             // Restore the original materials
//             characterRenderer.materials = originalMaterials;

//             // Reset the flashing flag
//             isFlashing = false;
//         }

//         void ChangeCurrentAbility(float scrollInput)
//         {
//             currentAbilityIndex = (currentAbilityIndex + (scrollInput < 0f ? 1 : -1) + abilitiesUnlocked) % abilitiesUnlocked;
//             Debug.Log(currentAbilityIndex);
//         }

//         // NEW CODE -------------------------------------------------------------------------------------------

//         private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
//         {
//             if (lfAngle < -360f) lfAngle += 360f;
//             if (lfAngle > 360f) lfAngle -= 360f;
//             return Mathf.Clamp(lfAngle, lfMin, lfMax);
//         }

//         private void OnDrawGizmosSelected()
//         {
//             Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
//             Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

//             if (Grounded) Gizmos.color = transparentGreen;
//             else Gizmos.color = transparentRed;

//             // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
//             Gizmos.DrawSphere(
//                 new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
//                 GroundedRadius);
//         }

//         private void OnFootstep(AnimationEvent animationEvent)
//         {
//             if (animationEvent.animatorClipInfo.weight > 0.5f)
//             {
//                 if (FootstepAudioClips.Length > 0)
//                 {
//                     var index = Random.Range(0, FootstepAudioClips.Length);
//                     AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
//                 }
//             }
//         }

//         private void OnLand(AnimationEvent animationEvent)
//         {
//             if (animationEvent.animatorClipInfo.weight > 0.5f)
//             {
//                 AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
//             }
//         }
//     }
// }