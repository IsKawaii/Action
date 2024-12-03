using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public GameFlagCollection flagCollection;

    public void flagToggle(string flagToggleCommand)
    {
        if (flagToggleCommand != null)
        {
            flagCollection.FlagOn(flagToggleCommand);
        }
    }

    public bool flagCheck(string flagCheckCommand)
    {
        //flagCollection.CheckFlag(flagCheckCommand);
        return flagCollection.CheckFlag(flagCheckCommand);
    }
}
