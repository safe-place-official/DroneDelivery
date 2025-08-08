using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class MainMenuQuadrocopter : MonoBehaviour
{
    [Header("Propeller Settings")]
    public float propellerRotationSpeed = 1000f;
    public float smoothSpeed = 1f;
    public float maxRotationSpeed = 1500f;
    public float minRotationSpeed = 500f;
    public float slowMotionFactor = 0.2f;

    [Header("Menu References")]
    public GameObject mainMenu;
    public GameObject upgradeMenu;
    public Transform upgradeMenuCameraPosition;
    public Transform initialCameraTransform;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float cameraSwayAmount = 0.1f;
    public float cameraSwaySpeed = 1f;
    public float positionSmoothTime = 0.3f;
    public float rotationSmoothTime = 0.2f;

    [Header("Propeller References")]
    public GameObject PL;
    public GameObject ZL;
    public GameObject ZP;
    public GameObject PP;

    private bool isUpgradeMenuOpen = false;
    private bool isCameraMoving = false;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;
    private Vector3 positionVelocity = Vector3.zero;
    private bool shouldSway = true;

    private float PLcurrentRotationSpeed;
    private float ZLcurrentRotationSpeed;
    private float ZPcurrentRotationSpeed;
    private float PPcurrentRotationSpeed;

    float time;

    void Start()
    {
        Time.timeScale = 1f;
        InitializeCameraPosition();

        //PlayerPrefs.SetInt("currentMoney", 100000);
    }

    void InitializeCameraPosition()
    {
        // Всегда используем explicit transform для инициализации
        if (initialCameraTransform != null)
        {
            initialCameraPosition = initialCameraTransform.position;
            initialCameraRotation = initialCameraTransform.rotation;

            // Сразу устанавливаем камеру в правильное положение
            if (cameraTransform != null)
            {
                cameraTransform.position = initialCameraPosition;
                cameraTransform.rotation = initialCameraRotation;
            }
        }

        targetCameraPosition = initialCameraPosition;
        targetCameraRotation = initialCameraRotation;
    }

    void Update()
    {
        UpdatePropellerSpeeds();
        RotatePropellers();
        UpdateCameraPosition();

        // Применяем покачивание камеры только когда она не движется и не в меню апгрейда
        if (shouldSway && !isUpgradeMenuOpen && !isCameraMoving)
        {
            SwayCamera();
        }

        // Обновляем цвет подсветки геймпада, если он подключен
        var gamepad = Gamepad.current;
        if (gamepad != null && gamepad is DualShockGamepad ds4)
        {
            time += Time.deltaTime;
            float intensity = (Mathf.Sin(time * 1.5f) + 1f) / 2f; // Значение от 0 до 1
            Color color = new Color(intensity, intensity, intensity); // От черного к синему
            ds4.SetLightBarColor(color);
        }
    }

    void ToggleUpgradeMenu()
    {
        isUpgradeMenuOpen = !isUpgradeMenuOpen;
        shouldSway = !isUpgradeMenuOpen;

        mainMenu.SetActive(!isUpgradeMenuOpen);
        upgradeMenu.SetActive(isUpgradeMenuOpen);

        if (isUpgradeMenuOpen)
        {
            // Перемещаем камеру к позиции меню апгрейда
            targetCameraPosition = upgradeMenuCameraPosition.position;
            targetCameraRotation = upgradeMenuCameraPosition.rotation;
            isCameraMoving = true;
        }
        else
        {
            // Возвращаем камеру на начальную позицию
            ReturnCameraToInitialPosition();
        }
    }

    void ReturnCameraToInitialPosition()
    {
        // Убираем перезапись initial позиций - используем только заранее заданные
        targetCameraPosition = initialCameraPosition;
        targetCameraRotation = initialCameraRotation;
        isCameraMoving = true;
        shouldSway = false;
    }

    void UpdatePropellerSpeeds()
    {
        float targetSpeed = isUpgradeMenuOpen ? 0f :
            Mathf.Clamp(propellerRotationSpeed * 1.2f, minRotationSpeed, maxRotationSpeed);

        PLcurrentRotationSpeed = Mathf.Lerp(PLcurrentRotationSpeed, targetSpeed, Time.deltaTime * smoothSpeed);
        ZLcurrentRotationSpeed = Mathf.Lerp(ZLcurrentRotationSpeed, targetSpeed, Time.deltaTime * smoothSpeed);
        ZPcurrentRotationSpeed = Mathf.Lerp(ZPcurrentRotationSpeed, targetSpeed, Time.deltaTime * smoothSpeed);
        PPcurrentRotationSpeed = Mathf.Lerp(PPcurrentRotationSpeed, targetSpeed, Time.deltaTime * smoothSpeed);
    }

    void RotatePropellers()
    {
        float slowMotionDelta = Time.deltaTime * slowMotionFactor;
        if (PL) PL.transform.Rotate(Vector3.forward, PLcurrentRotationSpeed * slowMotionDelta * -1);
        if (ZL) ZL.transform.Rotate(Vector3.forward, ZLcurrentRotationSpeed * slowMotionDelta);
        if (ZP) ZP.transform.Rotate(Vector3.forward, ZPcurrentRotationSpeed * slowMotionDelta * -1);
        if (PP) PP.transform.Rotate(Vector3.forward, PPcurrentRotationSpeed * slowMotionDelta);
    }

    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;
        if (!isCameraMoving) return;

        // Плавное перемещение позиции
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            targetCameraPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        // Плавное вращение
        cameraTransform.rotation = Quaternion.Slerp(
            cameraTransform.rotation,
            targetCameraRotation,
            Time.deltaTime / rotationSmoothTime
        );

        // Проверяем, достигла ли камера целевой позиции
        CheckCameraPositionReached();
    }

    void CheckCameraPositionReached()
    {
        if (!isCameraMoving) return;

        float positionDistance = Vector3.Distance(cameraTransform.position, targetCameraPosition);
        float angleDifference = Quaternion.Angle(cameraTransform.rotation, targetCameraRotation);

        if (positionDistance < 0.01f && angleDifference < 0.5f)
        {
            isCameraMoving = false;
            // Включаем sway только после полного возврата
            if (!isUpgradeMenuOpen) shouldSway = true;
        }
    }

    void SwayCamera()
    {
        if (cameraTransform == null || isUpgradeMenuOpen || isCameraMoving) return;

        // Используем целевую позицию как основу для покачивания
        float swayX = Mathf.Sin(Time.time * cameraSwaySpeed) * cameraSwayAmount;
        float swayY = Mathf.Cos(Time.time * cameraSwaySpeed * 0.5f) * cameraSwayAmount * 0.5f;
        cameraTransform.position = Vector3.SmoothDamp(
            cameraTransform.position,
            targetCameraPosition + new Vector3(swayX, swayY, 0),
            ref positionVelocity,
            positionSmoothTime
        );
    }

    public void HandleMenuToggle()
    {
        ToggleUpgradeMenu();
    }
}