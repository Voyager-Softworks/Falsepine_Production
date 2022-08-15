using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveInteract : Interactable  /// @todo Comment
{
    public Mission _mission;
    public GameObject completeSound;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    override public void DoInteract()
    {
        base.DoInteract();

        if (_mission != null)
        {
            if (MissionManager.instance != null)
            {
                if (_mission == MissionManager.instance.GetCurrentMission())
                {
                    MissionManager.instance.GetCurrentMission().SetCompleted(true);

                    if (completeSound != null)
                    {
                        Instantiate(completeSound, transform.position, Quaternion.identity);
                    }

                    base.DoInteract();

                    FadeScript fs = FindObjectOfType<FadeScript>();
                    if (fs){
                        fs.EndScreen(true);
                    }
                    else{
                        LevelController.LoadTown();
                    }
                }
            }
        }
    }
}
