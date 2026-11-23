using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public class PickUnitView : MonoBehaviour
    {
        public Button pickUnitButton;
        public RectTransform statsPanel;
        
        public void OpenStats(){
            statsPanel.gameObject.SetActive(true);
        }
        
        public void CloseStats(){
            statsPanel.gameObject.SetActive(false);
        }
    }
    

}