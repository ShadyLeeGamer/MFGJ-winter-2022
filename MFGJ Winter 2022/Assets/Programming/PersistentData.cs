using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PersistentData : MonoBehaviour
{
    public bool MobileController { get; set; }

    public static PersistentData Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        MobileController = CheckIfMobile();
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    public bool CheckIfMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return IsMobile();
#endif
        return false;
    }
}