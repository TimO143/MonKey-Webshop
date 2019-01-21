using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Resource.Option
{
    public interface Option<T>
    {
        bool empty { get; }
        T data { get; set; }

    }
}