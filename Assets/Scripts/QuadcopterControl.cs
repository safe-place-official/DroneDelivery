using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;

public class QuadcopterControl : MonoBehaviour
{
    [Header("Screws")]
    public GameObject PP; private float PPcurrentRotationSpeed = 0f;
    public GameObject PL; private float PLcurrentRotationSpeed = 0f;
    public GameObject ZP; private float ZPcurrentRotationSpeed = 0f;
    public GameObject ZL; private float ZLcurrentRotationSpeed = 0f;

    [Header("Forces")]
    public float baseLiftForce = 10f; // Базовая подъемная сила
    public float baseDescentForce = 5f; // Базовая сила снижения
    public float baseMoveForce = 5f; // Базовая сила движения
    public float maxHeight = 100f;
    public float baseTiltSpeed = 5f; // Базовая скорость наклона
    public float maxTiltAngle = 20f;
    public float baseRotationSpeed = 100f; // Базовая скорость вращения
    public float basePropellerRotationSpeed = 5f; // Базовая скорость вращения винтов
    public float decelerationRate = 1f;
    public float acceleration = 8f;
    public float deceleration = 12f;
    private float currentForwardSpeed;
    private float currentLateralSpeed;

    [Header("Movement Physics")]
    public float baseMaxSpeed = 15f; // Базовая максимальная скорость
    public float forwardAccel = 12f;
    public float backwardAccel = 16f;
    public float lateralAccel = 10f;
    public float brakeForce = 20f;
    [Range(0.1f, 0.5f)]
    public float brakeResponseTime = 0.2f;

    // Актуальные значения с учетом улучшений
    private float liftForce { get { return baseLiftForce * GetEngineMultiplier(); } }
    private float descentForce { get { return baseDescentForce * GetEngineMultiplier(); } }
    private float moveForce { get { return baseMoveForce * GetEngineMultiplier(); } }
    private float tiltSpeed { get { return baseTiltSpeed * GetScrewMultiplier(); } }
    private float rotationSpeed { get { return baseRotationSpeed * GetScrewMultiplier(); } }
    private float propellerRotationSpeed { get { return basePropellerRotationSpeed * GetScrewMultiplier(); } }
    private float maxSpeed { get { return baseMaxSpeed * GetEngineMultiplier(); } }

    [Header("Energy Settings")]
    public float Energy = 100f;
    public float NextUpdateTime;
    public float UpdateTime;
    public RectTransform EnergyPanel;
    public RectTransform FillEnergyPanel;
    public Image FillEnergyImage;
    public bool isOnEngine = false;

    [Header("RigidBody")]
    public Rigidbody rb;
    public bool isGround = true;

    [Header("GamePad")]
    public float Sensitivity = 0.1f;
    float rightStickHorizontal;
    float rightStickVertical;
    float leftStickHorizontal;
    float leftStickVertical;

    [Header("Phone")]
    public GameObject MobilePhoneCanvas;
    public Joystick leftJoystick;
    public Joystick rightJoystick;

    [Header("SignalSettings")]
    public bool isSignal = true;
    public GameObject LostSignal;
    public GameObject SignalThresholdText;

    bool isSpeedBonusActive = false;
    public SmoothTextChange moneyTextChanger;
    public Transform SignalsBarriers;

    public AudioSource SoundEngine;
    private float currentVolume = 0f;


    // Множители для улучшений
    private float GetScrewMultiplier()
    {
        int screwLevel = PlayerPrefs.GetInt("Level_screwUpgrade", 1);
        // Улучшение винтов дает прирост скорости вращения и маневренности
        return 1f + (screwLevel - 1) * 0.15f; // +15% за уровень (макс +30% на 3 уровне)
    }

    private float GetEngineMultiplier()
    {
        int engineLevel = PlayerPrefs.GetInt("Level_engineUpgrade", 1);
        // Улучшение двигателя дает прирост мощности и максимальной скорости
        return 1f + (engineLevel - 1) * 0.2f; // +20% за уровень (макс +80% на 5 уровне)
    }

