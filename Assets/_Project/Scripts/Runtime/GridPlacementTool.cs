using UnityEngine;
using UnityEditor;

public class GridPlacementTool : EditorWindow
{
  
    private GameObject blockPrefab;
    private float cellSize = 1f;
    private float currentRotation = 0f;
    private bool isPlacementModeActive = false;

   
    [MenuItem("Tools/Мой Строитель Сеток")]
    public static void ShowWindow()
    {
        GetWindow<GridPlacementTool>("Строитель Сеток");
    }

   
    private void OnGUI()
    {
        GUILayout.Label("Настройки строительства", EditorStyles.boldLabel);

      
        blockPrefab = (GameObject)EditorGUILayout.ObjectField("Префаб блока", blockPrefab, typeof(GameObject), false);

        
        cellSize = EditorGUILayout.FloatField("Размер клетки сетки", cellSize);

        GUILayout.Space(10);

        
        string buttonText = isPlacementModeActive ? "ВЫКЛЮЧИТЬ РЕЖИМ СТРОИТЕЛЬСТВА" : "ВКЛЮЧИТЬ РЕЖИМ СТРОИТЕЛЬСТВА";
        if (GUILayout.Button(buttonText, GUILayout.Height(40)))
        {
            isPlacementModeActive = !isPlacementModeActive;
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Инструкция:\n1. Включи режим строительства.\n2. Зажми [Shift] на сцене и кликай ЛКМ, чтобы ставить блок.\n3. Нажимай [R], чтобы повернуть блок на 90 градусов.", MessageType.Info);
    }

  
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

   
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacementModeActive || blockPrefab == null) return;

        Event e = Event.current;

        
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
        {
            currentRotation += 90f;
            if (currentRotation >= 360f) currentRotation = 0f;
            e.Use(); 
        }

       
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enterDistance))
        {
            Vector3 worldPos = ray.GetPoint(enterDistance);

         
            Vector3 snappedPos = SnapToGrid(worldPos, cellSize);

      
            Handles.color = Color.cyan;
            Handles.DrawWireCube(snappedPos, new Vector3(cellSize, 0.1f, cellSize));

          
            if (e.modifiers == EventModifiers.Shift && e.type == EventType.MouseDown && e.button == 0)
            {
             
                if (!IsCellOccupied(snappedPos))
                {
                    
                    GameObject newBlock = (GameObject)PrefabUtility.InstantiatePrefab(blockPrefab);
                    newBlock.transform.position = snappedPos;
                    newBlock.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

                    Undo.RegisterCreatedObjectUndo(newBlock, "Place Block via Tool");

                    e.Use(); 
                }
            }
        }

       
        sceneView.Repaint();
    }

    
    private Vector3 SnapToGrid(Vector3 position, float size)
    {
        return new Vector3(
            Mathf.Floor(position.x / size) * size + (size / 2f),
            0,
            Mathf.Floor(position.z / size) * size + (size / 2f)
        );
    }

   
    private bool IsCellOccupied(Vector3 position)
    {  
        Collider[] colliders = Physics.OverlapSphere(position, cellSize * 0.4f);
        return colliders.Length > 0;
    }
}