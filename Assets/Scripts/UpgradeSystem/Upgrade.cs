using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]

public class Upgrade
{
    public string upgradeName;
    public int currentLevel;
    public int maxLevel;
    public Text LevelText;

    public int cost;
    public GameObject CostText;
    public Text _CostText;
    public SmoothTextChange moneyTextChanger;
    public SmoothTextChange costTextChanger;

    public Mesh[] upgradeMeshes;
    public MeshFilter[] UpgradeMeshFilters;


    public void UpgradeLevel()
    {
        if (currentLevel < maxLevel && PlayerPrefs.GetInt("currentMoney", 0) >= cost)
        {
            PlayerPrefs.SetInt("currentMoney", PlayerPrefs.GetInt("currentMoney", 0) - cost);
            moneyTextChanger.SetValueSmoothly(PlayerPrefs.GetInt("currentMoney", 0)); // Плавное изменение текста

            PlayerPrefs.Save();

            currentLevel++;
            cost = Convert.ToInt32(cost * 1.7);
        }

        PlayerPrefs.SetInt("Level_" + upgradeName, currentLevel);
        PlayerPrefs.Save();

        Mesh newMesh = upgradeMeshes[currentLevel - 1];

        foreach (var meshFilter in UpgradeMeshFilters)
        {
            meshFilter.mesh = newMesh;
        }
    }

    public void CostManager()
    {
        if (currentLevel == maxLevel)
            CostText.SetActive(false);

        else
        {
            costTextChanger.SetValueSmoothly(cost);
            ChangeColorText();
        }
    }

    public void ChangeColorText()
    {
        Text CostTextComponent = _CostText.GetComponent<Text>();

        if (PlayerPrefs.GetInt("currentMoney", 0) < cost)
            CostTextComponent.color = Color.red;

        else
            CostTextComponent.color = Color.white;
    }

    public void LoadLevel()
    {
        currentLevel = PlayerPrefs.GetInt("Level_" + upgradeName, 1);

        cost = cost * (int)Math.Pow(1.7f, currentLevel);

        Mesh newMesh = upgradeMeshes[currentLevel - 1];

        foreach (var meshFilter in UpgradeMeshFilters)
        {
            meshFilter.mesh = newMesh;
        }

        if (currentLevel != maxLevel)
        {
            ChangeColorText();
        }
    }
}