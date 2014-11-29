using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphTest
{
    //キャッシュ作成用の仮想クラス
    public abstract class DBCache<T> where T : new()
    {
        protected Dictionary<int, T> _dic=null;
        public T Empty { get; set; }

        public DBCache()
        {
            _dic = new Dictionary<int, T>();
            this.Empty = new T(); // 空を設定
        }

        public void Set(int key, T value)
        {
            //if (_dic.ContainsKey(key) == true)
           // {
           //     _dic[key] = value;
           // }
           // else
           // {
           //     _dic.Add(key, value);
           // }
            _dic[key] = value;
        }

        public T Get(int key)
        {
            if (_dic.ContainsKey(key) == true)
            {
                return _dic[key];
            }
            else
            {
                return this.Empty;
            }
        }

        public Dictionary<int,T>.KeyCollection Keys()
        {
            if (_dic != null)
            {
                return _dic.Keys;
            }
            else
            {
                return null;
            }
        }

        public int KeyLength()
        {
            if (_dic != null)
            {
                return _dic.Keys.Count;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// キー情報を作成
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract int MakeKey(T it);


        /// <summary>
        /// データを設定
        /// </summary>
        /// <param name="table"></param>
       /* public void SetData(List<T> items)
        {
            foreach (T it in items)
            {
                this.Set(MakeKey(it), it);
            }
        }*/

        /// <summary>
        /// データを設定
        /// </summary>
        /// <param name="table"></param>
        public void SetData(T it)
        {
                this.Set(MakeKey(it), it);
        }

    }


}