    private void Start()
    {
        switch (PlayerPrefs.GetInt("Level_antennaUpgrade", 1))
        {
            case 1:
                SignalsBarriers.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                break;
            case 2:
                SignalsBarriers.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                break;
            case 3:
                SignalsBarriers.localScale = new Vector3(1f, 1f, 1f);
                break;
            default:
                SignalsBarriers.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                break;
        }

        Time.timeScale = 1f;
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 90;
        DeathScreen.isDead = false;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame))
        {
            onClickEngineButton();
        }
    }

    void FixedUpdate()
    {
        UpdateEnergy();

        HandlePropellerRotation();
        HandleMovement();

        MobilePhoneCanvas.SetActive(Application.isMobilePlatform && Gamepad.current == null);


        if (isOnEngine && !SoundEngine.isPlaying)
            SoundEngine.Play();
        else if (!isOnEngine)
            SoundEngine.Stop();
    }

    void UpdateEnergy()
    {
        if (Energy > 0)
        {
            if (isOnEngine && isSignal && isGround)
            {
                Energy -= 0.0075f / PlayerPrefs.GetInt("Level_batteryUpgrade", 1);
            }

            else if (isOnEngine)
            {
                Energy -= rb.linearVelocity.magnitude / (4000 * PlayerPrefs.GetInt("Level_batteryUpgrade", 1));
            }
        }

        else
        {
            isOnEngine = false;
            DeathScreen.isDead = true;
        }

        if (NewMonoBehaviourScript.IsCharging)
        {
            Energy += 6 * Time.deltaTime;
            Energy = Mathf.Clamp(Energy, 0, 100);
        }

        FillEnergyPanel.sizeDelta = new Vector2(EnergyPanel.sizeDelta.x, EnergyPanel.sizeDelta.y / 100 * Energy);
        FillEnergyImage.color = Color.HSVToRGB(Mathf.Lerp(0f, 0.33f, Energy / 100f), 1, 1);

        var gamepad = Gamepad.current;
        if (gamepad != null && gamepad is DualShockGamepad ds4)
        {
            ds4.SetLightBarColor(Color.HSVToRGB(Mathf.Lerp(0f, 0.33f, Energy / 100f), 1, 1));
        }
    }

    void ChangePropellerRotation(float PPspeed, float PLspeed, float ZPspeed, float ZLspeed)
    {
        PPcurrentRotationSpeed = Mathf.Lerp(PPcurrentRotationSpeed, propellerRotationSpeed * PPspeed, Time.deltaTime * 2f);
        PLcurrentRotationSpeed = Mathf.Lerp(PLcurrentRotationSpeed, propellerRotationSpeed * PLspeed, Time.deltaTime * 2f);
        ZPcurrentRotationSpeed = Mathf.Lerp(ZPcurrentRotationSpeed, propellerRotationSpeed * ZPspeed, Time.deltaTime * 2f);
        ZLcurrentRotationSpeed = Mathf.Lerp(ZLcurrentRotationSpeed, propellerRotationSpeed * ZLspeed, Time.deltaTime * 2f);

        float maxSpeed = propellerRotationSpeed * 2f; // Учитываем, что множитель может быть 2
        float average = (PPcurrentRotationSpeed + PLcurrentRotationSpeed + ZPcurrentRotationSpeed + ZLcurrentRotationSpeed) / 4f;
        float normalized = Mathf.InverseLerp(0f, maxSpeed, average);
        float targetVolume = normalized * PlayerPrefs.GetFloat("SoundsVolume", 1f);

        // Плавное приближение громкости
        float smoothing = 2f;
        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, Time.deltaTime * smoothing);
        SoundEngine.volume = currentVolume;
    }


    void HandlePropellerRotation()
    {
        if (isOnEngine && isSignal)
        {
            var gamepad = Gamepad.current;

            if (Application.isMobilePlatform && Gamepad.current == null)
            {
                rightStickHorizontal = rightJoystick.xAx;
                rightStickVertical = rightJoystick.yAx;
                leftStickHorizontal = leftJoystick.xAx;
                leftStickVertical = leftJoystick.yAx;
            }

            else if (Gamepad.current != null)
            {
                Vector2 leftStickInput = gamepad.leftStick.value;
                leftStickHorizontal = leftStickInput.x;
                leftStickVertical = leftStickInput.y;

                Vector2 rightStickInput = gamepad.rightStick.value;
                rightStickHorizontal = rightStickInput.x;
                rightStickVertical = rightStickInput.y;
            }

            if (Keyboard.current.wKey.isPressed || rightStickVertical > Sensitivity) ChangePropellerRotation(2f, 2f, 1f, 1f);

            else if (Keyboard.current.sKey.isPressed || rightStickVertical < -Sensitivity) ChangePropellerRotation(1f, 1f, 2f, 2f);

            else if (Keyboard.current.dKey.isPressed || rightStickHorizontal > Sensitivity) ChangePropellerRotation(2f, 1f, 2f, 1f);

            else if (Keyboard.current.aKey.isPressed || rightStickHorizontal < -Sensitivity) ChangePropellerRotation(1f, 2f, 1f, 2f);

            else if (Keyboard.current.cKey.isPressed || leftStickHorizontal > Sensitivity) ChangePropellerRotation(1f, 2f, 2f, 1f);

            else if (Keyboard.current.zKey.isPressed || leftStickHorizontal < -Sensitivity) ChangePropellerRotation(2f, 1f, 1f, 2f);

            else if (Keyboard.current.spaceKey.isPressed || leftStickVertical > Sensitivity) ChangePropellerRotation(1.5f, 1.5f, 1.5f, 1.5f);

            else ChangePropellerRotation(1f, 1f, 1f, 1f);
        }

        else ChangePropellerRotation(0f, 0f, 0f, 0f);

        PP.transform.Rotate(0, 0, PPcurrentRotationSpeed * Time.deltaTime * 200f);
        PL.transform.Rotate(0, 0, -PLcurrentRotationSpeed * Time.deltaTime * 200f);
        ZP.transform.Rotate(0, 0, -ZPcurrentRotationSpeed * Time.deltaTime * 200f);
        ZL.transform.Rotate(0, 0, ZLcurrentRotationSpeed * Time.deltaTime * 200f);
    }

    void HandleMovement()
    {
        if (isOnEngine && isSignal)
        {
            if (Keyboard.current.spaceKey.isPressed || leftStickVertical > Sensitivity)
            {
                Lift();
            }

            if (((Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed) || (leftStickVertical < -Sensitivity)) && !isGround)
            {
                Descend();
            }

            if (!isGround)
            {
                Move();
                Rotate();
            }

            if ((!Keyboard.current.wKey.isPressed && !Keyboard.current.sKey.isPressed && !Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed) || ((rightStickHorizontal < Sensitivity || rightStickHorizontal > -Sensitivity) && (leftStickHorizontal < Sensitivity || leftStickHorizontal > -Sensitivity) && (rightStickVertical < Sensitivity || rightStickVertical > -Sensitivity) && (leftStickVertical < Sensitivity || leftStickVertical > -Sensitivity)))
            {
                Tilt(0, transform.rotation.eulerAngles.y, 0);
            }
        }
    }

    void Lift()
    {
        if (transform.position.y < maxHeight)
        {
            rb.AddForce(Vector3.up * liftForce * Time.deltaTime * 250f, ForceMode.Acceleration);
        }

        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    void Descend()
    {
        if (!isGround)
        {
            rb.AddForce(Vector3.down * descentForce * Time.deltaTime * 250f, ForceMode.Acceleration);
        }

        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    void Move()
    {
        Vector3 moveDirection = Vector3.zero;
        float targetForwardSpeed = 0f;
        float targetLateralSpeed = 0f;

        // Обработка ввода для движения вперед/назад
        if (Keyboard.current.wKey.isPressed || rightStickVertical > Sensitivity)
        {
            targetForwardSpeed = 1f;
            Tilt(maxTiltAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (Keyboard.current.sKey.isPressed || rightStickVertical < -Sensitivity)
        {
            targetForwardSpeed = -1f;
            Tilt(-maxTiltAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        // Обработка ввода для движения влево/вправо
        if (Keyboard.current.dKey.isPressed || rightStickHorizontal > Sensitivity)
        {
            targetLateralSpeed = 1f;
            Tilt(transform.eulerAngles.x, transform.eulerAngles.y, -maxTiltAngle);
        }
        else if (Keyboard.current.aKey.isPressed || rightStickHorizontal < -Sensitivity)
        {
            targetLateralSpeed = -1f;
            Tilt(transform.eulerAngles.x, transform.eulerAngles.y, maxTiltAngle);
        }

        // Плавное изменение скоростей
        currentForwardSpeed = Mathf.Lerp(
            currentForwardSpeed,
            targetForwardSpeed * moveForce,
            (targetForwardSpeed == 0 ? deceleration : acceleration) * Time.deltaTime
        );

        currentLateralSpeed = Mathf.Lerp(
            currentLateralSpeed,
            targetLateralSpeed * moveForce,
            (targetLateralSpeed == 0 ? deceleration : acceleration) * Time.deltaTime
        );

        // Применение сил
        moveDirection = (transform.forward * currentForwardSpeed) +
                       (transform.right * currentLateralSpeed);

        if (moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection, ForceMode.Acceleration);
        }
    }

    void Rotate()
    {
        if (Keyboard.current.zKey.isPressed || leftStickHorizontal < -Sensitivity)
        {
            if (Application.isMobilePlatform && Gamepad.current == null)
                rb.angularVelocity = new Vector3(0, -rotationSpeed * Mathf.Deg2Rad * Time.deltaTime * 65f, 0);

            else
                rb.angularVelocity = new Vector3(0, -rotationSpeed * Mathf.Deg2Rad * Time.deltaTime * 250f, 0);
        }

        else if (Keyboard.current.cKey.isPressed || leftStickHorizontal > Sensitivity)
        {
            if (Application.isMobilePlatform && Gamepad.current == null)
                rb.angularVelocity = new Vector3(0, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime * 65f, 0);

            else
                rb.angularVelocity = new Vector3(0, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime * 250f, 0);
        }

        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Tilt(float x, float y, float z)
    {
        Quaternion targetRotation = Quaternion.Euler(x, y, z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);
    }

    public void onClickEngineButton()
    {
        if (Energy > 0) isOnEngine = !isOnEngine;
    }

    IEnumerator ApplySpeedBonus()
    {
        isSpeedBonusActive = true;

        baseMaxSpeed += 5f;
        forwardAccel += 5f;
        backwardAccel += 5f;
        lateralAccel += 5f;
        brakeForce += 5f;

        yield return new WaitForSeconds(10f);

        baseMaxSpeed -= 5f;
        forwardAccel -= 5f;
        backwardAccel -= 5f;
        lateralAccel -= 5f;
        brakeForce -= 5f;

        isSpeedBonusActive = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            isGround = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            isGround = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            isGround = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SignalThreshold"))
        {
            SignalThresholdText.SetActive(false);
        }

        else if (other.CompareTag("BatteryBonus"))
        {
            Energy = Mathf.Min(Energy + 10, 100);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("SpeedBonus") && !isSpeedBonusActive)
        {
            Destroy(other.gameObject);
            StartCoroutine(ApplySpeedBonus());
        }

        else if (other.CompareTag("TemperatureBonus"))
        {
            Destroy(other.gameObject);
            NewMonoBehaviourScript.TemperaturePizza = Mathf.Min(NewMonoBehaviourScript.TemperaturePizza + 25, 180);
        }

        else if (other.CompareTag("CubiteCoin"))
        {
            Destroy(other.gameObject);
            PlayerPrefs.SetInt("currentMoney", PlayerPrefs.GetInt("currentMoney", 0) + Random.Range(500, 1000));
            moneyTextChanger.SetValueSmoothly(PlayerPrefs.GetInt("currentMoney", 0));
            PlayerPrefs.Save();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SignalThreshold"))
        {
            SignalThresholdText.SetActive(true);
        }
        else if (other.CompareTag("Range"))
        {
            isSignal = false;
            DeathScreen.isDead = true;
        }
    }
}
