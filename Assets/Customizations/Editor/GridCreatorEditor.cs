using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GridCreator))]

public class GridCreatorEditor : Editor
{
    // serialize property
    SerializedProperty objectList, objectNum,
        mirrorAxisX, mirrorAxisY, mirrorAxisZ,
        row, column, layer, 
        baseSpace, spaceX, spaceY, spaceZ,
        baseSpaceChange, spaceXChange, spaceZChange, spaceYChangeX, spaceYChangeZ,
        rotateXIdentical, rotateYIdentical, rotateZIdentical,
        rotateX, rotateY, rotateZ,
        queueAxisX, queueAxisY, queueAxisZ;

    List<GameObject> selectedObject = new List<GameObject>();
    // color
    List<float> r = new List<float>(), g = new List<float>(), b = new List<float>();
    // size
    List<float> baseSize = new List<float>(), 
        sizeX = new List<float>(), sizeY = new List<float>(), sizeZ = new List<float>();

    // material
    List<bool> haveMaterial = new List<bool>();

    // find the target object
    GridCreator gridCreator;

    // foldout property
    bool foldoutColor = false, foldoutSize = false, foldoutGrid = false,
        foldoutSpace = false, foldoutRotate = false;

    // changed in game
    [HideInInspector]
    public bool valueChanged = false;

    // GUI background color save
    public Color originalColor;

    // set up serialized object
    public void OnEnable()
    {
        objectList = serializedObject.FindProperty("objectList");
        objectNum = serializedObject.FindProperty("objectNum");

        mirrorAxisX = serializedObject.FindProperty("mirrorAxisX");
        mirrorAxisY = serializedObject.FindProperty("mirrorAxisY");
        mirrorAxisZ = serializedObject.FindProperty("mirrorAxisZ");

        row = serializedObject.FindProperty("row");
        column = serializedObject.FindProperty("column");
        layer = serializedObject.FindProperty("layer");

        baseSpace = serializedObject.FindProperty("baseSpace");
        spaceX = serializedObject.FindProperty("spaceX");
        spaceY = serializedObject.FindProperty("spaceY");
        spaceZ = serializedObject.FindProperty("spaceZ");

        baseSpaceChange = serializedObject.FindProperty("baseSpaceChange");
        spaceXChange = serializedObject.FindProperty("spaceXChange");
        spaceZChange = serializedObject.FindProperty("spaceZChange");
        spaceYChangeX = serializedObject.FindProperty("spaceYChangeX");
        spaceYChangeZ = serializedObject.FindProperty("spaceYChangeZ");

        rotateX = serializedObject.FindProperty("rotateX");
        rotateY = serializedObject.FindProperty("rotateY");
        rotateZ = serializedObject.FindProperty("rotateZ");
        rotateXIdentical = serializedObject.FindProperty("rotateXIdentical");
        rotateYIdentical = serializedObject.FindProperty("rotateYIdentical");
        rotateZIdentical = serializedObject.FindProperty("rotateZIdentical");

        queueAxisX = serializedObject.FindProperty("queueAxisX");
        queueAxisY = serializedObject.FindProperty("queueAxisY");
        queueAxisZ = serializedObject.FindProperty("queueAxisZ");

    }


