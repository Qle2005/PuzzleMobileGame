using UnityEngine;

public class DraggableObjectVisual : MonoBehaviour
{
    [SerializeField]private Material originalMaterial;
    [SerializeField]private Material canPlaceMaterial;
    [SerializeField]private Material cannotPlacelMaterial;

    private MeshRenderer modelRenderer;
    private DraggableObject draggableObject;

    void Awake()
    {
        modelRenderer=GetComponentInChildren<MeshRenderer>();
        draggableObject=GetComponent<DraggableObject>();
    }

    void OnEnable()
    {
        draggableObject.OnStateChanged+=HandleChangeState;
    }

    void OnDisable()
    {
        draggableObject.OnStateChanged-=HandleChangeState;
    }

     private void HandleChangeState(DraggableObjectState objectState)
    {
        switch (objectState)
        {
            case DraggableObjectState.CanPlace:
            modelRenderer.material=canPlaceMaterial;
                break;
            case DraggableObjectState.CannotPlace:
            modelRenderer.material=cannotPlacelMaterial;
                break;
            case DraggableObjectState.Origin:
            modelRenderer.material=originalMaterial;
                break;
        }
    }
}
