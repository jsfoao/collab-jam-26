using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILinkable
{
    public bool CanLink(Link otherLink);
    public void OnLink(Link otherLink);
    public void OnUnLink(Link otherLink);
}

public class Link : MonoBehaviour, ILinkable
{
    [NonSerialized] public List<ILinkable> contexts = new List<ILinkable>();
    [NonSerialized] public List<Link> links = new List<Link>();

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
        if (!CanLink(otherLink) && !otherLink.CanLink(this))
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
}