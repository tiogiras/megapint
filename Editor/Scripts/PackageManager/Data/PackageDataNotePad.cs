﻿#if UNITY_EDITOR
namespace Editor.Scripts.PackageManager.Data
{

public static class PackageDataNotePad
{
    #region Public Methods

    public static MegaPintPackagesData.MegaPintPackageData Get()
    {
        return new MegaPintPackagesData.MegaPintPackageData
        {
            packageKey = MegaPintPackagesData.PackageKey.NotePad,
            packageName = "com.tiogiras.megapint-notepad",
            packageNiceName = "NotePad",
            gitUrl = "https://github.com/tiogiras/MegaPint-NotePad.git#v1.0.0",
            version = "1.0.0",
            lastUpdate = "25.01.2024",
            unityVersion = "2022.3.15f1 or higher",
            megaPintVersion = "1.1.0 or higher",
            infoText = "NotePad allows you to make and edit notes directly on a GameObjects MonoBehaviour."
        };
    }

    #endregion
}

}
#endif
