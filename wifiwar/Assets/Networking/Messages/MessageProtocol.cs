using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageProtocol
{
    enum MessageIDs
    {
        CLIENT_HELLO=1,
        CLIENT_GOODBYE,
        CLIENT_MOVE,
        CLIENT_LAST_UNUSED=100,
        SERVER_HELLO,
        SERVER_GOODBYE
    };
}
