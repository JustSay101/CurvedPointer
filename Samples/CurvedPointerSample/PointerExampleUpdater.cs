using System.Collections;
using CustomPointers;
using UnityEngine;

public class PointerExampleUpdater : MonoBehaviour
{
    [SerializeField] private CurvedPointer pointer;
    private bool isOngoing;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0) || isOngoing)
            return;
        
        isOngoing = true;
        pointer.TogglePointer();
        StartCoroutine(CursorUpdate());
    }
    
    private IEnumerator CursorUpdate()
    {
        var startingPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        for (; Input.GetMouseButton(0) ;)
        {
            pointer.UpdateCursorPosition(startingPoint, mainCamera.ScreenToWorldPoint(Input.mousePosition));
            yield return null;
        }
        
        isOngoing = false;
        pointer.TogglePointer();
    }
}