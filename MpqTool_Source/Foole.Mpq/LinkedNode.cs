namespace Foole.Mpq
{
    using System;

    internal class LinkedNode
    {
        public LinkedNode Child0;
        public int DecompressedValue;
        public LinkedNode Next;
        public LinkedNode Parent;
        public LinkedNode Prev;
        public int Weight;

        public LinkedNode(int decompVal, int weight)
        {
            this.DecompressedValue = decompVal;
            this.Weight = weight;
        }

        public LinkedNode Insert(LinkedNode other)
        {
            if (other.Weight <= this.Weight)
            {
                if (this.Next != null)
                {
                    this.Next.Prev = other;
                    other.Next = this.Next;
                }
                this.Next = other;
                other.Prev = this;
                return other;
            }
            if (this.Prev == null)
            {
                other.Prev = null;
                this.Prev = other;
                other.Next = this;
            }
            else
            {
                this.Prev.Insert(other);
            }
            return this;
        }

        public LinkedNode Child1
        {
            get
            {
                return this.Child0.Prev;
            }
        }
    }
}

