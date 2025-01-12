using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UIPosition : MonoBehaviour
{

    public Tilemap tilemap;        // ���� Tilemap
    public Text scoreText;         // ������ʾ�������ı�
    public Text timerText;         // ������ʾʱ����ı�
    public RectTransform scorePanel; // �������
    public RectTransform timerPanel; // ʱ�����
    public Camera mainCamera;      // �������

    void Start()
    {
        SetUIPositions();
    }

    void SetUIPositions()
    {
        // ��ȡ��ǰ��Ļ�ĳߴ�
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // �������̵�λ�ã�ʹ��λ����Ļ����
        Vector3 screenCenter = new Vector3(screenWidth / 2f, screenHeight / 2f, 0f);
        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(screenCenter);
        worldCenter.z = 0; // ȷ�� Z ��Ϊ 0������Ӱ�� 2D ��Ϸ����Ⱦ

        // �����̵�λ������Ϊ��������λ��
        tilemap.transform.position = worldCenter;

        // �����ı�λ�ã����÷�������λ������Ļ�Ķ��������붥�� 10%��
        float scorePanelY = screenHeight * 0.9f;  // ������Ļ���� 10%
        scorePanel.anchoredPosition = new Vector2(screenWidth * 0.1f, scorePanelY); // �������������

        // ����ʱ������λ������Ļ�ĵײ�������ײ� 10%��
        float timerPanelY = screenHeight * 0.1f;  // ������Ļ�ײ� 10%
        timerPanel.anchoredPosition = new Vector2(screenWidth * 0.9f, timerPanelY); // ����ʱ��������
    }



}
