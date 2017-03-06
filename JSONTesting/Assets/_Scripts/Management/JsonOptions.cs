using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The options that we have for monitoring
/// We can listen to filebeat, packetbeat, or both
/// </summary>
public struct JsonOptions  {

    public enum Option { Packetbeat, Filebeat };
    
}
