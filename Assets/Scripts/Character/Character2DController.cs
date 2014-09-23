using UnityEngine;
using System.Collections;
public enum EnumPlayerState {STAY=1,RIGHT=2,LEFT=3,DOWN=4,UP=5};

public class Character2DController : MonoBehaviour {

	public GameObject shootPrefab = null;
	public float ShootVelocity = 10.0f;
	public float timeCanShoot = 0.1f;
	public float velocity = 5.0f;
	public int numberShoots = 10;
	public int numberLife = 5;
	public float timeToBlink = 0.1f;
	public float timeMaxToBlink = 1.0f;
	public AudioClip fireSound;

	public Vector2 Direction 
	{
		get {
			return new Vector2( velocityVector.x > 0 ? 1 : -1, velocityVector.y > 0 ? 1 : -1 );
		}
	}

    private Vector3 velocityVector = Vector3.zero;
	private EnumPlayerState stateAnimation = EnumPlayerState.STAY; // 1 = Parado, 2 = Direita, 3 = Esquerda, 4 = Baixo, 5 = Cima
	private Animator scriptAnimator = null;
	private bool canShoot = true;
	private float countTime = 0.0f;

	private bool doBlink = false;
	private float countTimeBlink = 0.0f;
	private float countTimeMaxBlink = 0.0f;

	private TiledMapComponent map;

    // Call once at begin of the game
	void OnApplicationQuit() {
		PlayerPrefs.DeleteAll();
	}
	void OnLevelWasLoaded(int level) {
		PlayerPrefs.DeleteAll();
	}

	void Awake() {
        scriptAnimator = GetComponent<Animator>();
	}

	void Start() {
		map = FindObjectOfType<TiledMapComponent>();

		numberShoots = PlayerPrefs.GetInt("PlayerShoots",numberShoots);
		numberLife = PlayerPrefs.GetInt("PlayerLife",numberLife);
		// Locate the spawnPoint
		RespawnPoint[] objs = FindObjectsOfType<RespawnPoint>();
		foreach(RespawnPoint r in objs) {
			if(r.spawnPointName.Equals(PlayerPrefs.GetString("PlayerRespawn","Start"))) {
				transform.position = r.transform.position;
			}
		}
	}

	void Update() {
		if(doBlink) {
			countTimeBlink += Time.deltaTime;
			if(countTimeBlink >= timeToBlink) {
				countTimeBlink = 0.0f;
				GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
				countTimeMaxBlink += timeToBlink;
				if(countTimeMaxBlink >= timeMaxToBlink) {
					doBlink = false;
					countTimeBlink = 0.0f;
					countTimeMaxBlink = 0.0f;
					GetComponent<SpriteRenderer>().enabled = true;
				}
			}
		}
	}

    void ApplyAnimation() {
		if(scriptAnimator != null) {
			scriptAnimator.SetInteger("State",stateAnimation.GetHashCode());
		}
	}

	public void SetStateAnimation( EnumPlayerState state ) {
		stateAnimation = state;
	}
	
	public EnumPlayerState GetPlayerState() {
		return stateAnimation;	
	}

