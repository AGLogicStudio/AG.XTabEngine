using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.XTabEngine.Aggregators
{
    public interface IAggregator<T>
    {
        void Add(T item);
        double Result { get; }
    }

    public interface IAggregatorFactory<T>
    {
        IAggregator<T> Create(Func<T, double> selector);
    }


}
