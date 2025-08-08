using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public bool IsPizzaCollected = false;

    public static bool IsPizzaGave = false;

    public bool IsPizzaNear = false;
    public bool IsPointNear = false;

    public bool GetCollect = false;
    public bool GetGive = false;

    public static bool IsCharging = false;

    public Vector3 AttachmentPoint = new Vector3(0f, -0.56f, 0.4f);

    public static float TemperaturePizza;
    public Image TemperatureImage;
    public float t;
    public static float i;

    public bool isButtonDown = false;

    public SmoothTextChange moneyTextChanger;

    public GameObject PickUpPizzaText;
    public GameObject DeliverPizzaText;

    private void Start()
    {
        moneyTextChanger.SetValueSmoothly(PlayerPrefs.GetInt("currentMoney", 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (((Keyboard.current?.qKey.isPressed ?? false) || (Gamepad.current?.buttonSouth.isPressed ?? false) || isButtonDown) && !IsPizzaCollected && IsPizzaNear)
        {
            GetCollect = true;
            PickUpPizzaText.SetActive(false);

            GameObject[] objects = GameObject.FindGameObjectsWithTag("GavePizza");
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }

        else if (((Keyboard.current?.qKey.isPressed ?? false) || (Gamepad.current?.buttonSouth.isPressed ?? false) || isButtonDown) && IsPizzaCollected && IsPointNear)
        {
            GetGive = true;
            DeliverPizzaText.SetActive(false);
        }

        if (GameObject.FindWithTag("Pizza") != null || GameObject.FindWithTag("CollectedPizza") != null)
        {
            TemperaturePizza -= 0.0045f;
            t = Mathf.InverseLerp(180f, 20f, TemperaturePizza);

            if (i <= 5)
            {
                i += Time.deltaTime;
                TemperatureImage.color = Color.Lerp(TemperatureImage.color, Color.red, i / 5f);
            }

            else
            {
                TemperatureImage.color = Color.Lerp(Color.red, Color.blue, t);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Pizza":
                PickUpPizzaText.SetActive(true);
                IsPizzaNear = true;
                IsPointNear = false;
                break;

            case "Point":
                DeliverPizzaText.SetActive(true);
                IsPointNear = true;
                IsPizzaNear = false;
                break;

            case "ChargingStation":
                IsCharging = true;
                break;
        }
    }

    public void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Pizza" && GetCollect)
        {
            other.gameObject.transform.SetParent(transform);
            other.gameObject.transform.localPosition = AttachmentPoint;
            other.tag = "CollectedPizza";
            float currentYRotation = other.gameObject.transform.localRotation.eulerAngles.y;

            // Устанавливаем вращение с сохранением угла по оси Y
            other.gameObject.transform.localRotation = Quaternion.Euler(0, currentYRotation, 0);

            IsPizzaCollected = true;
            IsPizzaNear = false;
            GetCollect = false;
        }

        else if (other.gameObject.tag == "Point" && GetGive)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("CollectedPizza"))
                {
                    IsPizzaCollected = false;

                    child.tag = "GavePizza";
                    child.transform.SetParent(null);

                    GetGive = false;

                    GameObject deliveryPoint = GameObject.FindGameObjectWithTag("Point");
                    Destroy(deliveryPoint);

                    IsPizzaGave = true;

                    PlayerPrefs.SetInt("currentMoney", PlayerPrefs.GetInt("currentMoney", 0) + Convert.ToInt32(UnityEngine.Random.Range(1000, 4000) * ((TemperaturePizza - 10f) / (180f - 10f))));
                    moneyTextChanger.SetValueSmoothly(PlayerPrefs.GetInt("currentMoney", 0)); // Плавное изменение текста
                    PlayerPrefs.Save();

                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Pizza":
                PickUpPizzaText.SetActive(false);
                IsPizzaNear = false;
                break;

            case "Point":
                DeliverPizzaText.SetActive(false);
                IsPointNear = false;
                break;

            case "ChargingStation":
                IsCharging = false;
                break;
        }
    }

    public void CatchButtonDown()
    {
        isButtonDown = true;
    }

    public void CatchButtonUp()
    {
        isButtonDown = false;
    }
}
