using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeedGridCreator : EditorWindow
{
    // scrollable window
    public Vector2 scrollPosition, scrollPositionTop;

    // selected object
    public int objectList = 1;
    public List<GameObject> selectedObject;
    public int objectNum = 0;

    // grid start transform
    public Vector3 objectTransform;

    // color
    List<float> r = new List<float>(), g = new List<float>(), b = new List<float>();

    // size
    List<float> baseSize = new List<float>(),
        sizeX = new List<float>(), sizeY = new List<float>(), sizeZ = new List<float>();

    // grid
    public bool mirrorAxisX = false, mirrorAxisY = false, mirrorAxisZ = false;
    public int row = 1, column = 1, layer = 1;

    // space
    public float baseSpace = 3;
    public float spaceX = 1, spaceY = 1, spaceZ =1;

    // changing space
    public float baseSpaceChange;
    public float spaceXChange, spaceZChange, spaceYChangeX, spaceYChangeZ;

    // rotate
    public float rotateX, rotateY, rotateZ;

    // check if rotation is identical
    public bool rotateXIdentical = false, rotateYIdentical = false, rotateZIdentical = false;

    // object queue
    public bool queueAxisX, queueAxisY, queueAxisZ;

    // setting button
    bool objectSetting = false, gridSetting = false, windowSetting = false; 

    // GUI color save
    public Color originalColor;

    // window color setting
    public bool defaultColor = true;
    public float bgR, bgG, bgB, bgA = 0.2f;

    // add a menu item to add this component to all children
    [MenuItem("SpeedGridCreator/Speed Grid Creator")]
    static void OpenWindow()
    {
        SpeedGridCreator window = (SpeedGridCreator)GetWindow(typeof(SpeedGridCreator));
        window.minSize = new Vector2(600, 300);
    }

    // add a menu item to add this component to all children
    [MenuItem("SpeedGridCreator/Add GridCreator As Script")]
    static void AddEditor()
    {
        // get all the selected transforms
        Transform[] transforms = Selection.GetTransforms(
        SelectionMode.Deep |
        SelectionMode.ExcludePrefab |
        SelectionMode.Editable);

        // add the component when both mesh filter and mesh renderer exist
        foreach (Transform transform in transforms)
        {
            transform.gameObject.AddComponent<GridCreator>();
        }
    }

    void OnEnable()
    {   
        // call settings variables
        var bgDefaultData = EditorPrefs.GetString("SpeedGridCreator", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(bgDefaultData, this);     
    }

    void OnDisable()
    {
        // call settings variables
        var bgDefaultData = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("SpeedGridCreator", bgDefaultData);
    }

    void OnGUI()
    {
        // start scrollable
        scrollPositionTop = EditorGUILayout.BeginScrollView(scrollPositionTop,
            GUILayout.Width(Screen.width * 0.8f), GUILayout.Height(Screen.height * 0.95f));

        // background color
        originalColor = GUI.backgroundColor;

        if(defaultColor)
        {
            bgR = 1; bgG = 1; bgB = 1; bgA = 1;
        }

        GUI.backgroundColor = new Color(bgR, bgG, bgB, bgA);

        // control object list size
        objectList = EditorGUILayout.IntField("Object List", objectList);


        // set variable list
        if (selectedObject.Count <= 0)
        {
            objectList = 1;
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

        // show object field
        if(objectList > 0)
        {
            for (int i = 0; i < selectedObject.Count; i++)
            {
                selectedObject[i] = EditorGUILayout.ObjectField("Selected Object " + i,
                selectedObject[i], typeof(GameObject), true) as GameObject;
            }
        }          

        // show object number slider when object list more than 1
        if (objectList > 1)
        {
            objectNum = EditorGUILayout.IntSlider("Object Number",objectNum, 0, selectedObject.Count - 1);
        }
        else
        {
            objectNum = 0;
        }

        // setting button
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(Screen.width * 0.2f), GUILayout.ExpandHeight(true));
        if (selectedObject.Count > 0 )
        {
            if (selectedObject[objectNum] != null && objectList > 0)
            {
                if (GUILayout.Button("Object Settings"))
                {
                    objectSetting = true;
                    gridSetting = false;
                    windowSetting = false;
                }
                if (objectList > 0)
                {
                    if (GUILayout.Button("Grid Settings"))
                    {
                        objectSetting = false;
                        gridSetting = true;
                        windowSetting = false;
                    }
                }
            }
            else if (objectList > 0)
            {
                if (GUILayout.Button("Grid Settings"))
                {
                    objectSetting = false;
                    gridSetting = true;
                    windowSetting = false;
                }
            }
            else
            {
                objectSetting = false;
                gridSetting = false;
            }
        }
        
        if (GUILayout.Button("Window Settings"))
        {
            objectSetting = false;
            gridSetting = false;
            windowSetting = true;
        }

        ResetButton();

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        // start scrollable for setting panel
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
            GUILayout.Width(Screen.width * 0.8f), GUILayout.Height(Screen.height * 0.9f));

        // show setting when selected
        if (objectSetting && selectedObject[objectNum] != null)
        {
            if (selectedObject[objectNum].GetComponent<Renderer>() != null)
                if (selectedObject[objectNum].GetComponent<Renderer>().sharedMaterial != null)
                    ColorSetting();

            SizeSetting();
        }

        if (gridSetting)
        {
            objectTransform = EditorGUILayout.Vector3Field("Transform", objectTransform);

            GridSetting();
        }

        if (windowSetting)
        {
            WindowSetting();
        }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        // update list when list change
        if (objectList > 0 && selectedObject.Count != objectList)
        {
            // object
            if (selectedObject.Count < objectList)
            {
                while (selectedObject.Count != objectList)
                {
                    selectedObject.Add(null);
                }
            }
            else
            {
                while (selectedObject.Count != objectList)
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

        // check if gui change
        if (GUI.changed)
        {
            GridUpdate();
        }   
    }


    void ColorSetting()
    {
        // color
        GUILayout.Label("Color:");
        GUILayout.Label("Red");
        r[objectNum] = EditorGUILayout.Slider(r[objectNum], 0f, 1f);
        GUILayout.Label("Green");
        g[objectNum] = EditorGUILayout.Slider(g[objectNum], 0f, 1f);
        GUILayout.Label("Blue");
        b[objectNum] = EditorGUILayout.Slider(b[objectNum], 0f, 1f);

        Color guiColor = GUI.color;

        GUI.color = new Color(r[objectNum], g[objectNum], b[objectNum]);

        GUILayout.Label("Preview", GUILayout.Width(Screen.width * 0.7f));
        GUI.DrawTexture(new Rect(0, 160, Screen.width * 0.7f, 8), EditorGUIUtility.whiteTexture);

        GUI.color = guiColor;

        GUILayout.Label(" ");
        if (GUILayout.Button("Randomize"))
        {
            r[objectNum] = Random.Range(0f, 1f);
            g[objectNum] = Random.Range(0f, 1f);
            b[objectNum] = Random.Range(0f, 1f);
        }
        //Reset Color
        if (GUILayout.Button("Reset Color"))
        {
            r[objectNum] = 0; g[objectNum] = 0; b[objectNum] = 0;
        }
    }

    void SizeSetting()
    {
        // size
        GUILayout.Label("Size:");
        GUILayout.Label("Base Size");
        baseSize[objectNum] = EditorGUILayout.FloatField(baseSize[objectNum]);
        GUILayout.Label("SizeX");
        sizeX[objectNum] = EditorGUILayout.Slider(sizeX[objectNum], 0f, 10f);
        GUILayout.Label("SizeY");
        sizeY[objectNum] = EditorGUILayout.Slider(sizeY[objectNum], 0f, 10f);
        GUILayout.Label("SizeZ");
        sizeZ[objectNum] = EditorGUILayout.Slider(sizeZ[objectNum], 0f, 10f);
        GUILayout.Label(" ");
        if (GUILayout.Button("Randomize"))
        {
            sizeX[objectNum] = Random.Range(0f, 10f);
            sizeY[objectNum] = Random.Range(0f, 10f);
            sizeZ[objectNum] = Random.Range(0f, 10f);
        }
        //Reset Size
        if (GUILayout.Button("Reset Size"))
        {
            baseSize[objectNum] = 1; sizeX[objectNum] = 1; sizeY[objectNum] = 1; sizeZ[objectNum] = 1;
        }
    }

    void GridSetting()
    {
        // grid
        GUILayout.Label("Grid:");
        GUILayout.Label("Row");
        row = EditorGUILayout.IntField(row);
        GUILayout.Label("Column");
        column = EditorGUILayout.IntField(column);
        GUILayout.Label("Layer");
        layer = EditorGUILayout.IntField(layer);
        GUILayout.Label(" ");

        // reset bg color for toggle
        GUI.backgroundColor = originalColor;

        // mirror
        GUILayout.Label("Mirror");
        GUILayout.BeginHorizontal();
        GUILayout.Label("axis X");
        mirrorAxisX = EditorGUILayout.Toggle(mirrorAxisX);
        GUILayout.Label("axis Y");
        mirrorAxisY = EditorGUILayout.Toggle(mirrorAxisY);
        GUILayout.Label("axis Z");
        mirrorAxisZ = EditorGUILayout.Toggle(mirrorAxisZ);
        GUILayout.EndHorizontal();
        GUILayout.Label(" ");

        GUI.backgroundColor = new Color(bgR, bgG, bgB, bgA);

        //Reset Grid
        if (GUILayout.Button("Reset Grid"))
        {
            mirrorAxisX = false; mirrorAxisY = false; mirrorAxisZ = false;
            row = 1; column = 1; layer = 1;
        }

        // Space
        GUILayout.Label("Space:");
        GUILayout.Label("Base Space");
        baseSpace = EditorGUILayout.FloatField(baseSpace);
        GUILayout.Label("SpaceX");
        spaceX = EditorGUILayout.Slider(spaceX, 0f, 1f);
        GUILayout.Label("SpaceY");
        spaceY = EditorGUILayout.Slider(spaceY, 0f, 1f);
        GUILayout.Label("SpaceZ");
        spaceZ = EditorGUILayout.Slider(spaceZ, 0f, 1f);
        GUILayout.Label(" ");

        if (GUILayout.Button("Randomize"))
        {
            spaceX = Random.Range(0f, 1f);
            spaceY = Random.Range(0f, 1f);
            spaceZ = Random.Range(0f, 1f);
        }
        //Reset Space
        if (GUILayout.Button("Reset Space"))
        {
            baseSpace = 3; spaceX = 1; spaceY = 1; spaceZ = 1;
        }

        GUILayout.Label("Base Space Change");
        baseSpaceChange = EditorGUILayout.FloatField(baseSpaceChange);
        GUILayout.Label("spaceX Change");
        spaceXChange = EditorGUILayout.Slider(spaceXChange, -1f, 1f);
        GUILayout.Label("spaceZ Change");
        spaceZChange = EditorGUILayout.Slider(spaceZChange, -1f, 1f);
        GUILayout.Label("spaceY Change X");
        spaceYChangeX = EditorGUILayout.Slider(spaceYChangeX, -1f, 1f);
        GUILayout.Label("spaceY Change Z");
        spaceYChangeZ = EditorGUILayout.Slider(spaceYChangeZ, -1f, 1f);
        GUILayout.Label(" ");

        if (GUILayout.Button("Randomize"))
        {
            spaceXChange = Random.Range(-1f, 1f);
            spaceZChange = Random.Range(-1f, 1f);
            spaceYChangeX = Random.Range(-1f, 1f);
            spaceYChangeZ = Random.Range(-1f, 1f);
        }
        //Reset Space Change
        if (GUILayout.Button("Reset Space Change"))
        {
            baseSpaceChange = 0; spaceXChange = 0; spaceZChange = 0; spaceYChangeX = 0; spaceYChangeZ = 0;
        }

        // rotation
        GUILayout.Label("Rotation:");

        // reset bg color for toggle
        GUI.backgroundColor = originalColor;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotate X Identical");
        rotateXIdentical = EditorGUILayout.Toggle(rotateXIdentical);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(bgR, bgG, bgB, bgA);

        GUILayout.Label("Rotate X");
        rotateX = EditorGUILayout.Slider(rotateX, 0f, 360f);

        GUI.backgroundColor = originalColor;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotate Y Identical");
        rotateYIdentical = EditorGUILayout.Toggle(rotateYIdentical);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(bgR, bgG, bgB, bgA);

        GUILayout.Label("Rotate Y");
        rotateY = EditorGUILayout.Slider(rotateY, 0f, 360f);

        GUI.backgroundColor = originalColor;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotate Z Identical");
        rotateZIdentical = EditorGUILayout.Toggle(rotateZIdentical);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(bgR, bgG, bgB, bgA);

        GUILayout.Label("Rotate Z");
        rotateZ = EditorGUILayout.Slider(rotateZ, 0f
            , 360f);
        GUILayout.Label(" ");

        if (GUILayout.Button("Randomize"))
        {
            rotateX = Random.Range(0f, 360f);
            rotateY = Random.Range(0f, 360f);
            rotateZ = Random.Range(0f, 360f);
        }
        //Reset Rotation
        if (GUILayout.Button("Reset Rotote"))
        {
            rotateXIdentical = false;
            rotateYIdentical = false;
            rotateZIdentical = false;
            rotateX = 0; rotateY = 0; rotateZ = 0;
        }
        GUILayout.Label(" ");

        // Queue
        GUILayout.Label("Queue");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Queue Axis X");
        queueAxisX = EditorGUILayout.Toggle(queueAxisX);
        GUILayout.Label("Queue Axis Y");
        queueAxisY = EditorGUILayout.Toggle(queueAxisY);
        GUILayout.Label("Queue Axis Z");
        queueAxisZ = EditorGUILayout.Toggle(queueAxisZ);
        GUILayout.EndHorizontal();
        GUILayout.Label(" ");
        
    }

    void ResetButton()
    {
        GUILayout.Label(" ");
        if (GUILayout.Button("Reset All"))
        {
            r[objectNum] = 0; g[objectNum] = 0; b[objectNum] = 0;

            baseSize[objectNum] = 1; sizeX[objectNum] = 1; sizeY[objectNum] = 1; sizeZ[objectNum] = 1;

            mirrorAxisX = false; mirrorAxisY = false; mirrorAxisZ = false;

            row = 1; column = 1; layer = 1;

            baseSpace = 3; spaceX = 1; spaceY = 1; spaceZ = 1;

            baseSpaceChange = 0; spaceXChange = 0; spaceZChange = 0; spaceYChangeX = 0; spaceYChangeZ = 0;

            rotateXIdentical = false; rotateYIdentical = false; rotateZIdentical = false;
            rotateX = 0; rotateY = 0; rotateZ = 0;

            queueAxisX = false; queueAxisY = false; queueAxisZ = false;
        }
    }

    void WindowSetting()
    {
        // color
        if (!defaultColor)
        {
            GUILayout.Label("Color");
            GUILayout.Label("Red");
            bgR = EditorGUILayout.Slider(bgR, 0f, 1f);
            GUILayout.Label("Green");
            bgG = EditorGUILayout.Slider(bgG, 0f, 1f);
            GUILayout.Label("Blue");
            bgB = EditorGUILayout.Slider(bgB, 0f, 1f);
            GUILayout.Label("Opacity");
            bgA = EditorGUILayout.Slider(bgA, 0f, 1f);
            GUILayout.Label("");
        }       
        GUILayout.Label("Default Color");
        defaultColor = EditorGUILayout.Toggle(defaultColor);
    }

    void GridUpdate()
    {
        // check if object list is emepty
        bool checkEmpty = true;

        for(int i = 0;i < selectedObject.Count; i++)
        {
            if(selectedObject[i] != null)
            {
                checkEmpty = false;
                break;
            }
        }

        if (row < 1 || column < 1 || layer < 1)
        {
            Debug.LogWarning("Grid value cannot changed to below 1.");
        }
        else if (checkEmpty == true)
        {
            Debug.LogError ("The game object have not been selected.");
        }
        else
        {
            // group up the grid
            for (int i = 0; i < selectedObject.Count; i++)
            {
                if (selectedObject[i] != null)
                {
                    while (GameObject.Find(selectedObject[i].name + " Grid") != null)
                        DestroyImmediate(GameObject.Find(selectedObject[i].name + " Grid"));

                    if (GameObject.Find("Grid Group") != null)
                        DestroyImmediate(GameObject.Find("Grid Group"));
                    DestroyImmediate(GameObject.Find(selectedObject[i].name + " Grid"));
                }
            }
            GameObject objectGrid = new GameObject(" Grid");

            if (selectedObject[objectNum] != null)
            {
                objectGrid.name = selectedObject[objectNum].name + " Grid";
            }

            // set up object number
            int objectNumber = objectNum;

            // create grid
            for (int gridLayer = 1; gridLayer <= layer; gridLayer++)
            {              
                for (int gridColumn = 1; gridColumn <= column; gridColumn++)
                {
                    for (int gridRow = 1; gridRow <= row; gridRow++)
                    {
                        // check if selected object exist
                        if (selectedObject[objectNumber] != null)
                        {
                            // set material
                            if (selectedObject[objectNumber].GetComponent<Renderer>() != null)
                                if (selectedObject[objectNumber].GetComponent<Renderer>().sharedMaterial != null)
                                    selectedObject[objectNumber].GetComponent<Renderer>().sharedMaterial.color =
                                        new Color(r[objectNumber], g[objectNumber], b[objectNumber]);

                            // set size
                            selectedObject[objectNumber].transform.localScale =
                                new Vector3(sizeX[objectNumber], sizeY[objectNumber], 
                                sizeZ[objectNumber]) * baseSize[objectNumber];

                            // set rotation
                            var rotationX = rotateX; var rotationY = rotateY; var rotationZ = rotateZ;

                            // create object
                            var selectedPrefab = Instantiate(selectedObject[objectNumber], 
                                objectTransform + new Vector3((gridRow - 1) * spaceX, (gridLayer - 1) * spaceY,
                               (gridColumn - 1) * spaceZ) * baseSpace, Quaternion.identity);

                            // add more space change
                            selectedPrefab.transform.position += new Vector3(
                                (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                (gridLayer - 1) * spaceZChange * baseSpaceChange);

                            // check if rotation is identical
                            if (!rotateXIdentical)
                            {
                                rotationX = (gridRow - 1) * rotateX;
                            }
                            if (!rotateYIdentical)
                            {
                                rotationY = (gridLayer - 1) * rotateY;
                            }
                            if (!rotateZIdentical)
                            {
                                rotationZ = (gridColumn - 1) * rotateZ;
                            }

                            // set rotation
                            selectedPrefab.transform.Rotate(new Vector3(rotationX, rotationY, rotationZ));

                            // group up object
                            selectedPrefab.transform.parent = objectGrid.transform;

                            // mirror
                            if (mirrorAxisX)
                            {
                                var mirrorX = Instantiate(selectedObject[objectNumber], objectTransform +
                                new Vector3((gridRow) * -spaceX, (gridLayer - 1) * spaceY,
                                (gridColumn - 1) * spaceZ) * baseSpace, Quaternion.identity);

                                // add more space change
                                mirrorX.transform.position += new Vector3(
                                    (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }

                                // set rotation
                                mirrorX.transform.Rotate(new Vector3(rotationX, -rotationY, -rotationZ));

                                // group up object
                                mirrorX.transform.parent = objectGrid.transform;
                            }

                            // mirror object
                            if (mirrorAxisY)
                            {
                                var mirrorY = Instantiate(selectedObject[objectNumber], objectTransform +
                               new Vector3((gridRow - 1) * spaceX, (gridLayer) * -spaceY,
                               (gridColumn - 1) * spaceZ) * baseSpace, Quaternion.identity);

                                // add more space change
                                mirrorY.transform.position += new Vector3(
                                    (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }


                                mirrorY.transform.Rotate(new Vector3(-rotationX, rotationY, -rotationZ));

                                mirrorY.transform.parent = objectGrid.transform;

                                // match mirror
                                if (mirrorAxisX)
                                {
                                    var mirrorX = Instantiate(selectedObject[objectNumber], objectTransform +
                                    new Vector3((gridRow) * -spaceX, (gridLayer) * -spaceY,
                                    (gridColumn - 1) * spaceZ) * baseSpace, Quaternion.identity);

                                    // add more space change
                                    mirrorX.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorX.transform.Rotate(new Vector3(-rotationX, -rotationY, rotationZ));

                                    mirrorX.transform.parent = objectGrid.transform;
                                }
                            }

                            if (mirrorAxisZ)
                            {
                                var mirrorZ = Instantiate(selectedObject[objectNumber], objectTransform +
                               new Vector3((gridRow - 1) * spaceX, (gridLayer - 1) * spaceY,
                               (gridColumn) * -spaceZ) * baseSpace, Quaternion.identity);

                                // add more space change
                                mirrorZ.transform.position += new Vector3(
                                    (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                    ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                    ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                    (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                // check if rotation is identical
                                if (!rotateXIdentical)
                                {
                                    rotationX = (gridRow - 1) * rotateX;
                                }
                                if (!rotateYIdentical)
                                {
                                    rotationY = (gridLayer - 1) * rotateY;
                                }
                                if (!rotateZIdentical)
                                {
                                    rotationZ = (gridColumn - 1) * rotateZ;
                                }


                                mirrorZ.transform.Rotate(new Vector3(-rotationX, -rotationY, rotationZ));

                                mirrorZ.transform.parent = objectGrid.transform;

                                // match mirror
                                if (mirrorAxisX)
                                {
                                    var mirrorX = Instantiate(selectedObject[objectNumber], objectTransform +
                                    new Vector3((gridRow) * -spaceX, (gridLayer - 1) * spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, Quaternion.identity);

                                    // add more space change
                                    mirrorX.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorX.transform.Rotate(new Vector3(-rotationX, rotationY, -rotationZ));

                                    mirrorX.transform.parent = objectGrid.transform;
                                }

                                if (mirrorAxisY)
                                {
                                    var mirrorY = Instantiate(selectedObject[objectNumber], objectTransform +
                                    new Vector3((gridRow - 1) * spaceX, (gridLayer) * -spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, Quaternion.identity);

                                    // add more space change
                                    mirrorY.transform.position += new Vector3(
                                        (gridLayer - 1) * spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorY.transform.Rotate(new Vector3(rotationX, -rotationY, -rotationZ));

                                    mirrorY.transform.parent = objectGrid.transform;
                                }

                                if (mirrorAxisX && mirrorAxisY)
                                {
                                    var mirrorY = Instantiate(selectedObject[objectNumber], objectTransform +
                                    new Vector3((gridRow) * -spaceX, (gridLayer) * -spaceY,
                                    (gridColumn) * -spaceZ) * baseSpace, Quaternion.identity);

                                    // add more space change
                                    mirrorY.transform.position += new Vector3(
                                        (gridLayer - 1) * -spaceXChange * baseSpaceChange,
                                        ((gridRow - 1) * -spaceYChangeX * baseSpaceChange) +
                                        ((gridColumn - 1) * -spaceYChangeZ * baseSpaceChange),
                                        (gridLayer - 1) * -spaceZChange * baseSpaceChange);

                                    // check if rotation is identical
                                    if (!rotateXIdentical)
                                    {
                                        rotationX = (gridRow - 1) * rotateX;
                                    }
                                    if (!rotateYIdentical)
                                    {
                                        rotationY = (gridLayer - 1) * rotateY;
                                    }
                                    if (!rotateZIdentical)
                                    {
                                        rotationZ = (gridColumn - 1) * rotateZ;
                                    }

                                    mirrorY.transform.Rotate(new Vector3(rotationX, rotationY, rotationZ));

                                    mirrorY.transform.parent = objectGrid.transform;
                                }
                            }                            
                        }
                        // queue X
                        if (queueAxisX)
                        {
                            if (objectNumber < selectedObject.Count - 1)
                                objectNumber++;
                            else
                            {
                                objectNumber = 0;
                            }
                        }
                    }                      
                    // queue Z
                    if (queueAxisZ)
                    {
                        if (objectNumber < selectedObject.Count - 1)
                            objectNumber++;
                        else
                        {
                            objectNumber = 0;
                        }
                    }
                }
                // queue Y
                if (queueAxisY)
                {
                    if(objectNumber < selectedObject.Count - 1)
                        objectNumber++;
                    else
                    {
                        objectNumber = 0;
                    }
                }
            }
            // grid group
            if (selectedObject.Count > 1)
            {
                GameObject gridGroup = new GameObject("Grid Group");
                objectGrid.transform.parent = gridGroup.transform;
            }
        }
    }
}