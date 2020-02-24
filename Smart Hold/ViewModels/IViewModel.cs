using Smart_Hold.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Hold.ViewModels
{
    public interface IViewModel<T> : IViewModel where T : IView
    {
        new T View
        {
            get;
            set;
        }
    }

    public interface IViewModel
    {
        object View
        {
            get;
            set;
        }
    }
}
