using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

/// <summary>
/// A condition which requires all enemies to be killed.
/// </summary>
public class KillAll_Condition : LevelCondition
{
    public List<EnemyHealth> m_exclusions;

    [ReadOnly] public List<EnemyHealth> m_enemies = new List<EnemyHealth>();

    [ReadOnly] public Transform _playerTrans = null;
    public Transform _checkPosition = null;

    private InteractManager m_interactManager = null;
    private InteractManager interactManager {
        get {
            if (m_interactManager == null) {
                m_interactManager = FindObjectOfType<InteractManager>();
            }
            return m_interactManager;
        }
    }

    protected override void UpdateCondition()
    {
        m_isComplete = true;
        // get all "enemies" that have a healthscript
        m_enemies = GameObject.FindObjectsOfType<EnemyHealth>(/* true */).ToList();

        // remove any that match the exclusions
        for (int i = m_enemies.Count - 1; i >= 0; i--){
            EnemyHealth eh = m_enemies[i];
            
            if (m_exclusions.Contains(eh)){
                m_enemies.RemoveAt(i);
            }
        }

        // check that all of them are dead, if not, set false, break
        foreach (EnemyHealth enemy in m_enemies)
            {
                if (m_exclusions.Contains(enemy)) continue;

                if (!enemy.hasDied)
                {
                    m_isComplete = false;
                    break;
                }
            }
    }

    private void Start() {
        if (_playerTrans == null && FindObjectOfType<PlayerMovement>()) _playerTrans = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update() {

        // if any enemies are dead or null, remove them from the list
        for (int i = m_enemies.Count - 1; i >= 0; i--) {
            if (m_enemies[i] == null || m_enemies[i].hasDied) {
                m_enemies.RemoveAt(i);
            }
        }

        float interactDistance = 5.0f;

        // if there are still enemies, say that in the text
        if (m_enemies.Count() > 0){
            // if _transToCheck is close to the gate
            if (Vector3.Distance(_checkPosition.position, _playerTrans.position) <= interactDistance)
            {
                string text = m_enemies.Count() + (m_enemies.Count() > 1 ? " ENEMIES" : " ENEMY") + " REMAINING";
                // send request
                interactManager.RequestBottomText(new InteractManager.TextRequest(text, this, _checkPosition, interactDistance));
            }
        }
    }
}
