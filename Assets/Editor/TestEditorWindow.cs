using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEditorWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Color headerSectionColor = new Color(13f/255, 32f/255, 44f/255, 1f);
    Rect headerSection;
    
    Texture2D ImgButtonTexture;

    [MenuItem("Window/TestEditorWindow")]
    static void OpenWindow()
    {
        TestEditorWindow tes = (TestEditorWindow)GetWindow(typeof(TestEditorWindow));
        //restrain size of the window
        tes.minSize = new Vector2(600, 300);
        tes.Show();
    }
    void OnEnable()
    {
        InitTextures();
    }
    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        ImgButtonTexture = new Texture2D(25, 25);
        for(int i = 0; i < 25; i++)
        {
            for(int j = 0; j < 25; j++)
            {
                ImgButtonTexture.SetPixel(i,j,Color.red);
            }
        }
        ImgButtonTexture.Apply();
    }
    void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        //mouse up 이벤트가 일정 영역에서 발생시 실행
        if(Event.current.type == EventType.MouseUp)
        {
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x >= headerSection.x && mousePos.x <= headerSection.width - headerSection.x &&
                mousePos.y >= headerSection.y && mousePos.y <= headerSection.height - headerSection.y)
            {
                AudioClip ac = (AudioClip)Resources.Load("");
            }
        }
    }
    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;

        GUI.DrawTexture(headerSection, headerSectionTexture);
    }
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.BeginHorizontal();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}