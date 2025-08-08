using UnityEngine;
using UnityEngine.UI;

public class DroneUpgradeSystem : MonoBehaviour
{
    public Upgrade batteryUpgrade;
    public Upgrade engineUpgrade;
    public Upgrade antennaUpgrade;
    public Upgrade screwUpgrade;

    public SmoothTextChange moneyTextChanger;

    public void Start()
    {
        batteryUpgrade.upgradeName = "batteryUpgrade";
        engineUpgrade.upgradeName = "engineUpgrade";
        antennaUpgrade.upgradeName = "antennaUpgrade";
        screwUpgrade.upgradeName = "screwUpgrade";

        batteryUpgrade.LoadLevel();
        engineUpgrade.LoadLevel();
        antennaUpgrade.LoadLevel();
        screwUpgrade.LoadLevel();

        batteryUpgrade.LevelText.text = batteryUpgrade.currentLevel + "/" + batteryUpgrade.maxLevel;
        engineUpgrade.LevelText.text = engineUpgrade.currentLevel + "/" + engineUpgrade.maxLevel;
        antennaUpgrade.LevelText.text = antennaUpgrade.currentLevel + "/" + antennaUpgrade.maxLevel;
        screwUpgrade.LevelText.text = screwUpgrade.currentLevel + "/" + screwUpgrade.maxLevel;

        batteryUpgrade.CostManager();
        engineUpgrade.CostManager();
        antennaUpgrade.CostManager();
        screwUpgrade.CostManager();

        moneyTextChanger.SetValueSmoothly(PlayerPrefs.GetInt("currentMoney", 0));
    }

    public void UpgradeButton(string nameUpgrade)
    {
        Upgrade selected = null;

        switch (nameUpgrade.ToLower())
        {
            case "batteryupgrade":
                selected = batteryUpgrade;
                break;
            case "engineupgrade":
                selected = engineUpgrade;
                break;
            case "antennaupgrade":
                selected = antennaUpgrade;
                break;
            case "screwupgrade":
                selected = screwUpgrade;
                break;
        }

        selected.UpgradeLevel();
        selected.CostManager();
        selected.LevelText.text = selected.currentLevel + "/" + selected.maxLevel;

        batteryUpgrade.ChangeColorText();
        engineUpgrade.ChangeColorText();
        antennaUpgrade.ChangeColorText();
        screwUpgrade.ChangeColorText();
    }
}
