using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu_Panel, CarEditor_Panel;
    public Animator animator_Garage;
    public Transform mainMenuPoint, garagePoint, mainCamera;
    public float lerpSpeed = 0.5f;

    //Waar ben je in de main menu
    private int menuIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        menuIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCar()
	{
        animator_Garage.SetBool("Open/Close", true);
        mainMenu_Panel.SetActive(false);
        StartCoroutine(LerpCamera(garagePoint.position));
        menuIndex = 1;
    }

    public void CarBackToMenu()
	{
        animator_Garage.SetBool("Open/Close", false);
        CarEditor_Panel.SetActive(false);
        StartCoroutine(LerpCamera(mainMenuPoint.position));
        menuIndex = 0;
    }

    IEnumerator LerpCamera(Vector3 targetPosition)
    {
        float elapsedTime = 0;

        Vector3 startingPos = mainCamera.position;

        while (elapsedTime < lerpSpeed)
        {
            mainCamera.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / lerpSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (menuIndex == 1) CarEditor_Panel.SetActive(true);
        else mainMenu_Panel.SetActive(true);

        mainCamera.position = targetPosition; // Zorg ervoor dat de positie exact is wanneer de lerp eindigt.
    }

    public void OnQuit() => Application.Quit();
}
