using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	[SerializeField] private Transform _holdPoint;
	[SerializeField] private float _stunDuration;
	[SerializeField] private float _throwImpulse;

	private Artifact _heldArtifact;
	private float _stunnedCountdown;
	private NavMeshAgent _navAgent;
	private bool _stunned;
	private bool _canPickup = true;

	public Artifact HeldArtifact => _heldArtifact;

	private void Awake()
	{
		_navAgent = GetComponent<NavMeshAgent>();
	}

	private void OnCollisionEnter(Collision other)
	{
		// Make sure we're not already holding an artifact or we're stunned
		if (_heldArtifact != null || _stunned || !_canPickup)
			return;
		
		// If the player bumps into an artifact that isn't already picked up
		var artifact = other.collider.GetComponent<Artifact>();
		if (artifact != null && !artifact.PickedUp && !artifact.Stashed)
		{
			Debug.Log($"Picking up artifact");
			
			// Pick up the artifact
			artifact.OnPickedUp(_holdPoint);
			
			// Store the artifact we're holding onto
			_heldArtifact = artifact;
			_canPickup = false;
			
			// Alert all nearby robots
			GameManager.AlertNearbyRobotsToPlayer();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		// If the player entered the safe zone
		if (other.gameObject.CompareTag("SafeZone"))
		{
			// If the player is holding an artifact
			if (_heldArtifact)
			{
				// Drop the artifact
				_heldArtifact.Stashed = true;
				_heldArtifact.OnDropped();
				_heldArtifact = null;
				_canPickup = true;
				
				// Stash the artifact
				GameManager.OnStashArtifact();
			}
		}
	}

	public void OnStunned()
	{
		// Drop the artifact if we're holding one
		_heldArtifact?.OnDropped();
		_heldArtifact = null;
		_canPickup = false;
		
		// Start the stunned timer
		_stunnedCountdown = _stunDuration;
	}

	private void Update()
	{
		// If the player is stunned
		_stunnedCountdown -= Time.deltaTime;
		if (_stunnedCountdown > 0)
		{
			_stunned = true;
			_navAgent.ResetPath();
			_navAgent.isStopped = true;
		}
		// If the player isn't stunned any more
		else if(_stunned)
		{
			_stunned = false;
			_navAgent.isStopped = false;
			_canPickup = true;
		}
		
		// Drop the artifact when space is pressed
		if (Input.GetKeyDown(KeyCode.Space) && _heldArtifact != null)
		{
			// Throw the artifact away
			_heldArtifact.OnDropped();
			_heldArtifact.GetComponent<Rigidbody>().AddForce(transform.forward * _throwImpulse, ForceMode.Impulse);
			_heldArtifact = null;
			
			// Prevent picking up the artifact for a second
			_canPickup = false;
			Invoke(nameof(CanPickupAgain), 1f);
		}
		
		// If R is pressed, restart the level
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		
		// TODO: If the player drops the artifact in VR
			// Set no artifact as held
	}

	private void CanPickupAgain()
	{
		_canPickup = true;
	}
}
