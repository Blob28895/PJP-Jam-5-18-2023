using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    private Dictionary<int, List<Vector3>> positions = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Coroutine> currentPlaybacks = new Dictionary<int, Coroutine>();

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

    public void ClearRecording(int run)
    {
        positions[run].Clear();
    }   

    public void ClearAllRecordings()
    {
        positions.Clear();
    }

    public void StopAllPlaybacks()
    {
        foreach(KeyValuePair<int, Coroutine> entry in currentPlaybacks)
        {
            StopCoroutine(entry.Value);
        }
        currentPlaybacks.Clear();
    }

    public void StartPlaybackRun(int run, GameObject clone, int speed)
    {
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
}