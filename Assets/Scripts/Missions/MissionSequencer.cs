using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Script responsible for generating and managing mission scene sequencing.
/// </summary>
/// <remarks>
///  Its responsibilities include: Generating sequences of scenes, keeping track of where the player is in the sequence, and handling transitions between these scenes.
/// </remarks>
/// @todo
///     <list type="bullet">
///        <item>
///           Add a method to generate a new sequence of scenes.
///        </item>
///        <item>
///          Make the object persistent.
///        </item>
///        <item>
///          Add a system to handle the transition between scenes, and keep track of the current scene.
///       </item>
///        <item>
///          Register when a scene is complete and allow the player to move to the next scene.
///       </item>
///    </list>
public class MissionSequencer : MonoBehaviour
{
    public enum Area ///< Areas of the game world.
    {
        Tundra, ///< The tundra area.
        Desert, ///< The desert area.
        RedwoodForest, ///< The redwood forest area.
        AutumnForest, ///< The autumn forest area.
        Swamp, ///< The swamp area.
        Town ///< The town area.
    }


    /// <summary>
    /// Contains all of the data required to construct a sequence of scenes for a mission.
    /// </summary>
    [System.Serializable]
    public struct MissionSequence
    {
        public string name; ///< The name of the sequence.
        public Area area; ///< The area of the sequence.
        public int minimumFillerScenes; ///< The minimum number of filler scenes to be included in the sequence.
        public int maximumFillerScenes; ///< The maximum number of filler scenes to be included in the sequence.
        public List<string> fillerScenes, objectiveScenes; ///< The list of scenes to be included in the sequence.
        public string missionStartScene; ///< The scene to be loaded when the mission starts.
        public string missionEndScene; ///< The scene to be loaded when the mission ends.
    }

    public List<MissionSequence> missionSequences; ///< The list of mission sequences.

    public Queue<string> currentSequence; ///< The current sequence of scenes.

    //void GenerateNewSequence

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    // Update is called once per frame
    /// </summary>
    void Update()
    {
        
    }




}
