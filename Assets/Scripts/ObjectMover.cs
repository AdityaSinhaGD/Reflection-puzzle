using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectMover : MonoBehaviour, IDragHandler
{

    BeamEmitter beamEmitter;

    // Start is called before the first frame update
    void Start()
    {
        beamEmitter = FindObjectOfType<BeamEmitter>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.gameState != GameManager.GameState.end)
        {

            beamEmitter.CreateReflectableBeam();
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                transform.position = ray.origin + ray.direction * distance;
            }
        }
    }

}
