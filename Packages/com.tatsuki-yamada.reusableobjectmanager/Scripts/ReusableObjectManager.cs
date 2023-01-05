using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ReusableObjectManagement
{
    /// <summary>
    /// ReusableObjectManagerで管理するオブジェクトに必要なインターフェース
    /// </summary>
    public interface IHasActive
    {
        /// <summary>
        /// オブジェクトが生きているか示すフラグ。falseのとき、再利用するスタックに投入される。
        /// </summary>
        /// <value></value>
        public BoolReactiveProperty isActive { get; set; }

        /// <summary>
        /// isActiveへの参照を持つObserver
        /// </summary>
        public IObservable<bool> isActiveObserver => isActive;

        /// <summary>
        /// 生成・再利用後、isActive=trueを呼ぶ前に再利用スタックに投入されるのを防ぐため、再利用して良いかを別に示したフラグ
        /// </summary>
        /// <value></value>
        public bool canReuse { get; }
    }


    /// <summary>
    /// IHasObjectを継承したクラスを生成・管理・再利用できるマネージャークラス
    /// </summary>
    public class ReusableObjectManager : MonoBehaviour
    {
        // InstantiateするPrefabのリスト
        [SerializeField, Header("生成・再利用するPrefabを以下に登録する。")]
        GameObject[] toInstantiatePrefabs;

        // 各オブジェクトの管理リストと再利用スタックへの参照を持つリスト
        private List<System.Object> objectsListRefs = new List<System.Object>();

        // 管理リストが参照リストの何番目にあるかを持つ辞書
        private Dictionary<Type, int> objectListIndexDict = new Dictionary<Type, int>();


        /// <summary>
        /// 指定したクラスを生成・再利用する。
        /// </summary>
        /// <typeparam name="T"> 生成・再利用するクラス </typeparam>
        /// <returns> 生成・再利用したTクラス </returns>
        public T CreateOrReuse<T>() where T : IHasActive
        {
            // T型の管理リストがあるか確認する。
            if (CheckExistManagementList<T>(out List<T> targetManageList) == false)
            {
                // なければリストを新規作成する。
                targetManageList = CreateNewList<T>();
            }

            // 再利用スタックにオブジェクトがあれば、それをreturnにする。
            if (CheckExistReuseObject<T>(out T returnObject) == false)
            {
                // PrefabリストからTコンポーネントを持つPrefabを検索する。
                if (TryGetHasComponentPrefab<T>(out GameObject targetPrefab) == false)
                    return default(T);

                // 再利用スタックにオブジェクトがなければ新規作成、管理リストに格納する。
                returnObject = CreateNewObject<T>(targetPrefab);
                targetManageList.Add(returnObject);
            }

            // 作ったオブジェクトを返す。
            return returnObject;
        }


        /// <summary>
        /// T型の管理リストを取得する。
        /// </summary>
        /// <param name="outList"> 取得した管理リスト </param>
        /// <typeparam name="T"> このクラスの管理リストを取得する。 </typeparam>
        /// <returns>  </returns>
        public bool TryGetManagementList<T>(out List<T> outList)
        {
            // Tクラスの管理リストが存在するかチェックし、あればそれを返す。
            if (CheckExistManagementList<T>(out outList))
                return true;

            // なければエラーを出す。
            Debug.LogError($"Management list that contains '{typeof(T).FullName}' is not found. at '{this.GetType().FullName}'");
            outList = null;
            return false;

        }


        /// <summary>
        /// T型の管理リストが存在するかチェックし、あればそれを返す処理を行う。
        /// </summary>
        /// <param name="outList"> 見つかった管理リストが入る参照 </param>
        /// <typeparam name="T"> 管理リストの型 </typeparam>
        /// <returns> T型の管理リストが存在したか </returns>
        private bool CheckExistManagementList<T>(out List<T> outList)
        {
            // Tの管理リストが存在するかチェック
            if (objectListIndexDict.TryGetValue(typeof(T), out int index))
            {
                outList = objectsListRefs[index] as List<T>;
                return true;
            }

            outList = null;
            return false;
        }


        /// <summary>
        /// T型の管理リストと、再利用オブジェクトが入るスタックを新しく作成する。
        /// </summary>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        /// <returns> 作成した管理リスト </returns>
        private List<T> CreateNewList<T>()
        {
            // T型の管理リストと、再利用スタックを新しく作る。
            var createdManageList = new List<T>();
            var createdStackList = new Stack<T>();

            // T型の管理リストのインデックスを格納する。スタックは管理リスト+1のインデックスに入るため、登録する必要がない。
            objectListIndexDict.Add(typeof(T), objectsListRefs.Count);

            // 参照リストに2つを追加する。
            objectsListRefs.Add(createdManageList);
            objectsListRefs.Add(createdStackList);

            return createdManageList;
        }


        /// <summary>
        /// 再利用スタックから、オブジェクトを一つ取り出す。
        /// </summary>
        /// <param name="outObject"> 再利用スタックから取り出したオブジェクト </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        /// <returns> 再利用スタックにオブジェクトが入っているか </returns>
        private bool CheckExistReuseObject<T>(out T outObject) where T : IHasActive
        {
            // 管理リストのインデックス番号をもらう。
            objectListIndexDict.TryGetValue(typeof(T), out int index);

            // インデックス番号に+1したところにスタックリストがある。
            var stack = objectsListRefs[index + 1] as Stack<T>;

            // スタックリストが空でなければ先頭のオブジェクトを出す。
            if (stack.Count != 0)
            {
                outObject = stack.Pop();
                return true;
            }

            outObject = default(T);
            return false;
        }


        /// <summary>
        /// 新規オブジェクトを作成する。
        /// </summary>
        /// <param name="toInstancePrefab"> 生成するオブジェクトのPrefab </param>
        /// <typeparam name="T"> 生成するオブジェクトの型 </typeparam>
        /// <returns> 生成したオブジェクトのTクラス </returns>
        private T CreateNewObject<T>(GameObject toInstancePrefab) where T : IHasActive
        {
            // 新規オブジェクトを作成する。
            T newObject = Instantiate(toInstancePrefab).GetComponent<T>();

            // isActiveがfalseになったとき、再利用スタックに格納する処理を登録する。
            newObject.isActiveObserver
                .Where(flag => flag == false)
                .Subscribe(nouse =>
                {
                    // 生成したオブジェクトのisActive = trueよりも検知が早いと再利用されてしまうため、フラグ管理を1枚噛ませている。
                    if (newObject.canReuse)
                        AddStack(newObject);
                })
                .AddTo(this);

            return newObject;
        }


        /// <summary>
        /// 再利用スタックに投入する処理
        /// </summary>
        /// <param name="toPush"> 投入するオブジェクト </param>
        /// <typeparam name="T"> オブジェクトの型 </typeparam>
        private void AddStack<T>(T toPush) where T : IHasActive
        {
            // 管理リストのインデックス番号をもらう。
            objectListIndexDict.TryGetValue(typeof(T), out int index);

            // インデックス番号に+1したところにスタックリストがある。
            var stack = objectsListRefs[index + 1] as Stack<T>;

            stack.Push(toPush);
        }


        /// <summary>
        /// 指定したクラスを持つPrefabを、登録したPrefabリストから探す。
        /// </summary>
        /// <param name="foundPrefab"> 見つかったPrefab </param>
        /// <typeparam name="T"> 検索対象のクラス </typeparam>
        /// <returns> Prefabが見つかったか </returns>
        private bool TryGetHasComponentPrefab<T>(out GameObject foundPrefab)
        {
            foundPrefab = null;

            foreach (GameObject obj in toInstantiatePrefabs)
            {
                if (obj.TryGetComponent<T>(out T nouse))
                {
                    foundPrefab = obj;
                    return true;
                }
            }

            Debug.LogError($"Object that has '{typeof(T).FullName}' is not set in Inspector. at '{this.GetType().FullName}'");
            return false;
        }
    }
}