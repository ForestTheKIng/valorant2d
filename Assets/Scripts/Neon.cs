using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Neon : MonoBehaviour
{
    public GameObject outline;
    public GameObject pp;
    public GameObject steps;
    const float maxEnergy = 100f;
    public float currentEnergy = maxEnergy;
    [SerializeField] Image energyImage;
    public Movement movement;
    public float NeonModeSpeed = 7.0f;
    public GameObject energyBar;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentEnergy >= 0){
            NeonMode();
        } else {
            movement.moveSpeed = 5f;
            pp.SetActive(false);
            steps.SetActive(false);
            energyBar.SetActive(false);
        }
    }


    void NeonMode() {
        energyBar.SetActive(true);
        currentEnergy -= 7 * Time.deltaTime;
        if (energyImage != null){
            energyImage.fillAmount = currentEnergy / maxEnergy;
            movement.moveSpeed = NeonModeSpeed;
            pp.SetActive(true);
            steps.SetActive(true);
        }
    }
}
