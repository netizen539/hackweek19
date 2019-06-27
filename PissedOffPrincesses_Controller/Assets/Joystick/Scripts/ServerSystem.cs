using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Input;
using Unity.Tiny.Text;
#if UNITY_WEBGL
using Unity.Tiny.HTML;
#endif

public class ServerSystem
{

    [DllImport("__Internal")]
    private static extern void JSConnectToServer(int s1, int s2, int s3, int s4, int serverPort);

    [DllImport("__Internal")]
    private static extern void JSSendJoystickToServer(float x, double y);
    
    public static void ConnectToServer(int s1, int s2, int s3, int s4, int port)
    {
        JSConnectToServer(s1, s2, s3, s4, port);         
    }

    public static void SendJoystickToServer(float x, float y) 
    { 
        JSSendJoystickToServer(x, y);
    }
    
}

