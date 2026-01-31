using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PizzaBase : NetworkBehaviour
{
    public Dough Dough;

    public Sauce Sauce;

    public List<Topping> Toppings;

    [Button]
    public void Cook()
    {
        if (!Dough || !Sauce || Toppings.Count == 0)
        {
            Debug.LogWarning("Requirement not met");
            return;
        }
    }
}
