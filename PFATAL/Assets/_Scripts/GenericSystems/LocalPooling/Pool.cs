using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Pooling
{
    /// <summary>
    /// Ce script n'est pas un singleton car on peut avoir une pool par prefab dans la sc�ne,avec chacune des tailles diff�rentes.
    /// Dans un plus gros projet, on pourrait coder un script poolmanager qui serait un singleton avec dedans toutes les pools et des enums pour faire spawn n'importe quelle prefab facilement etc, mais KISS.
    /// 
    /// Messages optionnels envoy�s aux objets de la pool:
    /// - OnInstantiatedByPool
    /// - OnPulledFromPool
    /// - OnPutBackIntoPool
    /// </summary>
    public class Pool : MonoBehaviour
    {
        [SerializeField] GameObject _prefab;
        [FormerlySerializedAs("_poolSize")]
        [SerializeField] internal uint BaseSize = 50;
        [SerializeField] internal uint GrowRateWhenEmpty = 5;

        private List<PooledObject> _instances= new List<PooledObject>() ; //toutes les instances d'objets, actives comme inactives.
        private List<int> _freeIndices = new List<int>(); //contient tous les indices des instances inactives dans le tableau _instances
        public bool isEmpty => _freeIndices.Count == 0;
        public int freeInstancesCount => _freeIndices.Count;
        
        private void Awake()
        {
            LocalPoolManager.Instance.AllPools.Add(this);
            PopulatePool(BaseSize);
        }

        /// <summary>
        /// peuple la pool en instanciant des GameObjects ayant tous un component PooledObject
        /// </summary>
        internal void PopulatePool(uint instanceCount)
        {
            
            //regarde si il y'aura besoin d'ajouter le component PooledObject aux instances de la prefab,ou si il y est d�j�.
            bool PrefabAlreadyHasPooledObjectComponent = _prefab.GetComponent<PooledObject>();

            int baseInstanceCount = _instances.Count;
            
            //cr�e toutes les instances de la prefab et les met dans la pool.
            for (int i = 0; i < instanceCount; i++)
            {
                if (_prefab)
                {
                    GameObject instancedObject = GameObject.Instantiate(_prefab);
                    instancedObject.name += i.ToString();

                    PooledObject po = PrefabAlreadyHasPooledObjectComponent ? instancedObject.GetComponent<PooledObject>() : instancedObject.AddComponent<PooledObject>();
                    po.Index = baseInstanceCount + i;
                    po.Pool = this;
                    po.IsInPool = true;
                    _instances.Add(po);

                    //message optionnel
                    instancedObject.BroadcastMessage("OnInstantiatedByPool", SendMessageOptions.DontRequireReceiver);

                    PutObjectBackInPool(po);
                }
            }
        }


        /// <summary>
        /// permet de faire "spawn" un gameObject inactif depuis la pool. � appeler � la place de GAMEOBJECT.INSTANTIATE()
        /// </summary>
        /// <param name="Parent"></param>
        /// <returns></returns>
        public PooledObject PullObjectFromPool(Transform Parent = null,bool final = true)
        {
            //Assert.IsTrue(_freeIndices.Count > 0, $"Pool \"{gameObject.name}\" is empty!");

            if (_freeIndices.Count == 0)
            {
                Debug.LogWarning($"Pool was empty ! adding {GrowRateWhenEmpty} instances !");
                 PopulatePool(GrowRateWhenEmpty); 
            }
            
            //pioche le premier indice libre dans la pool.
            int id = _freeIndices[0];
            _freeIndices.Remove(id);

            //activation de l'objet s�lectionn�
            PooledObject o = _instances[id];
            o.IsInPool = false;
            o.gameObject.SetActive(true);
            o.transform.parent = Parent;

            //message optionnel
            if(final)o.gameObject.BroadcastMessage("OnPulledFromPool", SendMessageOptions.DontRequireReceiver);
            return o;
        }

        /// <summary>
        /// permet de faire "spawn" un gameObject inactif depuis la pool � une certaine position. 
        /// � appeler � la place de GAMEOBJECT.INSTANTIATE()
        /// </summary>
        public PooledObject PullObjectFromPool(Vector3 Position, Transform Parent = null,bool final = true)
        {
            PooledObject o = PullObjectFromPool(Parent,false);
            o.transform.position = Position;
        
            //message optionnel
            if(final)o.gameObject.BroadcastMessage("OnPulledFromPool", SendMessageOptions.DontRequireReceiver);
            return o;
        }

        /// <summary>
        /// permet de faire "spawn" un gameObject inactif depuis la pool � une certaine position et rotation. 
        /// � appeler � la place de GAMEOBJECT.INSTANTIATE()
        /// </summary>
        public PooledObject PullObjectFromPool(Vector3 Position, Quaternion rotation, Transform Parent = null)
        {
            PooledObject o = PullObjectFromPool(Position, Parent,false);
            o.transform.rotation = rotation;
        
            //message optionnel
            o.gameObject.BroadcastMessage("OnPulledFromPool", SendMessageOptions.DontRequireReceiver);
            return o;
        }



        /// <summary>
        /// desactive un objet et le remet dans la pool.
        /// le component PooledObject contient �galement une m�thode GoBackIntoPool() pour faire "despawn" l'objet facilement.
        /// </summary>
        /// <param name="ObjectToPool"></param>
        public void PutObjectBackInPool(PooledObject ObjectToPool)
        {
            if(!ObjectToPool)return;
            if (!_freeIndices.Contains(ObjectToPool.Index))
            {
                _freeIndices.Add(ObjectToPool.Index);
                ObjectToPool.gameObject.BroadcastMessage("OnPutBackIntoPool", SendMessageOptions.DontRequireReceiver);

            }
            ObjectToPool.transform.parent = transform;
            ObjectToPool.gameObject.SetActive(false);
            ObjectToPool.IsInPool = true;

        }


    }
}
