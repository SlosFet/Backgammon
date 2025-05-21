using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPlace : BoardPlace
{
    public override void SetAvailable(bool state)
    {
        _canAvailable = state;
    }
}
