using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu_Panel, carEditor_Panel, engineUpgrade_Panel;
    public GameObject car_GameObject;
    public GameObject menuCam, carCams;
    public Animator animator_Garage, animator_HoodCar;
    public Transform mainMenuPoint, garagePoint, enginePoint, car_StartPoint, mainCamera;
    private Quaternion mainMenuRotation, garageRotation; // Nieuwe rotatiepunten voor de camera
    public float lerpSpeed = 0.5f;

    //Waar ben je in de main menu
    private int menuIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        menuIndex = 0;
        mainMenuRotation = mainCamera.rotation; // Oorspronkelijke rotatie
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCar()
    {
        animator_Garage.SetBool("Open/Close", true);
        mainMenu_Panel.SetActive(false);
        StartCoroutine(LerpCamera(garagePoint.position, garageRotation)); // Lerpt naar de garage met nieuwe rotatie
        menuIndex = 1;
    }

    public void CarBackToMenu()
    {
        animator_Garage.SetBool("Open/Close", false);
        carEditor_Panel.SetActive(false);
        StartCoroutine(LerpCamera(mainMenuPoint.position, mainMenuRotation)); // Lerpt terug naar menu met oorspronkelijke rotatie
        menuIndex = 0;
    }

    public void OnEngineUpgrade()
    {
        animator_HoodCar.SetBool("Open/Close", true);
        menuIndex = 2;
        carEditor_Panel.SetActive(false);
        StartCoroutine(LerpCamera(enginePoint.position, enginePoint.rotation)); // Lerpt naar enginePos met bijbehorende rotatie
    }

    public void BackFromEngineUpgrade()
    {
        animator_HoodCar.SetBool("Open/Close", false);
        engineUpgrade_Panel.SetActive(false);
        StartCoroutine(LerpCamera(garagePoint.position, garagePoint.rotation)); // Lerpt terug naar garage met bijbehorende rotatie
        menuIndex = 1;
    }

    IEnumerator LerpCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        float elapsedTime = 0;

        Vector3 startingPos = mainCamera.position;
        Quaternion startingRot = mainCamera.rotation;

        while (elapsedTime < lerpSpeed)
        {
            mainCamera.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / lerpSpeed));
            mainCamera.rotation = Quaternion.Lerp(startingRot, targetRotation, (elapsedTime / lerpSpeed)); // Lerpt de rotatie
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (menuIndex == 1) carEditor_Panel.SetActive(true);
        else if (menuIndex == 2) engineUpgrade_Panel.SetActive(true);
        else mainMenu_Panel.SetActive(true);

        mainCamera.position = targetPosition; // Zorg ervoor dat de positie exact is wanneer de lerp eindigt.
        mainCamera.rotation = targetRotation; // Zorg ervoor dat de rotatie exact is wanneer de lerp eindigt.
    }

    public void OnPlay()
	{
        car_GameObject.transform.position = car_StartPoint.position;
        car_GameObject.transform.rotation = car_StartPoint.rotation;
        car_GameObject.GetComponent<CarManager>().enabled = true;
        menuCam.SetActive(false);
        carCams.SetActive(true);
	}

    public void OnQuit() => Application.Quit();
}
