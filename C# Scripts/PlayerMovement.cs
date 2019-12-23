using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{   
	#region VARIABLES
    //basic movement
    private CharacterController cc;
    
    [Header("Basic Movement")]
    public Camera maincam;

    public GameObject characterArt;
    public bool canMove = true, receiveInput = true, rolling;
	public float verticalVelocity = 0.0f, jogSpeed = 10.0f, runSpeed = 15.0f, walkSpeed = 5.0f, gravity = 30.0f, rollDist = 5f;
	private float speed;
	private Vector3 move = Vector3.zero;

	//variables for being grounded
	public LayerMask ground;

	//variables for slopes
	private Vector3 forward;
	
	//variables for coroutines
	public int powerWaitTime = 5;
	private WaitForSeconds longWait => new WaitForSeconds(powerWaitTime);
	private Coroutine loosePower, playGame, roll;
	#endregion

	private void OnEnable()
	{
		CounterAction += CounterActionHandler;
		StaticVars.DeathAction += DeathActionHandler;
	}

	private void OnDisable()
	{
		CounterAction -= CounterActionHandler;
		StaticVars.DeathAction -= DeathActionHandler;
		
		Reset();
	}

	private void Awake()
	{
		//locks the cursor to the center of the screen and turns it invisible
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Start()
	{
		cc = GetComponent<CharacterController>();
		speed = jogSpeed;
	}

	//input for movement
	public void MovePlayer(float moveX, float moveZ)
    {
		//checks if player is on ground
		if(IsGrounded())
		{
			verticalVelocity = 0;

			//this makes the character controller move based off the local rotation and not global
			move = transform.TransformDirection(new Vector3(moveX, -Mathf.Abs(forward.y), moveZ)) * speed;
			anim.SetFloat(StaticVars.moveX, moveX);
			anim.SetFloat(StaticVars.moveZ, moveZ);
		}
		
		//Rotates the character to follow the camera
		var eulerAngles = transform.eulerAngles;
		var angles = new Vector3(eulerAngles.x, maincam.transform.eulerAngles.y, eulerAngles.z);
		transform.rotation = Quaternion.Euler(angles);
		
		//calculates movement
		verticalVelocity -= gravity * Time.deltaTime;
		anim.SetFloat(StaticVars.moveY, verticalVelocity);
		Vector3 movement = move + verticalVelocity * Vector3.up;
		cc.Move(movement * Time.deltaTime);
	}

	//checks if the player is on the ground
	//parameters: none
	//returns: true if isGrounded, false if not
	//there was a glitch where going down/uphill made the character see itself as not grounded when using cc.isGrounded
	private bool IsGrounded()
	{
		if(cc.isGrounded)
			return true;
		
		var bottom = cc.transform.position - new Vector3(0, cc.height / 2, 0);

		//checks via raycast to see if the player is close enough to the ground to count as being grounded
		//also changes the forward vector so player doesn't bounce down slopes
		if(Physics.Raycast(bottom, -Vector3.up, out var hit, 0.2f, ground))
		{
			//calculates the forward movement direction
			forward = Vector3.Cross(transform.right, hit.normal);
			//calculates the side to side movement direction
			var strafeDir = Vector3.Cross(transform.forward, hit.normal);

			//checks to see which direction, forward or strafe, is more correct to prevent slope bouncing
			if(Mathf.Abs(strafeDir.y) > Mathf.Abs(forward.y))
			{
				forward = strafeDir;
			}
			return true;
		}
		return false;
	}
}