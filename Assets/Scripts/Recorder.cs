using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    private Dictionary<int, List<Vector3>> positions = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Coroutine> currentPlaybacks = new Dictionary<int, Coroutine>();

    void Update()
    {
        CheckDebugMessages();
    }

    public void RecordPosition(Vector3 currentPosition, int run)
    {
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

    public void StartPlaybackRun(int run, GameObject clone, int speed)
    {
        if(currentPlaybacks.ContainsKey(run))
        {
            StopCoroutine(currentPlaybacks[run]);
            currentPlaybacks.Remove(run);
        }

        

        currentPlaybacks.Add(run, StartCoroutine(PlaybackRun(run, clone, speed)));
    }

    private IEnumerator PlaybackRun(int run, GameObject clone, int speed)
    {
        foreach(Vector3 targetPosition in positions[run])
        {
            clone.transform.position = targetPosition;
            yield return new WaitForFixedUpdate();
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