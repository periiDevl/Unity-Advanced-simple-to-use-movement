using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textManager : MonoBehaviour
{
    public Text speedText;
     public Text cyoteText;
    public Text jumpsText;
    public Text GravityText;
    [SerializeField] playerMove p;

   void Update()
    {
        speedText.text = p.speed.ToString();
        cyoteText.text = p.mayJump.ToString();
        jumpsText.text = p.jumps.ToString();
        GravityText.text = p.gravity.ToString();
    }
}
