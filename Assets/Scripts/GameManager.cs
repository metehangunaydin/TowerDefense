using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Help_UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void ToggleHelpUI()
    {
        if (Help_UI != null)
        {
            Help_UI.SetActive(!Help_UI.activeSelf);
        }
    }
}