	void DecideDirectionAndShoot ()
	{
		if (((Input.GetAxis ("Horizontal") < 0.1f) && (Input.GetAxis ("Horizontal") > -0.1f)) && ((Input.GetAxis ("Vertical") < 0.1f) && (Input.GetAxis ("Vertical") > -0.1f))) {
			velocityVector = Vector3.zero;
			stateAnimation = EnumPlayerState.STAY;
			// Parado
		}
		else {
			if ((Input.GetAxis ("Horizontal") > 0.1f) && ((Input.GetAxis ("Vertical") < 0.1f) && (Input.GetAxis ("Vertical") > -0.1f))) {
				// Right direction
				stateAnimation = EnumPlayerState.RIGHT;
			}
			if ((Input.GetAxis ("Horizontal") < -0.1f) && ((Input.GetAxis ("Vertical") < 0.1f) && (Input.GetAxis ("Vertical") > -0.1f))) {
				// Left direction
				stateAnimation = EnumPlayerState.LEFT;
			}
			if ((Input.GetAxis ("Vertical") < -0.1f) && ((Input.GetAxis ("Horizontal") < 0.1f) && (Input.GetAxis ("Horizontal") > -0.1f))) {
				// Down direction
				stateAnimation = EnumPlayerState.DOWN;
			}
			if ((Input.GetAxis ("Vertical") > 0.1f) && ((Input.GetAxis ("Horizontal") < 0.1f) && (Input.GetAxis ("Horizontal") > -0.1f))) {
				// Up direction
				stateAnimation = EnumPlayerState.UP;
			}
			Vector3 vetorHorizontal = Vector3.right * Mathf.Clamp (velocity * Input.GetAxis ("Horizontal"), -velocity, velocity);
			Vector3 vetorVertical = Vector3.up * Mathf.Clamp (velocity * Input.GetAxis ("Vertical"), -velocity, velocity);
			velocityVector = vetorHorizontal + vetorVertical;
		}

		if(Input.GetButtonDown("Fire1") /*&& canShoot && numberShoots > 0*/) {
			GameObject shootClone = Instantiate(shootPrefab) as GameObject;
			shootClone.transform.position = transform.position;
			Vector2 directionShoot = new Vector2(velocityVector.x,velocityVector.y);
			if ( directionShoot.magnitude == 0 )
			{
				directionShoot = new Vector2( 0, -1 );
			}

			directionShoot.Normalize();
			shootClone.rigidbody2D.velocity = directionShoot*ShootVelocity;
			canShoot = false;
			numberShoots--;
			PlayerPrefs.SetInt("PlayerShoots",numberShoots);
			NGUITools.PlaySound(fireSound, 1, 1);
		}
	}
	
	void ApplyPlayerVelocity ()
	{
		if ( ( transform.position.x <= 0 && velocityVector.x < 0 ) || ( transform.position.x >= map.TiledMap.Width && velocityVector.x > 0 ) )
		{
			velocityVector.x = 0;
		}

		if ( ( transform.position.y >= 0 && velocityVector.y > 0 ) || ( transform.position.y <= -map.TiledMap.Height && velocityVector.y < 0 ) )
		{
			velocityVector.y = 0;
		}

		rigidbody2D.velocity = velocityVector;
	}

	void CalculateShootTime ()
	{
		if (canShoot == false) {
			countTime = countTime + Time.deltaTime;
			if (countTime >= timeCanShoot) {
				countTime = 0.0f;
				canShoot = true;
			}
		}
	}

	void FixedUpdate () {
		CalculateShootTime ();
		DecideDirectionAndShoot ();
		ApplyPlayerVelocity ();
		ApplyAnimation ();
	}

	void CheckEventTriggerCollision(GameObject other) {
		if(doBlink == false) {
			if("EnemyShoot".Equals(other.gameObject.tag)) {
				numberLife--;
				//if(numberLife<0) Application.LoadLevel("GameOver");
				doBlink = true;
	        }
		}
		if("HealDrunks_Blue".Equals(other.gameObject.tag)) {
			numberShoots += 2;
		}
		if("HealDrunks_Yellow".Equals(other.gameObject.tag)) {
			numberShoots += 4;
		}
		if("HealDrunks_Green".Equals(other.gameObject.tag)) {
			numberShoots += 8;
		}
		PlayerPrefs.SetInt("PlayerShoots",numberShoots);
		if("HealDrunks_Red".Equals(other.gameObject.tag)) {
			numberLife += 1;
		}
		PlayerPrefs.SetInt("PlayerLife",numberLife);
	}

	void OnCollisionEnter2D(Collision2D other) {
		CheckEventTriggerCollision(other.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		CheckEventTriggerCollision(other.gameObject);
	}
}