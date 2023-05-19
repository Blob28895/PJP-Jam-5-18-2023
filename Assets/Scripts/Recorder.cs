using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    Dictionary<int, List<Vector3>> positions = new Dictionary<int, List<Vector3>>();

    void Update()
    {
        CheckDebugMessages();
    }

    public void RecordPosition(Vector3 currentPosition, int run)
    {
        // add current posiution to list at key value run
        if(positions.ContainsKey(run))
        {
            positions[run].Add(currentPosition);
        }
        else
        {
            positions.Add(run, new List<Vector3>());
            positions[run].Add(currentPosition);
        }
    }

    public IEnumerator PlaybackRun(int run, GameObject clone, int speed)
    {
        foreach(Vector3 targetPosition in positions[run])
        {
            while(clone.transform.position != targetPosition)
            {
                clone.transform.position = Vector3.MoveTowards(clone.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void CheckDebugMessages()
    {
        // debug stuff
        if(Input.GetKeyDown(KeyCode.Z))
        {
            // print positions dictionary
            foreach(KeyValuePair<int, List<Vector3>> entry in positions)
            {
                Debug.Log("Run: " + entry.Key);
                foreach(Vector3 position in entry.Value)
                {
                    Debug.Log(position);
                }
            }
        }
    }
}