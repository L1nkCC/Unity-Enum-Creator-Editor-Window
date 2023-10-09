using UnityEngine;

namespace CC.GUI
{

    /// Author: LinkCC
    /// Created: 10/9/23
    /// Last Edited: 10/9/23
    /// <summary>
    /// Hold Information Concerning Styling in displays
    /// </summary>
    public static class Styles
    {

        /// <summary>
        /// Style used for Titles
        /// </summary>
        public static GUIStyle Title 
        {
            get
            {
                GUIStyle _title = new();
                _title.fontSize = 30;
                _title.alignment = TextAnchor.MiddleCenter;
                _title.fontStyle = FontStyle.Bold;
                _title.normal.textColor = Color.grey;
                return _title;
            }
        }
    }
}
