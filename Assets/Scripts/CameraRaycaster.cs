using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public Utils.Layer[] layerPriorities = {
		Utils.Layer.Enemy,
		Utils.Layer.Walkable
    };

    [SerializeField] private float distanceToBackground = 100f;
    private Camera viewCamera;

	private  RaycastHit m_hit;
    public RaycastHit hit
    {
        get { return m_hit; }
    }

	Utils.Layer m_layerHit;
    public Utils.Layer layerHit
    {
        get { return m_layerHit; }
    }

	public delegate void OnLayerChange(Utils.Layer layer);
	public event OnLayerChange onLayerChange;

    void Start() // TODO Awake?
    {
        viewCamera = Camera.main;
    }

    void Update()
    {
        // Look for and return priority layer hit
        foreach (Utils.Layer layer in layerPriorities)
        {
            var hit = RaycastForLayer(layer);
            if (hit.HasValue)
            {
				if (m_layerHit != layer)
				{
					onLayerChange(layer);
				}
				m_hit = hit.Value;
                m_layerHit = layer;
                return;
            }
        }

		if(m_layerHit != Utils.Layer.RaycastEndStop)
		{
			onLayerChange(Utils.Layer.RaycastEndStop);
		}

        // Otherwise return background hit
        m_hit.distance = distanceToBackground;
        m_layerHit = Utils.Layer.RaycastEndStop;
    }

    RaycastHit? RaycastForLayer(Utils.Layer layer)
    {
        int layerMask = 1 << (int)layer; // See Unity docs for mask formation
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit; // used as an out parameter
        bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask);
        if (hasHit)
        {
            return hit;
        }
        return null;
    }
}