    public override void OnInspectorGUI()
    { 
        // do this at the begining of OnInspectorGUI
        serializedObject.Update();

        // set target GridCreator
        GridCreator gridCreator = (GridCreator)target;

        // display the editable value panel
        EditorGUILayout.PropertyField(objectList);

        // set list
        selectedObject = gridCreator.selectedObject;
        r = gridCreator.r; g = gridCreator.g; b = gridCreator.b;
        baseSize = gridCreator.baseSize;
        sizeX = gridCreator.sizeX; sizeY = gridCreator.sizeY; sizeZ = gridCreator.sizeZ;
        haveMaterial = gridCreator.haveMaterial;

        // set variable list
        if (selectedObject.Count <= 0)
        {
            objectList.intValue = 1;
            selectedObject.Add(null);
        }
        if (r.Count <= 0)
        {
            r.Add(0); g.Add(0); b.Add(0);
        }
        if (baseSize.Count <= 0)
        {
            baseSize.Add(1); sizeX.Add(1); sizeY.Add(1); sizeZ.Add(1);
        }
        if (haveMaterial.Count <= 0)
        {
            haveMaterial.Add(false);
        }

        // update list when list change
        if (objectList.intValue > 0 && selectedObject.Count != objectList.intValue)
        {
            // object
            if (selectedObject.Count < objectList.intValue)
            {
                while (selectedObject.Count != objectList.intValue)
                {
                    selectedObject.Add(null);
                }
            }
            else
            {
                while (selectedObject.Count != objectList.intValue)
                {
                    selectedObject.Remove(selectedObject[selectedObject.Count - 1]);
                }
            }
        }
        // update color list
        if (r.Count < selectedObject.Count)
        {
            while (r.Count != selectedObject.Count)
            {
                r.Add(0); g.Add(0); b.Add(0);
            }
        }
        else
        {
            while (r.Count != selectedObject.Count)
            {
                r.Remove(r[r.Count - 1]);
                g.Remove(g[g.Count - 1]);
                b.Remove(b[b.Count - 1]);
            }
        }
        // update size list
        if (baseSize.Count < selectedObject.Count)
        {
            while (baseSize.Count != selectedObject.Count)
            {
                baseSize.Add(1); sizeX.Add(1); sizeY.Add(1); sizeZ.Add(1);
            }
        }
        else
        {
            while (baseSize.Count != selectedObject.Count)
            {
                baseSize.Remove(baseSize[baseSize.Count - 1]);
                sizeX.Remove(sizeX[sizeX.Count - 1]);
                sizeY.Remove(sizeY[sizeY.Count - 1]);
                sizeZ.Remove(sizeZ[sizeZ.Count - 1]);
            }
        }

        // update material list
        if (haveMaterial.Count < selectedObject.Count)
        {
            while (haveMaterial.Count != selectedObject.Count)
            {
                haveMaterial.Add(false);
            }
        }
        else
        {
            while (haveMaterial.Count != selectedObject.Count)
            {
                haveMaterial.Remove(haveMaterial[haveMaterial.Count - 1]);
            }
        }

        // show object field
        if (objectList.intValue > 0)
        {
            for (int i = 0; i < objectList.intValue; i++)
            {
                selectedObject[i] = EditorGUILayout.ObjectField("Selected Object " + i,
                selectedObject[i], typeof(GameObject), true) as GameObject;
            }
        }

        // show object number slider when object list more than 1
        if (objectList.intValue > 1)
        {
            EditorGUILayout.IntSlider(objectNum, 0, objectList.intValue - 1);
        }
        else
        {
            objectNum.intValue = 0;
        }
        
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(Screen.width), GUILayout.ExpandHeight(true));

        // check if object list exist
        if(objectList.intValue > 0)
        {
            if(selectedObject[objectNum.intValue] != null)
            {
                if (haveMaterial[objectNum.intValue] == true)
                    ColorSetting();

                SizeSetting();
            }          
            GridSetting();
            ResetButton();
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        // create grid and update list when gui change
        if (GUI.changed)
        {
            // update list when list change
            if (objectList.intValue > 0 && selectedObject.Count != objectList.intValue)
            {
                // object
                if (selectedObject.Count < objectList.intValue)
                {
                    while (selectedObject.Count != objectList.intValue)
                    {
                        selectedObject.Add(null);
                    }
                }
                else
                {
                    while (selectedObject.Count != objectList.intValue)
                    {
                        selectedObject.Remove(selectedObject[selectedObject.Count - 1]);
                    }
                }
            }
            // update color list
            if (r.Count < selectedObject.Count)
            {
                while (r.Count != selectedObject.Count)
                {
                    r.Add(0); g.Add(0); b.Add(0);
                }
            }
            else
            {
                while (r.Count != selectedObject.Count)
                {
                    r.Remove(r[r.Count - 1]);
                    g.Remove(g[g.Count - 1]);
                    b.Remove(b[b.Count - 1]);
                }
            }
            // update size list
            if (baseSize.Count < selectedObject.Count)
            {
                while (baseSize.Count != selectedObject.Count)
                {
                    baseSize.Add(1); sizeX.Add(1); sizeY.Add(1); sizeZ.Add(1);
                }
            }
            else
            {
                while (baseSize.Count != selectedObject.Count)
                {
                    baseSize.Remove(baseSize[baseSize.Count - 1]);
                    sizeX.Remove(sizeX[sizeX.Count - 1]);
                    sizeY.Remove(sizeY[sizeY.Count - 1]);
                    sizeZ.Remove(sizeZ[sizeZ.Count - 1]);
                }
            }

            // update material list
            if (haveMaterial.Count < selectedObject.Count)
            {
                while (haveMaterial.Count != selectedObject.Count)
                {
                    haveMaterial.Add(false);
                }
            }
            else
            {
                while (haveMaterial.Count != selectedObject.Count)
                {
                    haveMaterial.Remove(haveMaterial[haveMaterial.Count - 1]);
                }
            }
            gridCreator.CreateGrid();
        }
    }

