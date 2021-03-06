using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour, Weapon
{

  private static float MAX_RATE_OF_FIRE = 20;
  private static float POWER_UP_DURATION_SECONDS = 5;

  [SerializeField]
  private float muzzleFlashDurationSecs = 0.1f;

  private int nextShootState = 1;

  [SerializeField]
  private Animator muzzleFlashAnimator;

  [SerializeField]
  private Transform bulletOrigin;

  [SerializeField]
  private float puScale = 4;

  public GameObject projectile;
  public float speed;

  [SerializeField]
  GameObject weaponUserProvider;

  private WeaponUser weaponUser;

  [SerializeField]
  [Range(1, 30)]
  private float baseRateOfFire = 5;

  [SerializeField]
  private float rateOfFire = 1;

  [SerializeField]
  private float aimVariance = 5f;

  [SerializeField]
  private float aimResetTime = 0.2f;

  [SerializeField]
  private AudioClip audioClip;

  private AudioSource audioSource;

  private float timeSinceLastShot;
  private float timeSincePowerUp = POWER_UP_DURATION_SECONDS;

  private bool shotThisFrame = false;
  private bool muzzleFlashLocked = false;

  // Use this for initialization
  void Start()
  {
    rateOfFire = baseRateOfFire;
    timeSinceLastShot = rateOfFire;
    timeSincePowerUp = POWER_UP_DURATION_SECONDS;

    weaponUser = weaponUserProvider.GetComponent<WeaponUser>();
    audioSource = gameObject.GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    timeSinceLastShot += Time.deltaTime;
    timeSincePowerUp += Time.deltaTime;
    if (timeSincePowerUp >= POWER_UP_DURATION_SECONDS)
    {
      rateOfFire = baseRateOfFire;
    }
  }

  void LateUpdate()
  {
    if (!muzzleFlashLocked)
    {
      if (shotThisFrame)
      {
        muzzleFlashAnimator.SetInteger("ShootState", nextShootState);
        nextShootState = (nextShootState == 2) ? 1 : 2;
        muzzleFlashLocked = true;
        StartCoroutine(UnlockMuzzleFlash());
        audioSource.PlayOneShot(audioClip);
      }
      else
      {
        muzzleFlashAnimator.SetInteger("ShootState", 0);
      }
    }

    shotThisFrame = false;
  }

  public void use(Vector2 direction)
  {
    if (timeSinceLastShot < (1 / rateOfFire))
    {
      return;
    }

    shotThisFrame = true;

    // Introduce variance if user is shooting too fast.
    if (aimVariance != 0 && timeSinceLastShot < aimResetTime)
    {
      float angle = Random.Range(-aimVariance, aimVariance);
      Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);
      direction = rotation * direction;
    }

    spawnBullet(direction);

    if (isPoweredUp())
    {
      Quaternion rotationA = Quaternion.AngleAxis(-15, Vector3.back);
      Quaternion rotationB = Quaternion.AngleAxis(15, Vector3.back);
      Vector3 dirA = rotationA * direction;
      Vector3 dirB = rotationB * direction;
      spawnBullet(dirA);
      spawnBullet(dirB);
    }
  }

  private IEnumerator UnlockMuzzleFlash()
  {
    yield return new WaitForSeconds(muzzleFlashDurationSecs);
    muzzleFlashLocked = false;
  }

  private void spawnBullet(Vector3 direction)
  {
    GameObject tempGo = GameObject.Instantiate(projectile, Vector3.zero, Quaternion.identity) as GameObject;
    Rigidbody2D rigidBody = tempGo.GetComponent<Rigidbody2D>();
    tempGo.transform.position = bulletOrigin.position;
    if (isPoweredUp())
    {
      tempGo.transform.localScale = new Vector3(puScale, puScale, tempGo.transform.localScale.z);
    }

    rigidBody.velocity = direction * speed;
    timeSinceLastShot = 0;
  }

  private bool isPoweredUp()
  {
    return timeSincePowerUp < POWER_UP_DURATION_SECONDS;
  }

  public void powerUp()
  {
    timeSincePowerUp = 0;
    rateOfFire = MAX_RATE_OF_FIRE;
  }

  private Vector2 getAimDirectionMouse()
  {
    Vector3 p = new Vector3();
    Camera c = Camera.main;
    Vector2 mousePos = new Vector2();

    // Get the mouse position from Event.
    // Note that the y position from Event is inverted.
    mousePos.x = Input.mousePosition.x;
    mousePos.y = Input.mousePosition.y;

    p = c.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
    Vector2 direction = p - transform.position;
    return direction.normalized;
  }
}
