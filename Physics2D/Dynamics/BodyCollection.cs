﻿// Copyright (c) 2021 Kastellanos Nikolaos

using System;
using System.Collections;
using System.Collections.Generic;

namespace nkast.Aether.Physics2D.Dynamics;

public class BodyCollection : IEnumerable<Body>
    , ICollection<Body>, IList<Body>
{
    internal readonly List<Body> _list = new List<Body>(32);
    internal int _generationStamp = 0;
    private readonly World _world;

    public BodyCollection(World world)
    {
        _world = world;
    }


    #region IEnumerable<Body>

    IEnumerator<Body> IEnumerable<Body>.GetEnumerator()
    {
        return new BodyEnumerator(this, _list);
    }

    #endregion IEnumerable<Body>


    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new BodyEnumerator(this, _list);
    }

    #endregion IEnumerable

    public BodyEnumerator GetEnumerator()
    {
        return new BodyEnumerator(this, _list);
    }


    public struct BodyEnumerator : IEnumerator<Body>
    {
        private BodyCollection _collection;
        private List<Body> _list;
        private readonly int _generationStamp;
        private int i;

        public BodyEnumerator(BodyCollection collection, List<Body> list)
        {
            _collection = collection;
            _list = list;
            _generationStamp = collection._generationStamp;
            i = -1;
        }

        public Body Current
        {
            get
            {
                if (_generationStamp == _collection._generationStamp)
                    return _list[i];
                throw new InvalidOperationException("Collection was modified.");
            }
        }

        #region IEnumerator<Body>

        Body IEnumerator<Body>.Current
        {
            get
            {
                if (_generationStamp == _collection._generationStamp)
                    return _list[i];
                throw new InvalidOperationException("Collection was modified.");
            }
        }

        #endregion IEnumerator<Body>

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


    #region IList<Body>

    public Body this[int index]
    {
        get => _list[index];
        set => throw new NotSupportedException();
    }

    public int IndexOf(Body item)
    {
        return _list.IndexOf(item);
    }

    void IList<Body>.Insert(int index, Body item)
    {
        throw new NotSupportedException();
    }

    void IList<Body>.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    #endregion IList<Body>


    #region ICollection<Body>

    public bool IsReadOnly => true;

    public int Count => _list.Count;

    void ICollection<Body>.Add(Body item)
    {
        throw new NotSupportedException();
    }

    bool ICollection<Body>.Remove(Body item)
    {
        throw new NotSupportedException();
    }

    void ICollection<Body>.Clear()
    {
        throw new NotSupportedException();
    }

    public bool Contains(Body item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(Body[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    #endregion ICollection<Body>
}