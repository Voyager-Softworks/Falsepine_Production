using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
///  Script responsible for generating and managing mission scene sequencing.
/// </summary>
/// <remarks>
///  Its responsibilities include: Generating sequences of scenes, keeping track of where the player is in the sequence, and handling transitions between these scenes.
/// </remarks>
/// @todo
///     <list type="bullet">
///        <item>
///          Make the object persistent.
///        </item>
///    </list>
/// @bug
///    <list type="bullet">
///       <item>
///        The current implementation of the mission sequence is not robust.
///      </item>
///   </list>
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
        public List<string> scenes; ///< The list of scenes in the sequence.
        public string missionStartScene; ///< The scene to be loaded when the mission starts.
        public string missionEndScene; ///< The scene to be loaded when the mission ends.
    }

    public List<MissionSequence> missionSequences; ///< The list of mission sequences.

    public Queue<string> currentSequence; ///< The current sequence of scenes.

    /// <summary>
    ///  Generates a new sequence of scenes for a mission.
    /// </summary>
    /// <param name="area">The area of the game world the mission should take place.</param>
    void GenerateNewSequence(Area area)
    {
        currentSequence = new Queue<string>();
        // Pick a random sequence from the list of sequences, where the area matches the parameter.
        MissionSequence sequence = missionSequences.Where(x => x.area == area).OrderBy(x => Random.value).First();
        // Add the start scene to the sequence.
        currentSequence.Enqueue(sequence.missionStartScene);
        // Add the scenes in random order to the sequence.
        sequence.scenes.OrderBy(x => Random.value).ToList().ForEach(x => currentSequence.Enqueue(x));
        // Add the end scene to the sequence.
        currentSequence.Enqueue(sequence.missionEndScene);
    }

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

    void Awake()
    {
        // Dont destroy this object when the scene changes.
        DontDestroyOnLoad(gameObject);
    }




}
