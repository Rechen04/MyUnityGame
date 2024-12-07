using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CraftingUI : MonoBehaviour
{
    public PlayerStatus playerStatus; // ���� PlayerStatus
    public GameObject recipeButtonPrefab; // �䷽��ťԤ����
    public Transform recipesContainer; // ����䷽��ť������
    public List<CraftingRecipe> recipes; // �����䷽�б�

    private int currentRecipeIndex = 0; // ��ǰ��ʾ���䷽����
    private GameObject currentButton; // ��ǰ��ʾ�İ�ť
    private Coroutine resetTextCoroutine; // ���������ı���Э��

    private void Start()
    {
        ShowCurrentRecipe(); // ��ʾ��ǰ�䷽
    }

    private void ShowCurrentRecipe()
    {
        // ���֮ǰ�İ�ť
        if (currentButton != null)
        {
            Destroy(currentButton);
            currentButton = null; // �������
        }

        // ����䷽�б�
        if (currentRecipeIndex < recipes.Count && recipes[currentRecipeIndex] != null)
        {
            // �����µİ�ť
            currentButton = Instantiate(recipeButtonPrefab, recipesContainer);
            Debug.Log("Button initialized");
            UpdateRecipeButton(currentButton, recipes[currentRecipeIndex]);
        }
        else
        {
            Debug.LogWarning("No more recipes to display or current recipe is null.");
            currentButton = null; // ����ʾ�κΰ�ť
        }
    }

    private void UpdateRecipeButton(GameObject button, CraftingRecipe recipe)
    {
        var textComponents = button.GetComponentsInChildren<TMP_Text>();
        var imageComponents = button.GetComponentsInChildren<Image>();

        // �����䷽���ƺͳɱ�
        textComponents[1].text = recipe.name; // �������� 1 ���䷽����
        textComponents[2].text = $"Cost: {recipe.oreCost}"; // �������� 2 �ǳɱ�
        imageComponents[1].sprite = recipe.icon; // �������� 1 ��ͼ��

        // ���֮ǰ�ļ������������ظ����
        button.GetComponent<Button>().onClick.RemoveAllListeners();

        // Ϊ������ť��ӵ���¼�
        button.GetComponent<Button>().onClick.AddListener(() => {
            if (resetTextCoroutine != null)
            {
                StopCoroutine(resetTextCoroutine); // ֹͣ�κ����е�Э��
            }

            // �����ı�Ϊȷ����Ϣ
            textComponents[0].text = "ȷ��������";

            resetTextCoroutine = StartCoroutine(ResetTextAfterDelay(button, recipe));
        });
    }

    private IEnumerator ResetTextAfterDelay(GameObject button, CraftingRecipe recipe)
    {
        yield return new WaitForSeconds(2f); 

        // �ָ�ԭʼ�ı�
        var textComponents = button.GetComponentsInChildren<TMP_Text>();
        textComponents[0].text = "����";

        // ����������Ʒ
        playerStatus.CraftItem(recipe); // ֱ�ӳ���������Ʒ
        ShowNextRecipe(); // �л�����һ���䷽
    }

    private void ShowNextRecipe()
    {
        // ���µ�ǰ�䷽����
        currentRecipeIndex++;

        // ��ʾ��һ���䷽
        ShowCurrentRecipe();
    }
}

[System.Serializable]
public class CraftingRecipe
{
    public string name; // �䷽����
    public int oreCost; // ���������ʯ����
    public GameObject prefab; // ����������Ԥ����
    public Sprite icon; // �䷽ͼ��
}