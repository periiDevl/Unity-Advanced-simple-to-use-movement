using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveEditor : EditorWindow
{
    MovementSystem ms;
    AnimationCurve accelorationCurve;
    AnimationCurve DeCurve;
    LayerMask gm;

    float TToDecel;
    float gravity;
    float JumpForce;
    bool ChangeSelectionValue;

    bool showBaseMovement;
    

    [MenuItem("Movement System Tools/Normal Movement/Base Movement")]
    public static void ShowBasedMovementWindow()
    {
        GetWindow<MoveEditor>("BaseMovementWindow");
        
    }

    private void OnGUI()
    {
        
        
        

        GUILayout.Label("Base Movement Editor");
        


        foreach (GameObject obj in Selection.gameObjects)
        {

            ms = obj.GetComponent<MovementSystem>();
            ChangeSelectionValue = true;
            
            if (ChangeSelectionValue)
            {
                gm = ms.GroundLayerMask;
                accelorationCurve = ms.AccelerationMovementCurve;
                DeCurve = ms.DecelerationCurve;
                gravity = ms.Gravity;
                TToDecel = ms.TimeToDecelerate;
                JumpForce = ms.JumpForce;
                ChangeSelectionValue = false;

            }
            if (!ChangeSelectionValue)
            {
                accelorationCurve = EditorGUILayout.CurveField("acceleration curve", accelorationCurve);
                ms.TimeToDecelerate = EditorGUILayout.FloatField("time to decelrate", TToDecel);
                DeCurve = EditorGUILayout.CurveField("deceleration curve", DeCurve);
                ms.Gravity = EditorGUILayout.FloatField("gravity", gravity);
                ms.JumpForce = EditorGUILayout.FloatField("jump force", JumpForce);
                ms.GroundLayerMask = EditorGUILayout.LayerField("ground layer", gm);
            }

            Debug.Log(ChangeSelectionValue);

            
        }
         
        

    }

    

    
}
