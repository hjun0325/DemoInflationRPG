using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_EquipmentPanel : UI_Popup
{
    // UI 참조 변수들.
    [SerializeField] private Button weaponButton;
    [SerializeField] private Image weaponImage;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text addATKText;
    [SerializeField] private TMP_Text mulATKText;

    [SerializeField] private Button armorButton;
    [SerializeField] private Image armorImage;
    [SerializeField] private TMP_Text armorName;
    [SerializeField] private TMP_Text addDEFText;
    [SerializeField] private TMP_Text mulDEFText;

    [SerializeField] private Button accessoryButton1;
    [SerializeField] private Image accessoryImage1;
    [SerializeField] private TMP_Text accessoryName1;
    [SerializeField] private TMP_Text addStatText1;
    [SerializeField] private TMP_Text mulStatText1;

    [SerializeField] private Button accessoryButton2;
    [SerializeField] private Image accessoryImage2;
    [SerializeField] private TMP_Text accessoryName2;
    [SerializeField] private TMP_Text addStatText2;
    [SerializeField] private TMP_Text mulDStatext2;


    public void OnClick_Back()
    {
        Hide();
    }
}
