using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExitGate : MonoBehaviour
{
    public enum Destination
    {
        Next,
        Town
    }

    public Destination destination = Destination.Next;

    private void OnTriggerEnter(Collider other) {
        switch (destination) {
            case Destination.Next:
                MissionManager.instance?.LoadNextLesserScene();
                break;
            case Destination.Town:
                LevelController.LoadTown();
                break;
        }
    }
}
