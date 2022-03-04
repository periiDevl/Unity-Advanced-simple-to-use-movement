using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewbobing : MonoBehaviour
{
    [SerializeField] bool headBobing;
    [SerializeField, Range(0, 0.1f)] private float amp = 0.015f;
    [SerializeField, Range(0, 30f)] private float freq = 10f;
    private float toggleSpeed = 1;

    [SerializeField] Transform _cam = null;
    [SerializeField] Transform _camHolder = null;

    
    private Vector3 startPos;
    public CharacterController _controller;
    public playerMove p;

    //rotation
    private Vector3 currentRoation;
    private Vector3 targetRotation;

    


    //hipfire Recoil
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    //Settings
    public float snappiness;
    public float returnSpeed;

    

    private void Start()
    {
       
        startPos = _cam.localPosition;
    }

    // Update is called once per frame
    void Update()
    {


        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRoation = Vector3.Slerp(currentRoation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRoation);
        if (!headBobing) return;
        CheckMotion();
        ResetPos();
    }
    private void PlayMotion(Vector3 motion)
    {
        _cam.localPosition += motion;
    }

    private void CheckMotion()
    {
        //float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        // print(speed);
        
        
        if (p.moving == false) return;
        if (!_controller.isGrounded) return;

       
        PlayMotion(footStepM());
    }

    private Vector3 footStepM()
    {
        
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * freq) * amp;
        pos.x += Mathf.Cos(Time.time * freq / 2) * amp * 2;
        

        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        
        return pos;
    }

    void ResetPos()
    {
        
        if (_cam.localPosition == startPos) return;
        _cam.localPosition = Vector3.Lerp(_cam.localPosition, startPos, 1 * Time.deltaTime);
    }
}
