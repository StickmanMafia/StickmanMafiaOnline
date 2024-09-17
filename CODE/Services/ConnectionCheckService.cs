using UnityEngine;

public class ConnectionCheckService : MonoBehaviour
{
    public static bool Check() => Application.internetReachability != NetworkReachability.NotReachable;
}
