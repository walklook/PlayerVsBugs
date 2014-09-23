using UnityEngine;
using System.Collections;

public class CameraFolow2D : MonoBehaviour
{
	public TiledMapComponent map;
    public float smoothMovement = 3.0f;
	private Vector3 topLeft;
	private Vector3 bottomRight;
	private Character2DController playerRefence = null;

	void UpdateCameraPosition ()
	{
		Vector3 pos = transform.position;
		transform.position = new Vector3( playerRefence.transform.position.x, playerRefence.transform.position.y, transform.position.z );
		Vector3 pos1 = Camera.main.WorldToScreenPoint( topLeft );

		if ( pos1.x >= 0 )
		{
			transform.position = new Vector3( pos.x, transform.position.y, transform.position.z ); 
		}

		if ( pos1.y <= Screen.height )
		{
			transform.position = new Vector3( transform.position.x, pos.y, transform.position.z );
		}

		Vector3 pos2 = Camera.main.WorldToScreenPoint( bottomRight );

		if ( pos2.x <= Screen.width )
		{
			transform.position = new Vector3( pos.x, transform.position.y, transform.position.z );
		}

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
		transform.position = new Vector3( map.TiledMap.Width * 0.5f, -map.TiledMap.Height * 0.5f, transform.position.z );

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