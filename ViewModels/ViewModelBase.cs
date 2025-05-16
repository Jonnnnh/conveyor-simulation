using System;
using ReactiveUI;

namespace ConveyorApp.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable
{
    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}