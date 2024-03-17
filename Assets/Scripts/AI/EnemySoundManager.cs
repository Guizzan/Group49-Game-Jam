using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Guizzan.Extensions;
public class EnemySoundManager : BaseSoundEvent
{
    public Transform SoundPosTransform;
    public LayerMask RaycastSurfaceMask;

    public List<SurfaceSound> WalkingSounds = new();
    private Dictionary<int, List<int>> _usedWalkingSounds = new();
    public float MinWalkingStepInterval;
    private float _lastWalkingStepTime;

    public List<SurfaceSound> RunningSounds = new();
    private Dictionary<int, List<int>> _usedRunningSounds = new();
    public float MinRunningStepInterval;
    private float _lastRunningStepTime;

    public override void SoundEvent(string type) //Animation Event Trigger
    {
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit SurfaceHit, RaycastSurfaceMask);
        if (SurfaceHit.Equals(null)) return;
        LayerMask surfaceLayer = SurfaceHit.collider.gameObject.layer;
        switch (type)
        {
            case "WalkingStep":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref WalkingSounds, ref _usedWalkingSounds, ref MinWalkingStepInterval, ref _lastWalkingStepTime, 0.3f);
                break;
            case "RunningStep":
                MakeSound(ref SoundPosTransform, surfaceLayer, ref RunningSounds, ref _usedRunningSounds, ref MinRunningStepInterval, ref _lastRunningStepTime, 0.8f);
                break;
        }
    }
}
