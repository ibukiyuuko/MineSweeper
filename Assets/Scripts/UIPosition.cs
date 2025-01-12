using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UIPosition : MonoBehaviour
{

    public Tilemap tilemap;        // 雷盘 Tilemap
    public Text scoreText;         // 用于显示分数的文本
    public Text timerText;         // 用于显示时间的文本
    public RectTransform scorePanel; // 分数面板
    public RectTransform timerPanel; // 时间面板
    public Camera mainCamera;      // 主摄像机

    void Start()
    {
        SetUIPositions();
    }

    void SetUIPositions()
    {
        // 获取当前屏幕的尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 计算雷盘的位置，使其位于屏幕中心
        Vector3 screenCenter = new Vector3(screenWidth / 2f, screenHeight / 2f, 0f);
        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(screenCenter);
        worldCenter.z = 0; // 确保 Z 轴为 0，避免影响 2D 游戏的渲染

        // 将雷盘的位置设置为世界中心位置
        tilemap.transform.position = worldCenter;

        // 计算文本位置，设置分数面板的位置在屏幕的顶部（距离顶部 10%）
        float scorePanelY = screenHeight * 0.9f;  // 距离屏幕顶部 10%
        scorePanel.anchoredPosition = new Vector2(screenWidth * 0.1f, scorePanelY); // 假设分数面板居左

        // 设置时间面板的位置在屏幕的底部（距离底部 10%）
        float timerPanelY = screenHeight * 0.1f;  // 距离屏幕底部 10%
        timerPanel.anchoredPosition = new Vector2(screenWidth * 0.9f, timerPanelY); // 假设时间面板居右
    }



}
