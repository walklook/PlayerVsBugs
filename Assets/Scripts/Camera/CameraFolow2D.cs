using UnityEngine;
using System.Collections;

public class CameraFolow2D : MonoBehaviour
{
	public TiledMapComponent map;
    public float smoothMovement = 3.0f;
	private Vector3 topLeft;
	private Vector3 bottomRight;
	private Character2DController playerRefence = null;

	// add this function to make the camera follow the player but not out of the map.
	void UpdateCameraPosition ()
	{
		Vector3 pos = transform.position;
		// detect the edge by assigning player's position to the camera first
		transform.position = new Vector3( playerRefence.transform.position.x, playerRefence.transform.position.y, transform.position.z );
		// get the left/top edges of the map
		Vector3 pos1 = Camera.main.WorldToScreenPoint( topLeft );

		// if the camera is out of the map on the left
		if ( pos1.x >= 0 )
		{
			transform.position = new Vector3( pos.x, transform.position.y, transform.position.z ); 
		}

		// if the camera is out of the map on the top
		if ( pos1.y <= Screen.height )
		{
			transform.position = new Vector3( transform.position.x, pos.y, transform.position.z );
		}

		// get the right/bottom edges of the map
		Vector3 pos2 = Camera.main.WorldToScreenPoint( bottomRight );

		// if the camera is out of the map on the right
		if ( pos2.x <= Screen.width )
		{
			transform.position = new Vector3( pos.x, transform.position.y, transform.position.z );
		}

		// if the camera is out of the map on the bottom
		if ( pos2.y >= 0 )
		{
			transform.position = new Vector3( transform.position.x, pos.y, transform.position.z );
		}
	}

	void TryToFindPlayer ()
	{
		// Obtem a instancia do objeto do jogador
		playerRefence = FindObjectOfType<Character2DController> ();
		if(playerRefence != null) {
			// Set current position camera to player
			UpdateCameraPosition ();
		}
	}

    void Start ()
    {
		// initialize the camera's position to make sure it's not out of the map. Please don't make the camera's scope exceed the map's area.
		transform.position = new Vector3( map.TiledMap.Width * 0.5f, -map.TiledMap.Height * 0.5f, transform.position.z );

		// initialize the two points which are used to test whether the camera's scope is on the edge of camera.
		topLeft = new Vector3( 0, 0, 0 );
		bottomRight = new Vector3( map.TiledMap.Width, -map.TiledMap.Height, 0 );

        TryToFindPlayer ();
    }

    void Update ()
    {
		if(playerRefence == null) {
			TryToFindPlayer ();
			if(playerRefence == null)
			{
				return;
			}
		}

		UpdateCameraPosition();
    }
    
}