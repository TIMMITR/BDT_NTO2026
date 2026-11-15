using UnityEngine;

namespace Features.ButtonsPack
{
    public class ButtonView : MonoBehaviour
    {
        public ButtonType type;
    }

    public enum ButtonType
    {
        Start,
        Exit,
        Setting // если будет время
    }
}