    void ColorSetting()
    {
        // color
        foldoutColor = EditorGUILayout.Foldout(foldoutColor, "Color");
        if (foldoutColor)
        {
            r[objectNum.intValue] = EditorGUILayout.Slider("Red", r[objectNum.intValue], 0f, 1f);
            g[objectNum.intValue] = EditorGUILayout.Slider("Green", g[objectNum.intValue], 0f, 1f);
            b[objectNum.intValue] = EditorGUILayout.Slider("Blue", b[objectNum.intValue], 0f, 1f);

            Color guiColor = GUI.color;

            GUI.color = new Color(r[objectNum.intValue], g[objectNum.intValue], b[objectNum.intValue]);

            GUILayout.Label("Preview");
            GUILayout.Label("");
            if(selectedObject.Count == 1)
            {
                GUI.DrawTexture(new Rect(20, 150, Screen.width * 0.9f, 8), EditorGUIUtility.whiteTexture);
            }
            else
            {
                GUI.DrawTexture(new Rect(20, 150 + (20 * selectedObject.Count + 1), Screen.width * 0.9f, 8),
                EditorGUIUtility.whiteTexture);
            }         

            GUI.color = guiColor;

            if (GUILayout.Button("Randomize"))
            {
                r[objectNum.intValue] = Random.Range(0f, 1f);
                g[objectNum.intValue] = Random.Range(0f, 1f);
                b[objectNum.intValue] = Random.Range(0f, 1f);
            }
            //Reset Color
            if (GUILayout.Button("Reset Color"))
            {
                r[objectNum.intValue] = 0; g[objectNum.intValue] = 0; b[objectNum.intValue] = 0;
            }
        }

    }

    void SizeSetting()
    {
        // size
        foldoutSize = EditorGUILayout.Foldout(foldoutSize, "Size");
        if (foldoutSize)
        {
            baseSize[objectNum.intValue] = EditorGUILayout.FloatField("Base Size", baseSize[objectNum.intValue]);
            sizeX[objectNum.intValue] = EditorGUILayout.Slider("SizeX", sizeX[objectNum.intValue], 0f, 10f);
            sizeY[objectNum.intValue] = EditorGUILayout.Slider("SizeY", sizeY[objectNum.intValue], 0f, 10f);
            sizeZ[objectNum.intValue] = EditorGUILayout.Slider("SizeZ", sizeZ[objectNum.intValue], 0f, 10f);
            if (GUILayout.Button("Randomize"))
            {
                sizeX[objectNum.intValue] = Random.Range(0f, 10f);
                sizeY[objectNum.intValue] = Random.Range(0f, 10f);
                sizeZ[objectNum.intValue] = Random.Range(0f, 10f);
            }
            if (GUILayout.Button("Reset Size"))
            {
                baseSize[objectNum.intValue] = 1; sizeX[objectNum.intValue] = 1; 
                sizeY[objectNum.intValue] = 1; sizeZ[objectNum.intValue] = 1;
            }
        }
    }

