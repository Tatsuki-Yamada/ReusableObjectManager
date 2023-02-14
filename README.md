# ReusableObjectManager
一度生成したオブジェクトを再利用する、汎用マネージャークラス。継承して使っても、そのまま使用することも可能。

## 別途必要なパッケージ
- UniRx

## インストール方法
1. Package Manager を開き、+ボタンから"Add package from git URL..."を選択する。
1. https://github.com/Tatsuki-Yamada/ReusableObjectManager.git?path=Packages/com.tatsuki-yamada.reusableobjectmanager  
を入力し、Addボタンを押す。

## 使用方法
- `using ReusableObjectManagement;` を追加する。
- 管理するオブジェクトには、`IHasAlive` インターフェースを継承させる必要がある。 

### IHasAlive のプロパティ  
  - **BoolReactiveProperty isAlive** :  
    - オブジェクトが生きているか示すフラグで、isAlive.Value=false かつ後述の canReuse=true のときに再利用されるオブジェクトとなる。
  - **IObservable<bool> isAliveObserver => isAlive** :  
    - isAliveにUniRxのイベントを仕込むためのObserver
  - **bool canReuse** :  
    - isAlive=falseのみだと生成した瞬間に再利用されるため、初期化処理が終わるまでisAliveの判定を待つためのフラグ  
    
> IHasAliveを継承したオブジェクトは、初期化処理で isAlive.Value=true をした後に、canReuse=trueをする必要がある。<br>また、破棄して再利用するには isAlive.Value=false をする必要がある。


### ReusableObjectManagerのpublicメソッド
- **CreateOrReuse\<T>()** :  
  - Inspectorで登録されたPrefabから、Tクラスを持つものを探して生成する。<br>
  **return** : 生成・再利用されたT
- **TryGetManagementList<T>()** :
  - Managerで管理しているすべてのTクラスをListで渡す。  
  **return** : Tの管理リスト

