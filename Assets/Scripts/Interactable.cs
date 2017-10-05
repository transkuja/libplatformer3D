using UnityEngine;


public enum InteractableType
        // For chests 
        { Openable,
        // To pick items on the ground
        Pickable,
        // To talk to NPCs
        Talkable };

public class Interactable : MonoBehaviour {

    [SerializeField]
    InteractableType type;

    public void Interact()
    {
        switch (type)
        {
            case InteractableType.Openable:
                Open();
                break;
            case InteractableType.Pickable:
                Pick();
                break;
            case InteractableType.Talkable:
                Talk();
                break;
        }
    }

    // Export this into components?
    void Open()
    {
        
    }

    void Pick()
    {

    }

    void Talk()
    {

    }
}
