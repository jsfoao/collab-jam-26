using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkable
{
    public bool CanLink(Link otherLink);
    public void OnLink(Link otherLink);
    public void OnUnLink(Link otherLink);
}

[RequireComponent(typeof(GameplayID))]
public class Link : MonoBehaviour, ILinkable
{
    [NonSerialized] public List<ILinkable> contexts = new List<ILinkable>();
    [NonSerialized] public List<Link> links = new List<Link>();

    // List of valid gameplay object IDs that can be linked with this link. If empty, any gameplay object ID is valid.
    [SerializeField] public List<GameplayObjectID> validObjects = new List<GameplayObjectID>();

    // If maxLinks is 0, there is no limit to the number of links that can be made with this link
    [SerializeField] public int maxLinks = 1;

    public GameplayObjectID ID => GetID();

    public bool LinkTo(Link otherLink)
    {
        if (!otherLink)
        {
            return false;
        }
        if (otherLink == this)
        {
            return false;
        }
        if (!CanLink(otherLink) || !otherLink.CanLink(this))
        {
            return false;
        }
        OnLink(otherLink);
        if (otherLink)
        {
            otherLink.OnLink(this);
        }
        return true;
    }

    public bool UnLink(Link otherLink)
    {
        if (!otherLink)
        {
            return false;
        }
        if (otherLink == this)
        {
            return false;
        }
        OnUnLink(otherLink);
        if (otherLink)
        {
            otherLink.OnUnLink(this);
        }
        return true;
    }

    // --- ILinkable ---

    public virtual bool CanLink(Link otherLink)
    {
        if (!otherLink)
        {
            return false;
        }
        if (otherLink == this)
        {
            return false;
        }
        if (links.Count >= maxLinks && maxLinks > 0)
        {
            return false;
        }

        // Evaluate if gameplay ID is valid
        GameplayID gameplayID = GetComponent<GameplayID>();
        if (!ID)
        {
            return false;
        }

        // Evaluate if other gameplay ID is valid
        if (!otherLink.ID)
        {
            return false;
        }

        if (validObjects.Count == 0 && otherLink.validObjects.Count == 0)
        {
            return false;
        }
        
        // Evaluate if other gameplay ID is valid for this link
        if (!validObjects.Contains(otherLink.ID) && !otherLink.validObjects.Contains(ID))
        {
            return false;
        }

        // Evaluate contexts
        if (contexts.Count > 0)
        {
            foreach (ILinkable context in contexts)
            {
                if (!context.CanLink(otherLink))
                {
                    return false;
                }
            }
        }
        return enabled;
    }

    public virtual void OnLink(Link otherLink)
    {
        Debug.Log($"Linked {gameObject.name} with {otherLink.gameObject.name}");

        links.Add(otherLink);
        foreach (ILinkable context in contexts)
        {
            context.OnLink(otherLink);
        }
    }

    public virtual void OnUnLink(Link otherLink)
    {
        Debug.Log($"Unlinked {gameObject.name} from {otherLink.gameObject.name}");

        foreach (ILinkable context in contexts)
        {
            context.OnUnLink(otherLink);
        }
        links.Remove(otherLink);
    }

    // Return gameplay ID
    private GameplayObjectID GetID()
    {
        GameplayID gameplayID = GetComponent<GameplayID>();
        if (gameplayID)
        {
            return gameplayID.ID;
        }
        return null;
    }
}