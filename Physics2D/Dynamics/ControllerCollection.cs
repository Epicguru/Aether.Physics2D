// Copyright (c) 2021 Kastellanos Nikolaos

using System;
using System.Collections;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Controllers;

namespace nkast.Aether.Physics2D.Dynamics;

public class ControllerCollection : IEnumerable<Controller>
    , ICollection<Controller>, IList<Controller>
{
    internal readonly List<Controller> _list = new List<Controller>(32);
    internal int _generationStamp = 0;
    private readonly World _world;

    public ControllerCollection(World world)
    {
        _world = world;
    }


    #region IEnumerable<Controller>

    IEnumerator<Controller> IEnumerable<Controller>.GetEnumerator()
    {
        return new ControllerEnumerator(this, _list);
    }

    #endregion IEnumerable<Controller>


    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new ControllerEnumerator(this, _list);
    }

    #endregion IEnumerable

    public ControllerEnumerator GetEnumerator()
    {
        return new ControllerEnumerator(this, _list);
    }


    public struct ControllerEnumerator : IEnumerator<Controller>
    {
        private ControllerCollection _collection;
        private List<Controller> _list;
        private readonly int _generationStamp;
        private int i;

        public ControllerEnumerator(ControllerCollection collection, List<Controller> list)
        {
            _collection = collection;
            _list = list;
            _generationStamp = collection._generationStamp;
            i = -1;
        }

        public Controller Current
        {
            get
            {
                if (_generationStamp == _collection._generationStamp)
                    return _list[i];
                throw new InvalidOperationException("Collection was modified.");
            }
        }

        #region IEnumerator<Controller>

        Controller IEnumerator<Controller>.Current
        {
            get
            {
                if (_generationStamp == _collection._generationStamp)
                    return _list[i];
                throw new InvalidOperationException("Collection was modified.");
            }
        }

        #endregion IEnumerator<Controller>

        #region IEnumerator

        public bool MoveNext()
        {
            if (_generationStamp != _collection._generationStamp)
                throw new InvalidOperationException("Collection was modified.");

            return ++i < _list.Count;
        }


        object IEnumerator.Current
        {
            get
            {
                if (_generationStamp == _collection._generationStamp)
                    return _list[i];
                throw new InvalidOperationException();
            }
        }

        void IDisposable.Dispose()
        {
            _collection = null;
            _list = null;
            i = -1;
        }

        void IEnumerator.Reset()
        {
            i = -1;
        }

        #endregion IEnumerator
    }


    #region IList<Controller>

    public Controller this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException();
    }

    public int IndexOf(Controller item)
    {
        return _list.IndexOf(item);
    }

    void IList<Controller>.Insert(int index, Controller item)
    {
        throw new NotSupportedException();
    }

    void IList<Controller>.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    #endregion IList<Controller>


    #region ICollection<Controller>

    public bool IsReadOnly => true;

    public int Count => _list.Count;

    void ICollection<Controller>.Add(Controller item)
    {
        throw new NotSupportedException();
    }

    bool ICollection<Controller>.Remove(Controller item)
    {
        throw new NotSupportedException();
    }

    void ICollection<Controller>.Clear()
    {
        throw new NotSupportedException();
    }

    public bool Contains(Controller item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(Controller[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    #endregion ICollection<Controller>
}