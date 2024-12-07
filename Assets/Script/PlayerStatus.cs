using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public int oreCount = 0; // ��ҵ�ǰ�Ŀ�ʯ����
    public GameObject craftingUI; // �������� UI
    public List<TMP_Text> oreCountTexts; // ���� TMP �ı����

    // �����ƻ����屻����ʱ���ô˷���
    public void OnBreakableDestroyed()
    {
        oreCount++; // ���ӿ�ʯ����
        UpdateOreUI(); // ���� UI
    }

    // ���¿�ʯ UI
    private void UpdateOreUI()
    {
        foreach (var oreCountText in oreCountTexts)
        {
            if (oreCountText != null)
            {
                oreCountText.text = oreCount.ToString(); // ������ת��Ϊ�ַ�������ʾ
            }
            Debug.Log($"��ʯ����: {oreCount}");
        }
    }

    // ���ô˷�������������Ʒ
    public void CraftItem(CraftingRecipe recipe)
    {
        if (recipe != null && oreCount >= recipe.oreCost)
        {
            oreCount -= recipe.oreCost; // �۳���ʯ����
            Instantiate(recipe.prefab, transform.position, Quaternion.identity); // ������������Ʒ
            UpdateOreUI(); // ���� UI
        }
    }
}