using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public interface IHasActive
{
    public bool isActive { get; set; }
}


public class ReusableObjectManager : MonoBehaviour
{
    // InstantiateするPrefabのリスト
    [SerializeField] GameObject[] toInstantiatePrefabs;

    // 各オブジェクトの管理リストへの参照をもつリスト
    private List<System.Object> listRefs = new List<System.Object>();

    // 管理リストが参照リストの何番目にあるかを持つ辞書
    private Dictionary<Type, int> listRefIds_Dict = new Dictionary<Type, int>();




    public T MainAction<T>() where T : IHasActive
    {
        // リストがあるか確認
        if (CheckExistList<T>(out List<T> targetList) == false)
        {
            // なければリストを新規作成
            targetList = CreateNewList<T>();
        }

        // リストにアクティブではないオブジェクトがあればそれを再利用
        if (CheckExistInActiveObject<T>(targetList, out T inActiveObject) == false)
        {
            // Prefabリストから指定のコンポーネントを持つPrefabを検索
            if (TryGetHasComponentPrefab<T>(out GameObject targetPrefab) == false)
                return default(T);

            // なければオブジェクトを作成、リストに格納
            inActiveObject = CreateObject<T>(targetPrefab);
            targetList.Add(inActiveObject);
        }

        // 作ったオブジェクトを返す
        return inActiveObject;
    }


    private bool CheckExistList<T>(out List<T> outList)
    {
        // Tの管理リストが存在するかチェック
        if (listRefIds_Dict.TryGetValue(typeof(T), out int index))
        {
            outList = listRefs[index] as List<T>;
            return true;
        }

        outList = null;
        return false;
    }


    private List<T> CreateNewList<T>()
    {
        // T型の管理リストを新しく作る。
        var createdList = new List<T>();
        var createdStackList = new Stack<T>();

        // T型の管理リストのインデックスを格納する。
        listRefIds_Dict.Add(typeof(T), listRefs.Count);

        // リストの参照リストに追加する。
        listRefs.Add(createdList);
        listRefs.Add(createdStackList);

        return createdList;
    }


    private bool CheckExistInActiveObject<T>(List<T> targetList, out T outObject) where T : IHasActive
    {
        foreach (T type in targetList)
        {
            if (type.isActive == false)
            {
                outObject = type;
                return true;
            }
        }

        outObject = default(T);
        return false;
    }



    private T CreateObject<T>(GameObject toInstancePrefab) where T : IHasActive
    {
        // 新規オブジェクトを作成する。
        T newObject = Instantiate(toInstancePrefab).GetComponent<T>();

        // newObject.ObserveEveryValueChanged(newObject => newObject.isActive).Subscribe()



        return newObject;
    }


    private bool TryGetHasComponentPrefab<T>(out GameObject outObject)
    {
        outObject = null;

        foreach (GameObject obj in toInstantiatePrefabs)
        {
            if (obj.TryGetComponent<T>(out T nouse))
            {
                outObject = obj;
                return true;
            }
        }

        Debug.LogError($"Object that has '{typeof(T).FullName}' is not set in Inspector. at '{this.GetType().FullName}'");
        return false;
    }
}
