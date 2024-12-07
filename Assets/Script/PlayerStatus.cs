using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public int oreCount = 0; // 玩家当前的矿石数量
    public GameObject craftingUI; // 引用制作 UI
    public List<TMP_Text> oreCountTexts; // 引用 TMP 文本组件

    // 当可破坏物体被销毁时调用此方法
    public void OnBreakableDestroyed()
    {
        oreCount++; // 增加矿石数量
        UpdateOreUI(); // 更新 UI
    }

    // 更新矿石 UI
    private void UpdateOreUI()
    {
        foreach (var oreCountText in oreCountTexts)
        {
            if (oreCountText != null)
            {
                oreCountText.text = oreCount.ToString(); // 将数字转换为字符串并显示
            }
            Debug.Log($"矿石数量: {oreCount}");
        }
    }

    // 调用此方法尝试制作物品
    public void CraftItem(CraftingRecipe recipe)
    {
        if (recipe != null && oreCount >= recipe.oreCost)
        {
            oreCount -= recipe.oreCost; // 扣除矿石费用
            Instantiate(recipe.prefab, transform.position, Quaternion.identity); // 创建制作的物品
            UpdateOreUI(); // 更新 UI
        }
    }
}