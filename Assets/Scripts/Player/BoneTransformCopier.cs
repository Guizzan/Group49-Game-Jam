using UnityEngine;
using Guizzan.Extensions;
public class BoneTransformCopier : MonoBehaviour
{
    
    public Transform copyObject;
    public Transform pasteObject;
    Transform[] root;
    Transform[] copyObjectRoot;
    void Start()
    {
        root = pasteObject.GetComponentsInChildren<Transform>();
        copyObjectRoot = copyObject.GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < root.Length; i++)
        {
            root[i].LocalSetTransform(copyObjectRoot[i]);
        }
    }
}
