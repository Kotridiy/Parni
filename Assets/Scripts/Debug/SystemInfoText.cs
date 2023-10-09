using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Debug
{
    [RequireComponent(typeof(Text))]
    public class SystemInfoText : MonoBehaviour
    {
        private static SystemInfoText instance;
        private Text text;

        static SystemInfoText Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SystemInfoText>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            text = GetComponent<Text>();
            text.text = "";
        }

        public void ChangeText(string formattedText)
        {
            if (text == null)
            {
                throw new System.NullReferenceException("System text component is empty!");
            }

            if (formattedText != text.text)
            {
                text.text = formattedText;
            }
        }

        public static void ChangeInfoText(string formattedText)
        {
            Instance.ChangeText(formattedText);
        }
    }
}