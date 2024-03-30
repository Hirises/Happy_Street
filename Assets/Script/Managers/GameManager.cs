using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum SceneState
    {
        Play,
        Shop,
        Blueprinting
    }

    private static GameManager instance;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject buildingPostion;
    [SerializeField]
    private GameObject spaceMarkerPrefab;
    [SerializeField]
    private Material blueprintOccupied;
    [SerializeField]
    private Material blueprintPlaceable;

    [SerializeField]
    private float buildingWidthUnit;
    [SerializeField]
    private int initalStreetLength;

    [SerializeField]
    private GameObject topMenuUI;
    [SerializeField]
    private GameObject shopUI;

    private int rightStreetLenth;
    private int leftStreetLenth;
    private List<Space> spaces;
    private SceneState sceneState;
    private StructureData blueprintStructure;
    private int blueprintStartPosX;
    private int blueprintPreviousEndPosX;
    private bool startBlueprinting;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        sceneState = SceneState.Play;
        rightStreetLenth = leftStreetLenth = initalStreetLength;
        spaces = new List<Space>(initalStreetLength * 2 + 1);

        loadStreet();
    }

    private void loadStreet()
    {
        for(int index = 0; index < GetStreetLength(); index++)
        {
            Position buildingPos = Position.FromIndexPosX(index);
            Vector3 pos = new Vector3(buildingPos.ToWorldPosX(), buildingPostion.transform.position.y, buildingPostion.transform.position.z);
            GameObject obj = Instantiate(spaceMarkerPrefab, pos, Quaternion.identity, buildingPostion.transform);
            Space building = obj.GetComponent<Space>();
            building.SetPosition(buildingPos);
            spaces.Add(building);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (GetSceneState())
        {
            case SceneState.Blueprinting:
                CheckPrebuilding();
                break;
        }
    }

    private void CheckPrebuilding()
    {
        if (Input.GetMouseButtonUp(1))
        {
            //설치 취소
            QuitBlueprintMode();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //설치 시작
            startBlueprinting = true;
            blueprintStartPosX = blueprintPreviousEndPosX = SnapToBuildingPosX(mainCamera.ScreenToWorldPoint(Input.mousePosition).x);
            DisplayBulePrint(blueprintStartPosX);
        }
        else if (startBlueprinting && Input.GetMouseButtonUp(0))
        {
            //설치 끝
            ApplyBlueprint();
            blueprintStartPosX = blueprintPreviousEndPosX = SnapToBuildingPosX(mainCamera.ScreenToWorldPoint(Input.mousePosition).x);
            startBlueprinting = false;
        }
        else if (Input.GetMouseButton(0))
        {
            //설치 중
            int curPosX = SnapToBuildingPosX(mainCamera.ScreenToWorldPoint(Input.mousePosition).x);
            DisplayBulePrint(curPosX);
        }
    }

    public static IEnumerable<int> GetEnumerator(int first, int second)
    {
        int min = first;
        int max = second;
        if(first > second)
        {
            min = second;
            max = first;
        }
        for(int num = min; num <= max; num++)
        {
            yield return num;
        }
    }

    private void DisplayBulePrint(int curPosX)
    {

        //표시할 범위
        int minBoundary = blueprintStartPosX;
        int maxBoundary = curPosX;
        if (curPosX < blueprintStartPosX)
        {
            minBoundary = curPosX;
            maxBoundary = blueprintStartPosX;
        }

        //검사
        foreach (int index in GetEnumerator(blueprintStartPosX, blueprintPreviousEndPosX))
        {
            if((index - minBoundary) % blueprintStructure.GetLength() != 0)
            {
                continue;
            }

            Position pos = Position.FromSignedPosX(index);
            if (!pos.IsInStreetBoundary())
            {
                continue;
            }

            if(minBoundary <= index && index <= maxBoundary)
            {
                GetSpace(pos).SetStructureBluePrint(blueprintStructure);
            }
            else
            {
                GetSpace(pos).RemoveStructureBluePrint();
            }
        }

        blueprintPreviousEndPosX = curPosX;
    }

    //월드 X 좌표를 건물 인덱스로 Snap
    public static int SnapToBuildingPosX(float x)
    {
        return (int) Mathf.Floor((x / GetInstance().buildingWidthUnit) + 0.5f);
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public void ChangeSceneState(SceneState state)
    {
        sceneState = state;
    }

    public SceneState GetSceneState()
    {
        return sceneState;
    }

    public int GetStreetLength()
    {
        return leftStreetLenth + 1 + rightStreetLenth;
    }

    public int GetRightStreetLength()
    {
        return rightStreetLenth;
    }

    public int GetLeftStreetLength()
    {
        return leftStreetLenth;
    }

    public float GetBuildingWidthUnit()
    {
        return buildingWidthUnit;
    }

    public Space GetSpace(Position pos)
    {
        if (pos.IsInStreetBoundary())
        {
            int index = pos.ToIndexPosX();
            if (index >= 0 && index < spaces.Capacity)
            {
                return spaces[index];
            }
        }
        return null;
    }

    public void openShop()
    {
        if (GetSceneState() != SceneState.Shop)
        {
            shopUI.SetActive(true);
            topMenuUI.SetActive(false);
            ChangeSceneState(SceneState.Shop);

        }
    }

    public void closeShop()
    {
        if (GetSceneState() == SceneState.Shop)  
        {
            shopUI.SetActive(false);
            topMenuUI.SetActive(true);
            ChangeSceneState(SceneState.Play);
        }
    }

    public void enterBlueprintMode(StructureData building)
    {
        if(GetSceneState() != SceneState.Blueprinting)
        {
            blueprintStartPosX = 0;
            blueprintPreviousEndPosX = 0;
            blueprintStructure = building;
            shopUI.SetActive(false);
            startBlueprinting = false;
            ChangeSceneState(SceneState.Blueprinting);
        }
    }

    public void QuitBlueprintMode()
    {
        if (GetSceneState() == SceneState.Blueprinting)
        {
            foreach (int index in GetEnumerator(blueprintStartPosX, blueprintPreviousEndPosX))
            {
                Position pos = Position.FromSignedPosX(index);
                if (!pos.IsInStreetBoundary())
                {
                    continue;
                }

                GetSpace(pos).RemoveStructureBluePrint();
            }
            topMenuUI.SetActive(true);
            ChangeSceneState(SceneState.Play);
            blueprintStartPosX = 0;
            blueprintPreviousEndPosX = 0;
            blueprintStructure = null;
            startBlueprinting = false;
        }
    }

    public void ApplyBlueprint()
    {
        if (GetSceneState() == SceneState.Blueprinting)
        {
            foreach (int index in GetEnumerator(blueprintStartPosX, blueprintPreviousEndPosX))
            {
                Position pos = Position.FromSignedPosX(index);
                if (!pos.IsInStreetBoundary())
                {
                    continue;
                }

                GetSpace(pos).applyStructureBluePrint();
            }
        }
    }

    public Material GetBlueprintMaterial(bool occupied)
    {
        if (occupied)
        {
            return blueprintOccupied;
        }
        else
        {
            return blueprintPlaceable;
        }
    }
}
