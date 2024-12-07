using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CraftingUI : MonoBehaviour
{
    public PlayerStatus playerStatus; // 引用 PlayerStatus
    public GameObject recipeButtonPrefab; // 配方按钮预制体
    public Transform recipesContainer; // 存放配方按钮的容器
    public List<CraftingRecipe> recipes; // 所有配方列表

    private int currentRecipeIndex = 0; // 当前显示的配方索引
    private GameObject currentButton; // 当前显示的按钮
    private Coroutine resetTextCoroutine; // 用于重置文本的协程

    private void Start()
    {
        ShowCurrentRecipe(); // 显示当前配方
    }

    private void ShowCurrentRecipe()
    {
        // 清除之前的按钮
        if (currentButton != null)
        {
            Destroy(currentButton);
            currentButton = null; // 清除引用
        }

        // 检查配方列表
        if (currentRecipeIndex < recipes.Count && recipes[currentRecipeIndex] != null)
        {
            // 创建新的按钮
            currentButton = Instantiate(recipeButtonPrefab, recipesContainer);
            Debug.Log("Button initialized");
            UpdateRecipeButton(currentButton, recipes[currentRecipeIndex]);
        }
        else
        {
            Debug.LogWarning("No more recipes to display or current recipe is null.");
            currentButton = null; // 不显示任何按钮
        }
    }

    private void UpdateRecipeButton(GameObject button, CraftingRecipe recipe)
    {
        var textComponents = button.GetComponentsInChildren<TMP_Text>();
        var imageComponents = button.GetComponentsInChildren<Image>();

        // 设置配方名称和成本
        textComponents[1].text = recipe.name; // 假设索引 1 是配方名称
        textComponents[2].text = $"Cost: {recipe.oreCost}"; // 假设索引 2 是成本
        imageComponents[1].sprite = recipe.icon; // 假设索引 1 是图标

        // 清除之前的监听器，避免重复添加
        button.GetComponent<Button>().onClick.RemoveAllListeners();

        // 为制作按钮添加点击事件
        button.GetComponent<Button>().onClick.AddListener(() => {
            if (resetTextCoroutine != null)
            {
                StopCoroutine(resetTextCoroutine); // 停止任何现有的协程
            }

            // 更改文本为确认信息
            textComponents[0].text = "确认制作？";

            resetTextCoroutine = StartCoroutine(ResetTextAfterDelay(button, recipe));
        });
    }

    private IEnumerator ResetTextAfterDelay(GameObject button, CraftingRecipe recipe)
    {
        yield return new WaitForSeconds(2f); 

        // 恢复原始文本
        var textComponents = button.GetComponentsInChildren<TMP_Text>();
        textComponents[0].text = "制作";

        // 继续制作物品
        playerStatus.CraftItem(recipe); // 直接尝试制作物品
        ShowNextRecipe(); // 切换到下一个配方
    }

    private void ShowNextRecipe()
    {
        // 更新当前配方索引
        currentRecipeIndex++;

        // 显示下一个配方
        ShowCurrentRecipe();
    }
}

[System.Serializable]
public class CraftingRecipe
{
    public string name; // 配方名称
    public int oreCost; // 制作所需矿石数量
    public GameObject prefab; // 制作的物体预制体
    public Sprite icon; // 配方图标
}