#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Windows
{
    public class MegaPintPackageManagerWindow : MegaPintEditorWindowBase
    {
        private const string ListItemTemplate = "User Interface/Import/MegaPintPackageItem";
        
        private readonly Color _normalColor = new (0.823529422f, 0.823529422f, 0.823529422f);
        private readonly Color _wrongVersionColor = new (0.688679218f,0.149910346f,0.12019401f);
        
        /// <summary> Loaded reference of the uxml </summary>
        private VisualTreeAsset _baseWindow;
        private VisualTreeAsset _listItem;

        private ScrollView _packages;
        private Label _loading;
        
        private GroupBox _rightPane;
        private ListView _list;
        private Label _packageName;
        private Label _version;
        private Label _lastUpdate;
        private Label _unityVersion;
        private Label _megaPintVersion;

        private Button _btnImport;
        private Button _btnRemove;
        private Button _btnUpdate;

        private MegaPintPackageManager.CachedPackages _allPackages;
        private List<MegaPintPackagesData.MegaPintPackageData> _displayedPackages;
        private MegaPintPackagesData.MegaPintPackageData _selectedPackage;

        #region Overrides

        protected override string BasePath() => "User Interface/Import/MegaPintPackageManager";

        public override MegaPintEditorWindowBase ShowWindow()
        {
            titleContent.text = "Import Package";
            return this;
        }
        
        protected override void CreateGUI()
        {
            base.CreateGUI();

            var root = rootVisualElement;

            VisualElement content = _baseWindow.Instantiate();

            _list = content.Q<ListView>("MainList");

            _list.makeItem = () => _listItem.Instantiate();

            _list.bindItem = UpdateItem;

            _list.unbindItem = (element, _) => element.Clear();

            _list.onSelectedIndicesChange += _ => UpdateRightPane();

            _packages = content.Q<ScrollView>("Packages");
            _loading = content.Q<Label>("Loading");
            
            _loading.style.display = DisplayStyle.Flex;
            _packages.style.display = DisplayStyle.None;

            _rightPane = content.Q<GroupBox>("RightPane");
            _packageName = _rightPane.Q<Label>("PackageName");
            _version = _rightPane.Q<Label>("NewestVersion");
            _lastUpdate = _rightPane.Q<Label>("LastUpdate");
            _unityVersion = _rightPane.Q<Label>("UnityVersion");
            _megaPintVersion = _rightPane.Q<Label>("MegaPintVersion");

            _btnImport = _rightPane.Q<Button>("BTN_Import");
            _btnRemove = _rightPane.Q<Button>("BTN_Remove");
            _btnUpdate = _rightPane.Q<Button>("BTN_Update");

            _btnImport.clicked += OnImport;
            _btnRemove.clicked += OnRemove;
            _btnUpdate.clicked += OnUpdate;
            
            UpdateRightPane();

            InitializeList();

            root.Add(content);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _btnImport.clicked -= OnImport;
            _btnRemove.clicked -= OnRemove;
            _btnUpdate.clicked -= OnUpdate;
        }

        protected override bool LoadResources()
        {
            _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
            _listItem = Resources.Load<VisualTreeAsset>(ListItemTemplate);
            return _baseWindow != null && _listItem != null;
        }

        protected override void LoadSettings() { }

        #endregion

        private void UpdateItem(VisualElement element, int index)
        {
            var package = _displayedPackages[index];

            element.Q<Label>("PackageName").text = package.PackageNiceName;
            
            var version = element.Q<Label>("Version");
            version.text = _allPackages.CurrentVersion(package.PackageKey);

            version.style.display = _allPackages.IsImported(package.PackageKey) ? DisplayStyle.Flex : DisplayStyle.None;
            version.style.color = _allPackages.NeedsUpdate(package.PackageKey) ? _wrongVersionColor : _normalColor;
        }

        private void UpdateRightPane()
        {
            var content = _rightPane.Q<GroupBox>("Content");
            var index = _list.selectedIndex;

            if (index < 0)
            {
                content.style.display = DisplayStyle.None;
                return;
            }
            
            content.style.display = DisplayStyle.Flex;
            
            var package = _displayedPackages[index];
            _packageName.text = package.PackageNiceName;
            _version.text = package.Version;
            _lastUpdate.text = package.LastUpdate;
            _unityVersion.text = package.UnityVersion;
            _megaPintVersion.text = package.MegaPintVersion;

            var isImported = _allPackages.IsImported(package.PackageKey);
            _btnImport.style.display = isImported ? DisplayStyle.None : DisplayStyle.Flex;
            _btnRemove.style.display = isImported ? DisplayStyle.Flex : DisplayStyle.None;
            _btnUpdate.style.display = isImported && _allPackages.NeedsUpdate(package.PackageKey)
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }
        
        private void InitializeList()
        {
            _allPackages = new MegaPintPackageManager.CachedPackages(_loading, () =>
            {
                SetDisplayedPackages("");
            });
        }

        private void SetDisplayedPackages(string searchString)
        {
            _loading.style.display = DisplayStyle.None;
            _packages.style.display = DisplayStyle.Flex;
            
            _displayedPackages = searchString.Equals("") ? 
                _allPackages.ToDisplay() :
                _allPackages.ToDisplay().Where(package => package.PackageNiceName.StartsWith(searchString)).ToList();
            
            _list.itemsSource = _displayedPackages;
            _list.RefreshItems();
        }
        
        private void OnImport()
        {
            MegaPintPackageManager.OnSuccess += OnImportSuccess;
            MegaPintPackageManager.OnFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex].GitUrl);
        }

        private static void OnImportSuccess()
        {
            MegaPintPackageManager.OnSuccess -= OnImportSuccess;
            MegaPintPackageManager.OnFailure -= OnFailure;
        }

        private void OnRemove()
        {
            MegaPintPackageManager.OnSuccess += OnRemoveSuccess;
            MegaPintPackageManager.OnFailure += OnFailure;
            MegaPintPackageManager.Remove(_displayedPackages[_list.selectedIndex].PackageName);
        }

        private static void OnRemoveSuccess()
        {
            MegaPintPackageManager.OnSuccess -= OnRemoveSuccess;
            MegaPintPackageManager.OnFailure -= OnFailure;
        }

        private void OnUpdate()
        {
            MegaPintPackageManager.OnSuccess += OnUpdateSuccess;
            MegaPintPackageManager.OnFailure += OnFailure;
            MegaPintPackageManager.AddEmbedded(_displayedPackages[_list.selectedIndex].GitUrl);
        }

        private static void OnUpdateSuccess()
        {
            MegaPintPackageManager.OnSuccess -= OnUpdateSuccess;
            MegaPintPackageManager.OnFailure -= OnFailure;
        }

        private static void OnFailure(string error) => Debug.LogError(error);
    }
}
#endif