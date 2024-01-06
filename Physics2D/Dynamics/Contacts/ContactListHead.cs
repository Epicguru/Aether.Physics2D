// Copyright (c) 2017 Kastellanos Nikolaos

using System.Collections;
using System.Collections.Generic;

namespace nkast.Aether.Physics2D.Dynamics.Contacts;

/// <summary>Head of a circular doubly linked list.</summary>
public class ContactListHead : Contact, IEnumerable<Contact>
{
    internal ContactListHead() : base(null, 0, null, 0)
    {
        Prev = this;
        Next = this;
    }

    IEnumerator<Contact> IEnumerable<Contact>.GetEnumerator()
    {
        return new ContactEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new ContactEnumerator(this);
    }


    #region Nested type: ContactEnumerator

    private struct ContactEnumerator : IEnumerator<Contact>
    {
        private ContactListHead _head;

        public Contact Current { get; private set; }

        object IEnumerator.Current => Current;


        public ContactEnumerator(ContactListHead contact)
        {
            _head = contact;
            Current = _head;
        }

        public void Reset()
        {
            Current = _head;
        }

        public bool MoveNext()
        {
            Current = Current.Next;
            return Current != _head;
        }

        public void Dispose()
        {
            _head = null;
            Current = null;
        }
    }

    #endregion
}