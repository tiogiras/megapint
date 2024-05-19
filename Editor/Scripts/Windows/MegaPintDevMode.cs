﻿#if UNITY_EDITOR
using Editor.Scripts.GUI;
using Editor.Scripts.PackageManager;
using Editor.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = Editor.Scripts.GUI.GUIUtility;

namespace Editor.Scripts.Windows
{

public class MegaPintDevMode : MegaPintEditorWindowBase
{
    private static MegaPintSettingsBase _Settings =>
        MegaPintSettings.instance.GetSetting("General");

    private VisualTreeAsset _baseWindow;
    private Button _btnOff;
    private Button _btnOn;

    private bool _devModeValue;
    
    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Development Mode";

        return this;
    }

    #endregion
    
    #region Protected Methods

    protected override string BasePath()
    {
        return "MegaPint/User Interface/Windows/Dev Mode";
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();

        content.style.flexGrow = 1f;
        content.style.flexShrink = 1f;

        root.Add(content);

        _btnOn = content.Q <Button>("BTN_On");
        _btnOff = content.Q <Button>("BTN_Off");

        UpdateButtonStyles();

        RegisterCallbacks();
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());

        return _baseWindow != null;
    }

    protected override bool LoadSettings()
    {
        if (!base.LoadSettings())
            return false;

        _devModeValue = _Settings.GetValue("devMode", false);

        return true;
    }

    protected override void RegisterCallbacks()
    {
        _btnOn.clicked += ToggleOn;
        _btnOff.clicked += ToggleOff;
    }

    protected override void UnRegisterCallbacks()
    {
        _btnOn.clicked -= ToggleOn;
        _btnOff.clicked -= ToggleOff;
    }

    #endregion
    
    #region Private Methods

    private void ToggleOff()
    {
        _devModeValue = false;
        _Settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    private void ToggleOn()
    {
        _devModeValue = true;
        _Settings.SetValue("devMode", _devModeValue);

        UpdateButtonStyles();

        MegaPintPackageManager.UpdateAll();
    }

    private void UpdateButtonStyles()
    {
        if (_devModeValue)
        {
            _btnOn.AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);
            _btnOn.AddToClassList(StyleSheetClasses.Background.Color.Identity);
            
            _btnOff.RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
            _btnOff.RemoveFromClassList(StyleSheetClasses.Background.Color.Identity);
        }
        else
        {
            _btnOn.RemoveFromClassList(StyleSheetClasses.Text.Color.ButtonActive);
            _btnOn.RemoveFromClassList(StyleSheetClasses.Background.Color.Identity);
            
            _btnOff.AddToClassList(StyleSheetClasses.Text.Color.ButtonActive);
            _btnOff.AddToClassList(StyleSheetClasses.Background.Color.Identity);
        }
    }

    #endregion
}

}
#endif
