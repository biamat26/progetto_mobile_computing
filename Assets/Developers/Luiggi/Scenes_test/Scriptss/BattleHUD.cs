using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
   public TMP_Text nameText;
   // public TMP_Text levelText;  <--- L'abbiamo disattivata!

   public Slider hpSlider;

   public void SetHUD(Unit unit)
   {
        nameText.text = unit.unitName;
        // levelText.text = "Lvl " + unit.unitLevel; <--- Disattivata anche questa (era lei che causava l'errore rosso!)
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
   }

   public void SetHP(int hp)
   {
        hpSlider.value = hp;
   }
}
