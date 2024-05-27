using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{

    [SerializeField]GameObject carWeapon;
    [SerializeField] float rotateSpeed = 25f;
    [SerializeField] ParticleSystem particle = new ParticleSystem();
    [SerializeField] GameObject awakePage, carSelectorPage, mapSelectorPage;
    [SerializeField] Animator cameraAnim;
    [SerializeField] Animator carAnim;
    [SerializeField] Canvas canvas;
    [SerializeField] VehicleList vehicleList;
    [SerializeField] int vehiclePointer;
    float elapsedTime = 2f;

    private Vector3 spawnPos = new Vector3(-1.348612f, 1f, 13.89127f);
    private Quaternion spawnRot = new Quaternion(0f, 90f, 0f, 45f);


    // Start is called before the first frame update
    void Start()
    {
        carWeapon = GameObject.Find("RangedWeapon.003").gameObject? GameObject.Find("RangedWeapon.003").gameObject : null ;
        particle = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
        cameraAnim = GameObject.Find("Main Camera").GetComponent<Animator>();
        canvas = GameObject.Find("canvas").GetComponent<Canvas>();
        awakePage = canvas.transform.Find("AwakePage").gameObject;
        carSelectorPage = canvas.transform.Find("CarSelectorPage").gameObject;
        mapSelectorPage = canvas.transform.Find("MapSelectorPage").gameObject;
        vehicleList = GameObject.Find("VehicleList").GetComponent<VehicleList>();
        PlayerPrefs.SetInt("pointer", 0);
        vehiclePointer = PlayerPrefs.GetInt("pointer");
    }

    // Update is called once per frame
    void Update()
    {
        if(carWeapon != null) 
            carWeapon.transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        particle.Play();
        if(vehiclePointer >= vehicleList.vehicles.Length - 1)
            carSelectorPage.transform.Find("Next").GetComponent<Button>().enabled = false;
        else 
            carSelectorPage.transform.Find("Next").GetComponent<Button>().enabled = true; 

        if (vehiclePointer == 0)
            carSelectorPage.transform.Find("Previous").GetComponent<Button>().enabled = false;
        else 
            carSelectorPage.transform.Find("Previous").GetComponent<Button>().enabled = true;
        if (carAnim == null)
        {
           carAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        }
        if (elapsedTime > 0 && (carAnim.GetBool("backClicked") || carAnim.GetBool("startClicked") || cameraAnim.GetBool("backToCarClicked") || cameraAnim.GetBool("confirmCarClicked")))
        {
            elapsedTime -= Time.deltaTime;
        }
        else
        {

            // car booleans
            carAnim.SetBool("backClicked", false);
            carAnim.SetBool("startClicked", false);

            // camera booleans
            cameraAnim.SetBool("backClicked", false);
            cameraAnim.SetBool("startClicked", false);
            cameraAnim.SetBool("backToCarClicked", false);
            cameraAnim.SetBool("confirmCarClicked", false);
            elapsedTime = 2;
        }  

    }

    public void startGame()
    {
        cameraAnim.SetBool("startClicked", true);
        carAnim.SetBool("startClicked", true);
        cameraAnim.SetBool("backClicked", false);
        carAnim.SetBool("backClicked", false);

        awakePage.SetActive(false);
        carSelectorPage.SetActive(true);
        
    }

    public void nextButton()
    {
        if (vehiclePointer < vehicleList.vehicles.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject child = Instantiate(vehicleList.vehicles[vehiclePointer], spawnPos, spawnRot);
            
        }
    }

    public void prevButton()
    {
        if (vehiclePointer >= vehicleList.vehicles.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject child = Instantiate(vehicleList.vehicles[vehiclePointer], spawnPos, spawnRot);
        }
    }

    public void confirmCarButton()
    {
        cameraAnim.SetBool("confirmCarClicked", true);
        mapSelectorPage.SetActive(true);
        carSelectorPage.SetActive(false);
    }

    public void backToCarButton()
    {
        cameraAnim.SetBool("backToCarClicked", true);
        mapSelectorPage.SetActive(false);
        carSelectorPage.SetActive(true);
    }


    public void backToMain()
    {
        cameraAnim.SetBool("backClicked", true);
        carAnim.SetBool("backClicked", true);
        cameraAnim.SetBool("startClicked", false);
        carAnim.SetBool("startClicked", false);
        

        awakePage.SetActive(true);
        carSelectorPage.SetActive(false);   
    }

    public void backToCar()
    {
        cameraAnim.SetBool("backToCarClicked", true);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    


}
