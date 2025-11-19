using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBase : MonoBehaviour, IAbility
{
    void Update()
    {
        ActionExecution();
    }
    public virtual void ActionExecution()
    {

    }

}
