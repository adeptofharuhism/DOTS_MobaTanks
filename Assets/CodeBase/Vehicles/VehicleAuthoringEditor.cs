using Assets.CodeBase.Vehicles.Wheels;
using UnityEditor;
using UnityEngine;

namespace Assets.CodeBase.Vehicles
{
    [CustomEditor(typeof(VehicleAuthoring))]
    public class VehicleAuthoringEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUILayout.Button("Enumerate wheels")) {
                VehicleAuthoring vehicleAuthoring = (VehicleAuthoring)target;
                WheelAuthoring[] wheelAuthoringComponents = vehicleAuthoring.GetComponentsInChildren<WheelAuthoring>();

                for (int i = 0; i < wheelAuthoringComponents.Length; i++) {
                    wheelAuthoringComponents[i].WheelIndex = i;
                    wheelAuthoringComponents[i].Parent = vehicleAuthoring.gameObject;
                }

                vehicleAuthoring.WheelAmount = wheelAuthoringComponents.Length;

                Debug.Log($"Enumerated {vehicleAuthoring.WheelAmount} wheel authoring components");
            }
        }
    }
}