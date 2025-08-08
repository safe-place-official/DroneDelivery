using UnityEngine;

public class LoadUpgrade : MonoBehaviour
{
    public string upgradeName;
    public int currentLevel;

    public Mesh[] upgradeMeshes;
    public MeshFilter[] UpgradeMeshFilters;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("Level_" + upgradeName, 1);

        Mesh newMesh = upgradeMeshes[currentLevel - 1];

        foreach (var meshFilter in UpgradeMeshFilters)
        {
            meshFilter.mesh = newMesh;
        }
    }
}
