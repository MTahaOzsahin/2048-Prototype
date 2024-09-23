using UnityEngine;

public class GridPanelButtonController : MonoBehaviour
{
    private GameObject parentOfThisGameObject;
    [SerializeField] private GameObject targetGameObject;

    private void Awake()
    {
        parentOfThisGameObject = transform.parent.gameObject;
    }

    public void OnGridPanelOpen()
    {
        transform.SetParent(targetGameObject.transform);
    }

    public void OnGridPanelClose()
    {
        transform.SetParent(parentOfThisGameObject.transform);
    }
}
