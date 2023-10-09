using UnityEngine;
using UnityEditor;

namespace CC.Enum.Editor
{
    /// Author: LinkCC
    /// Created: 10/9/23
    /// Last Edited: 10/9/23
    /// <summary>
    /// Create a Editor Window that allows Developers to create Enum files.
    /// </summary>
    public class EnumEditorWindow : EditorWindow
    {
        protected const string BASE_MENU_PATH = "Window/CC/Enum Creator";

        //this object's Serialization
        SerializedObject m_serialized;

        //User inputed values
        [SerializeField] string m_enumName;
        [SerializeField] string[] m_enumValues;
        [SerializeField] string m_enumNamespace;

        //User GUI interface inputs
        [SerializeField] int m_enumTypeToDeleteIndex = 0;
        [SerializeField] bool m_lockNamespace = false;


        string[] EnumTypes => EnumEditorHandler.GetFileNames();
        string EnumTypeToDelete => EnumTypes[m_enumTypeToDeleteIndex];


        [MenuItem(BASE_MENU_PATH, true, 0)]
        public static void CreateWindow()
        {
            GetWindow<EnumEditorWindow>();
        }

        protected virtual void OnEnable()
        {
            titleContent = new("Enum Creator");
            m_serialized = new SerializedObject(this);
            m_enumNamespace = EnumEditorHandler.STANDARD_NAMESPACE;
        }


        public void OnGUI()
        {
            DrawCreationWidget();

            EditorGUILayout.Space(30);

            DrawDeletionWidget();

            m_serialized.ApplyModifiedProperties();
        }

        /// <summary>
        ///  Draw the area for Creating a Enum
        /// </summary>
        protected virtual void DrawCreationWidget()
        {
            GUILayout.Label("Create Enum", CC.GUI.Styles.Title);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_enumNamespace"));
            m_lockNamespace = GUILayout.Toggle(m_lockNamespace, "Lock");
            if (GUILayout.Button("Refresh")) {m_enumNamespace = EnumEditorHandler.STANDARD_NAMESPACE; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_enumName"));
            EditorGUILayout.PropertyField(m_serialized.FindProperty("m_enumValues"), true);

            if (GUILayout.Button("Create Enum")) { EnumEditorHandler.WriteEnumFile(m_enumName, m_enumValues, m_enumNamespace); ResetCreationValues(); }
        }

        /// <summary>
        ///  Draw the area for Deleting a Enum
        /// </summary>
        protected virtual void DrawDeletionWidget()
        {
            GUILayout.Label("Delete Enum", CC.GUI.Styles.Title);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Enum Type to Delete");
            m_enumTypeToDeleteIndex = EditorGUILayout.Popup(m_enumTypeToDeleteIndex, EnumTypes);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Delete Enum")) { EnumEditorHandler.DeleteEnumFile(EnumTypeToDelete); }
        }

        /// <summary>
        /// Reset input fields to default values
        /// </summary>
        private void ResetCreationValues()
        {
            m_enumName = "";
            m_enumValues = new string[0];
            if(!m_lockNamespace) m_enumNamespace = EnumEditorHandler.STANDARD_NAMESPACE;
        }
    }
}
