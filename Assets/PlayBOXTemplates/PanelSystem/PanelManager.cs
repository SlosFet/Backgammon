using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private List<Panel> _panels;

    private void Start()
    {
        OpenPanel(PanelType.AvatarSelectPanel);
    }

    public void OpenPanel(PanelType type,bool closeAllPanels = true)
    {
        if (closeAllPanels)
            _panels.ForEach(x => x.ClosePanel());

        _panels.Find(x => x.PanelType == type).OpenPanel();
    }

    public void ClosePanel(PanelType type)
    {
        _panels.Find(x => x.PanelType == type).ClosePanel();
    }
}

[System.Serializable]
public enum PanelType
{
    AvatarSelectPanel,
    GamePanel,
    WinPanel
}