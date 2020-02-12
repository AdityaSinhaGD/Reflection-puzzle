using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamEmitter : MonoBehaviour
{
    [SerializeField]private int noOfReflections = 10;
    [SerializeField]private float laserRange = 1000f;

    List<Vector3> reflectionTracker = new List<Vector3>();

    private int goalCount;
    private int goalObjectsHit = 0;
    private GameObject[] goalObjects;

    private LineRenderer lineRenderer;


    private void Awake()
    {
        InitializeGoalObjects();
        GameManager.Instance.gameState = GameManager.GameState.playing;
        CreateReflectableBeam();
    }

    public void CreateReflectableBeam()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        reflectionTracker.Clear();
        lineRenderer.SetPositions(reflectionTracker.ToArray());
        SimulateReflection(transform.position, transform.forward, noOfReflections);
    }

    private void InitializeGoalObjects()
    {
        goalObjects = GameObject.FindGameObjectsWithTag("goal");
        goalCount = goalObjects.Length;
        goalObjectsHit = 0;
        foreach (GameObject go in goalObjects)
        {
            go.GetComponent<HitCheck>().isAlreadyHit = false;
        }
    }

    void SimulateReflection(Vector3 pos, Vector3 direction, int noOfsteps)
    {
        
        if (noOfsteps == 0)
        {
            MaximumReflectionLimitReached();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(pos, direction, out hit, laserRange))
        {
            reflectionTracker.Add(pos);
            reflectionTracker.Add(hit.point);

            if(hit.transform.CompareTag("goal"))
            {
                hit = ProcessGoalObjectsHit(hit);
            }

            direction = Vector3.Reflect(hit.point - pos, hit.normal).normalized;
            pos = hit.point;
        }
        else
        {

            reflectionTracker.Add(pos);
            reflectionTracker.Add(direction * laserRange);
            lineRenderer.positionCount = reflectionTracker.Count;
            lineRenderer.SetPositions(reflectionTracker.ToArray());
            //Debug.Log("No Reflectable Surface Found");
            return;
        }

        SimulateReflection(pos, direction, --noOfsteps);
    }

    private RaycastHit ProcessGoalObjectsHit(RaycastHit hit)
    {
        if (hit.transform.GetComponent<HitCheck>().isAlreadyHit == false)
        {
            hit.transform.GetComponent<HitCheck>().isAlreadyHit = true;
            goalObjectsHit++;

            if (goalObjectsHit == goalCount)
            {
                StartCoroutine(LevelComplete());
            }
        }

        return hit;
    }

    private void MaximumReflectionLimitReached()
    {
        lineRenderer.positionCount = reflectionTracker.Count;
        lineRenderer.SetPositions(reflectionTracker.ToArray());
        Debug.Log("Maximum number of reflections reached");
        return;
    }

    IEnumerator LevelComplete()
    {
        GameManager.Instance.gameState = GameManager.GameState.end;
        GameManager.Instance.PlayLevelCompleteSFX();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadNextLevel();
    }
}