    void GridSetting()
    {
        // grid
        foldoutGrid = EditorGUILayout.Foldout(foldoutGrid, "Grid");
        if (foldoutGrid)
        {
            EditorGUILayout.PropertyField(row);
            EditorGUILayout.PropertyField(column);
            EditorGUILayout.PropertyField(layer);
            GUILayout.Label(" ");

            GUILayout.Label("Mirror");
            EditorGUILayout.PropertyField(mirrorAxisX);
            EditorGUILayout.PropertyField(mirrorAxisY);
            EditorGUILayout.PropertyField(mirrorAxisZ);

            if (GUILayout.Button("Reset Grid"))
            {

                mirrorAxisX.boolValue = false; mirrorAxisY.boolValue = false; mirrorAxisZ.boolValue = false;
                row.intValue = 1; column.intValue = 1; layer.intValue = 1;
            }

            // space
            foldoutSpace = EditorGUILayout.Foldout(foldoutSpace, "Space");
            if (foldoutSpace)
            {
                EditorGUILayout.PropertyField(baseSpace);
                EditorGUILayout.Slider(spaceX, 0f, 1f);
                EditorGUILayout.Slider(spaceY, 0f, 1f);
                EditorGUILayout.Slider(spaceZ, 0f, 1f);

                if (GUILayout.Button("Randomize"))
                {
                    spaceX.floatValue = Random.Range(0f, 1f);
                    spaceY.floatValue = Random.Range(0f, 1f);
                    spaceZ.floatValue = Random.Range(0f, 1f);
                }
                if (GUILayout.Button("Reset Space"))
                {
                    baseSpace.floatValue = 3; spaceX.floatValue = 1; spaceY.floatValue = 1; spaceZ.floatValue = 1;
                }

                EditorGUILayout.PropertyField(baseSpaceChange);
                EditorGUILayout.Slider(spaceXChange, -1f, 1f);
                EditorGUILayout.Slider(spaceZChange, -1f, 1f);
                EditorGUILayout.Slider(spaceYChangeX, -1f, 1f);
                EditorGUILayout.Slider(spaceYChangeZ, -1f, 1f);

                if (GUILayout.Button("Randomize"))
                {
                    spaceXChange.floatValue = Random.Range(-1f, 1f);
                    spaceZChange.floatValue = Random.Range(-1f, 1f);
                    spaceYChangeX.floatValue = Random.Range(-1f, 1f);
                    spaceYChangeZ.floatValue = Random.Range(-1f, 1f);
                }
                if (GUILayout.Button("Reset Space Change"))
                {
                    baseSpaceChange.floatValue = 0; spaceXChange.floatValue = 0; spaceZChange.floatValue = 0;
                    spaceYChangeX.floatValue = 0; spaceYChangeZ.floatValue = 0;
                }
            }

            // rotation
            foldoutRotate = EditorGUILayout.Foldout(foldoutRotate, "Rotation");
            if (foldoutRotate)
            {                
                EditorGUILayout.PropertyField(rotateXIdentical);
                EditorGUILayout.Slider(rotateX, 0f, 360f);
                EditorGUILayout.PropertyField(rotateYIdentical);
                EditorGUILayout.Slider(rotateY, 0f, 360f);
                EditorGUILayout.PropertyField(rotateZIdentical);

                EditorGUILayout.Slider(rotateZ, 0f, 360f);
                if (GUILayout.Button("Randomize"))
                {
                    rotateX.floatValue = Random.Range(0f, 360f);
                    rotateY.floatValue = Random.Range(0f, 360f);
                    rotateZ.floatValue = Random.Range(0f, 360f);
                }
                if (GUILayout.Button("Reset Rotote"))
                {
                    rotateXIdentical.boolValue = false;
                    rotateYIdentical.boolValue = false;
                    rotateZIdentical.boolValue = false;
                    rotateX.floatValue = 0; rotateY.floatValue = 0; rotateZ.floatValue = 0;
                }
            }

            // queue
            GUILayout.Label(" ");
            GUILayout.Label("Queue");
            EditorGUILayout.PropertyField(queueAxisX);
            EditorGUILayout.PropertyField(queueAxisY);
            EditorGUILayout.PropertyField(queueAxisZ);
        }
    }

    void ResetButton()
    {
        // reset variable
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset All"))
        {
            r[objectNum.intValue] = 0; g[objectNum.intValue] = 0; b[objectNum.intValue] = 0;

            baseSize[objectNum.intValue] = 1; sizeX[objectNum.intValue] = 1; 
            sizeY[objectNum.intValue] = 1; sizeZ[objectNum.intValue] = 1;

            row.intValue = 1; column.intValue = 1; layer.intValue = 1;

            baseSpace.floatValue = 3; spaceX.floatValue = 1; spaceY.floatValue = 1; spaceZ.floatValue = 1;

            baseSpaceChange.floatValue = 0; spaceXChange.floatValue = 0; spaceZChange.floatValue = 0;
            spaceYChangeX.floatValue = 0; spaceYChangeZ.floatValue = 0;

            rotateXIdentical.boolValue = false; 
            rotateYIdentical.boolValue = false;
            rotateZIdentical.boolValue = false;
            rotateX.floatValue = 0; rotateY.floatValue = 0; rotateZ.floatValue = 0;
            queueAxisX.boolValue = false;
            queueAxisY.boolValue = false;
            queueAxisZ.boolValue = false;
        }
        GUILayout.EndHorizontal();
    }
}
