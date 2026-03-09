using System.Security;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public TMP_Text nameText;
   public TMP_Text levelText;

   public Slider hpSlider;

   public void SetHUD(Unit unit){
    nameText.text = unit.unitName;
    levelText.text = "Lvl " + unit.unitLevel;
    hpSlider.maxValue = unit.maxHP;
    hpSlider.value = unit.currentHP;
   }

   public void SetHP(int hp){
    hpSlider.value = hp;
   }
}
