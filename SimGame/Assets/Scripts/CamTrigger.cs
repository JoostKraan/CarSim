using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamTrigger : MonoBehaviour
{
    public GameObject trigger1, trigger2, trigger3, trigger4, trigger5, trigger6, auto ,CarCam;
    CinemachineBrain brain;
    public GameObject Cam1, Cam2, Cam3, Cam4;
    public void OnTriggerEnter(Collider other)
    {
        if (auto)
        {
            if (trigger1)
            { 
                if(brain.ActiveVirtualCamera.Name == "CAM1")
                {
                    Cam1.SetActive(true);
                    Cam2.SetActive(false); 
                    Cam3.SetActive(false);
                    Cam4.SetActive(false);
                    CarCam.SetActive(false);
                }
            }
            if (trigger2)
            {
                if (brain.ActiveVirtualCamera.Name == "CAM2")
                {
                    Cam1.SetActive(false);
                    Cam2.SetActive(true);
                    Cam3.SetActive(false);
                    Cam4.SetActive(false);
                    CarCam.SetActive(false);
                }
            }
            if (trigger3)
            {
                Cam1.SetActive(false);
                Cam2.SetActive(false);
                Cam3.SetActive(false);
                Cam4.SetActive(false);
                CarCam.SetActive(true);
            }
           
        }
    }


}
