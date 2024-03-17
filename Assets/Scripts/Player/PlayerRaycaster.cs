using UnityEngine;
using Guizzan.Extensions;

public class PlayerRaycaster : MonoBehaviour
{

    public static GameObject Selection;
    public float ReachDistance = 5;
    public LayerMask RaycastLayer; [Tooltip("Layers that raycast can hit")]
    public LayerMask SelectLayer; [Tooltip("Layers that will be highlighted")]
    public int HighlightLayer; [Tooltip("Index of layer that makes objects highlighted")]
    public static Ray Ray;
    public static RaycastHit Hit;
    private Camera CameraMain;
    private GameObject cross;
    private int _oldLayer;
    private PlayerController _player;

    private void Start()
    {
        _player = GetComponent<PlayerController>();
        cross = _player.Crosshair;
        CameraMain = Camera.main;
    }
    void Update()
    {
        if (!_player._isAiming)
        {
            if (Selection != null)
            {
                Selection.layer = _oldLayer;
                foreach (Renderer renderer in Selection.GetComponentsInChildren<Renderer>())
                    renderer.gameObject.layer = _oldLayer;
                Selection = null;
            }
            return;
        }
        GameObject _raycastObject = null;
        Ray = CameraMain.ScreenPointToRay(new Vector2(cross.transform.position.x, cross.transform.position.y));
        if (Physics.Raycast(Ray, out Hit, ReachDistance, RaycastLayer))
        {
            if (SelectLayer.ContainsLayer(Hit.collider.gameObject.layer)) _raycastObject = Hit.collider.transform.GetTopMostParrent().gameObject;
            else Hit = new RaycastHit();
        }
        else Hit = new RaycastHit();

        if (Selection != _raycastObject)
        {
            if (Selection != null)
            {
                Selection.layer = _oldLayer;
                foreach (Renderer renderer in Selection.GetComponentsInChildren<Renderer>())
                    renderer.gameObject.layer = _oldLayer;
            }

            if (_raycastObject != null)
            {
                _oldLayer = _raycastObject.layer;
                _raycastObject.layer = HighlightLayer;
                foreach (Renderer renderer in _raycastObject.GetComponentsInChildren<Renderer>())
                    renderer.gameObject.layer = HighlightLayer;

            }
        }
        Selection = _raycastObject;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Debug.DrawRay(Ray.origin, Ray.direction * ReachDistance, Color.yellow);
    }
#endif
}
