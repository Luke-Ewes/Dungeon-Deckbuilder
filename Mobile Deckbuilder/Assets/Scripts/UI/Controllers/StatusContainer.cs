using System.Collections.Generic;
using UnityEngine;

public class StatusContainer : MonoBehaviour
{
    //Keeps track of the count of each status type
    private Dictionary<StatusType, int> statusIconsCount = new();
    //Keeps track of the instances of each status icon
    private Dictionary<StatusType, StatusIcon> statusIconInstances = new();

    [SerializeField] private BaseCharacter character;

    [Tooltip("World for enemies, normal for player"), SerializeField] private StatusIcon statusIconPrefab;

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponentInParent<BaseCharacter>();
        }
    }

    private void OnEnable()
    {
        SubToActions();
    }

    private void OnDisable()
    {
        UnsubFromActions(character);
    }

    /// <summary>
    /// Add a stack of a specific status type to the UI.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public void AddStatusStack(StatusType type, int count)
    {
        if (statusIconsCount.ContainsKey(type))
        {
            statusIconsCount[type] += count;
        }
        else
        {
            statusIconsCount[type] = count;
            statusIconInstances[type] = StatusIcon.Create(statusIconPrefab, transform, UIManager.Instance.IconSprites[type]);
        }
        StatusIcon icon = statusIconInstances[type];
        icon.SetStackCount(statusIconsCount[type]);
    }

    /// <summary>
    /// Remove a specific number of stacks from a status type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void RemoveStatusStack(StatusType type, int count)
    {
        if (statusIconsCount.ContainsKey(type))
        {
            statusIconsCount[type] -= count;
            if (statusIconsCount[type] <= 0)
            {
                EmptyStack(type);
            }
            else
            {
                statusIconInstances[type].SetStackCount(statusIconsCount[type]);
            }
        }
    }


    /// <summary>
    /// Empty the status stack of a specific type, removing the icon from the UI.
    /// </summary>
    /// <param name="type"></param>
    public void EmptyStack(StatusType type)
    {
        if (statusIconsCount.ContainsKey(type))
        {
            Destroy(statusIconInstances[type].gameObject);
            statusIconsCount.Remove(type);
            statusIconInstances.Remove(type);
        }
    }

    private void SubToActions()
    {
        character.StatusStackAdded += AddStatusStack;
        character.StatusStackRemoved += RemoveStatusStack;
        character.StatusStackEmptied += EmptyStack;
        character.Died += UnsubFromActions;
    }

    private void UnsubFromActions(BaseCharacter character)
    {
        character.StatusStackAdded -= AddStatusStack;
        character.StatusStackRemoved -= RemoveStatusStack;
        character.StatusStackEmptied -= EmptyStack;
        character.Died -= UnsubFromActions;
    }
}
