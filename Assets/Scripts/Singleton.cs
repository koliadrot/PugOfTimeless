using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    #region Field Declarations
    private static T _instance = null;
    private static readonly object threadlock = new object();

    public static T Instance
    {
        get
        {
            lock (threadlock)
            {
                return _instance;
            }
        }
    }
    #endregion

    #region Startup
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
    }

    #endregion
}
